using UniversityManagementSystem.Application.DTOs.Registration;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ISemesterOfferedCourseRepository _offeredRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IAuditLogService _auditLogService;
        private readonly IFeeService _feeService;

        private const int MinimumCredits = 0;
        private const int MaximumCredits = 24;

        public RegistrationService(
            IRegistrationRepository registrationRepo,
            IEnrollmentRepository enrollmentRepo,
            ISemesterOfferedCourseRepository offeredRepo,
            IStudentRepository studentRepo,
            IAuditLogService auditLogService,
            IFeeService feeService)
        {
            _registrationRepo = registrationRepo;
            _enrollmentRepo = enrollmentRepo;
            _offeredRepo = offeredRepo;
            _studentRepo = studentRepo;
            _auditLogService = auditLogService;
            _feeService = feeService;
        }

        public async Task<IEnumerable<SemesterOfferedCourseListDto>> GetOpenOfferingsBySemesterAsync(int semesterId, int studentId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                throw new NotFoundException(nameof(Student), studentId);

            var offerings = await _offeredRepo.GetOpenBySemesterIdAsync(semesterId);

            var result = new List<SemesterOfferedCourseListDto>();
            foreach (var offering in offerings)
            {
                var registered = await _offeredRepo.IsStudentRegisteredAsync(studentId, offering.Id);
                var registeredCount = await _offeredRepo.GetRegisteredCountAsync(offering.Id);

                result.Add(new SemesterOfferedCourseListDto
                {
                    Id = offering.Id,
                    CourseId = offering.CourseId,
                    CourseTitle = offering.Course?.Title ?? string.Empty,
                    CourseCode = offering.Course?.Code ?? string.Empty,
                    Credits = offering.Course?.Credits ?? 0,
                    DepartmentName = offering.Department?.Name ?? string.Empty,
                    Capacity = offering.Capacity,
                    RegisteredCount = registeredCount,
                    IsStudentRegistered = registered,
                    SemesterId = offering.SemesterId,
                    DepartmentId = offering.DepartmentId
                });
            }

            return result;
        }        public async Task<RegistrationSubmissionResultDto> SubmitRegistrationAsync(int studentId, int semesterId, IEnumerable<int> offeredCourseIds)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                throw new NotFoundException(nameof(Student), studentId);

            // Finance clearance gate: self-sponsored students must have paid the
            // required first installment before they can register (Admin can
            // disable this in Finance Settings).
            var clearance = await _feeService.CheckRegistrationClearanceAsync(studentId, semesterId);
            if (!clearance.IsCleared)
                throw new InvalidDataException($"Finance hold: {clearance.Message} Please complete your payment on the My Finance page and try again.");

            var offerings = await _offeredRepo.GetOpenBySemesterIdAsync(semesterId);
            var offeringDict = offerings.ToDictionary(o => o.Id, o => o);

            var ids = offeredCourseIds.Distinct().ToList();
            if (!ids.Any())
                throw new InvalidDataException("Select at least one course to register.");

            var selectedOfferings = ids
                .Select(id => offeringDict.GetValueOrDefault(id))
                .Where(o => o != null)
                .Cast<SemesterOfferedCourse>()
                .ToList();

            if (selectedOfferings.Count != ids.Count)
            {
                var invalidIds = ids.Where(id => !offeringDict.ContainsKey(id)).ToList();
                throw new InvalidDataException($"Some selected courses are closed or not offered this semester: {string.Join(", ", invalidIds)}.");
            }

            foreach (var offering in selectedOfferings)
            {
                if (offering.Capacity <= await _offeredRepo.GetRegisteredCountAsync(offering.Id))
                    throw new InvalidDataException($"There are no seats left in {offering.Course?.Code}. Please remove it and select another course.");
            }

            var totalCredits = selectedOfferings.Sum(o => o.Course?.Credits ?? 0);
            if (totalCredits < MinimumCredits)
                throw new InvalidDataException("Total selected credits cannot be below the minimum registration load.");
            if (totalCredits > MaximumCredits)
                throw new InvalidDataException($"Total selected credits ({totalCredits}) exceed the maximum allowed ({MaximumCredits}).");

            var activeRegistration = await _registrationRepo.GetByStudentAndSemesterAsync(studentId, semesterId);
            if (activeRegistration != null
                && (activeRegistration.Status == RegistrationStatus.Submitted
                    || activeRegistration.Status == RegistrationStatus.Approved))
                throw new DuplicateException("You already have an active registration for this semester.");

            foreach (var offering in selectedOfferings)
            {
                if (await _offeredRepo.IsStudentRegisteredAsync(studentId, offering.Id))
                    throw new DuplicateException($"You are already registered for {offering.Course?.Code}.");
            }

            // Build schedules AFTER collecting selectedOfferings so the conflict check below
            // operates on the same non-null list.
            var schedules = new Dictionary<int, List<OfferedCourseSchedule>>();
            foreach (var offering in selectedOfferings)
            {
                schedules[offering.Id] = (await _offeredRepo.GetSchedulesAsync(offering.Id)).ToList();
            }

            var conflicts = FindConflicts(selectedOfferings, schedules);
            if (conflicts.Any())
            {
                var conflictCodes = conflicts
                    .SelectMany(c => new[] { c.Item1.Course?.Code ?? string.Empty, c.Item2.Course?.Code ?? string.Empty })
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
                throw new InvalidDataException($"Registration has schedule conflicts: {string.Join(", ", conflictCodes)}.");
            }

            var registration = new Registration
            {
                StudentId = studentId,
                SemesterId = semesterId,
                DepartmentId = student.DepartmentId,
                Status = RegistrationStatus.Submitted,
                TotalCredits = totalCredits
            };

            foreach (var offering in selectedOfferings)
            {
                var registeredCount = await _offeredRepo.GetRegisteredCountAsync(offering.Id);
                var item = new RegistrationItem
                {
                    SemesterOfferedCourseId = offering.Id,
                    Credits = offering.Course?.Credits ?? 0,
                    Status = offering.Capacity > registeredCount
                        ? RegistrationItemStatus.Registered
                        : RegistrationItemStatus.Waitlisted
                };
                registration.Items.Add(item);
            }

            await _registrationRepo.AddAsync(registration);
            await _registrationRepo.SaveChangesAsync();

            await _auditLogService.LogAsync(
                student.ApplicationUserId,
                $"{student.FirstName} {student.LastName}",
                "Student",
                "RegistrationSubmit",
                "Registration",
                registration.Id,
                $"Submitted registration for semester {semesterId} with {totalCredits} credits.");

            return new RegistrationSubmissionResultDto
            {
                RegistrationId = registration.Id,
                TotalCredits = registration.TotalCredits,
                Status = registration.Status.ToString(),
                Message = "Registration submitted successfully."
            };
        }

        public async Task<RegistrationDetailDto?> GetRegistrationAsync(int id, int studentId)
        {
            var registration = await _registrationRepo.GetByIdWithDetailsAsync(id);
            if (registration == null || registration.StudentId != studentId)
                return null;

            return MapDetail(registration);
        }

        public async Task<RegistrationDetailDto?> GetRegistrationForReviewAsync(int id)
        {
            var registration = await _registrationRepo.GetByIdWithDetailsAsync(id);
            return registration == null ? null : MapDetail(registration);
        }

        public async Task<IEnumerable<RegistrationListDto>> GetStudentRegistrationsAsync(int studentId)
        {
            var registrations = await _registrationRepo.GetByStudentIdAsync(studentId);
            return registrations.Select(MapList);
        }

        public async Task<IEnumerable<RegistrationListDto>> GetSemesterRegistrationsAsync(int semesterId)
        {
            var registrations = await _registrationRepo.GetBySemesterIdAsync(semesterId);
            return registrations.Select(MapList);
        }

        public async Task<IEnumerable<RegistrationListDto>> GetPendingRegistrationsAsync(int semesterId)
        {
            var registrations = await _registrationRepo.GetPendingBySemesterIdAsync(semesterId);
            return registrations.Select(MapList);
        }

        public async Task<RegistrationListDto> ReviewRegistrationAsync(int id, RegistrationReviewDto dto)
        {
            var registration = await _registrationRepo.GetByIdWithDetailsAsync(id);
            if (registration == null)
                throw new NotFoundException(nameof(Registration), id);

            var newStatus = Enum.Parse<RegistrationStatus>(dto.NewStatus, ignoreCase: true);
            if (registration.Status == newStatus)
                throw new InvalidDataException("Registration already has this status.");

            registration.Status = newStatus;
            registration.ReviewedAt = DateTime.UtcNow;
            registration.RejectionReason = dto.RejectionReason;

            foreach (var item in registration.Items)
            {
                item.Status = newStatus switch
                {
                    RegistrationStatus.Approved => item.Status == RegistrationItemStatus.Registered
                        ? RegistrationItemStatus.Registered
                        : RegistrationItemStatus.Waitlisted,
                    RegistrationStatus.Rejected => RegistrationItemStatus.Rejected,
                    _ => item.Status
                };
            }

            if (newStatus == RegistrationStatus.Approved)
            {
                var studentEnrollments = (await _enrollmentRepo.GetEnrollmentsByStudentIdAsync(registration.StudentId)).ToList();
                foreach (var item in registration.Items.Where(i => i.Status == RegistrationItemStatus.Registered))
                {
                    var offering = item.SemesterOfferedCourse
                        ?? await _offeredRepo.GetByIdAsync(item.SemesterOfferedCourseId);
                    if (offering == null)
                        throw new NotFoundException(nameof(SemesterOfferedCourse), item.SemesterOfferedCourseId);

                    var enrollment = studentEnrollments.FirstOrDefault(e => e.CourseId == offering.CourseId);
                    if (enrollment == null)
                    {
                        enrollment = new Enrollment
                        {
                            StudentId = registration.StudentId,
                            CourseId = offering.CourseId,
                                    EnrollmentDate = registration.CreatedAt,
                            Status = EnrollmentStatus.Approved,
                            ApprovalDate = registration.ReviewedAt
                        };
                        await _enrollmentRepo.AddAsync(enrollment);
                        studentEnrollments.Add(enrollment);
                    }
                    else
                    {
                        enrollment.EnrollmentDate = registration.CreatedAt;
                        enrollment.Status = EnrollmentStatus.Approved;
                        enrollment.ApprovalDate = registration.ReviewedAt;
                        enrollment.RejectionReason = null;
                        await _enrollmentRepo.UpdateAsync(enrollment);
                    }
                }
            }

            await _registrationRepo.AddStatusHistoryAsync(new RegistrationStatusHistory
            {
                RegistrationId = registration.Id,
                OldStatus = GetEffectiveOldStatus(registration),
                NewStatus = newStatus,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = dto.ReviewedBy,
                Reason = dto.RejectionReason
            });

            await _registrationRepo.SaveChangesAsync();
            registration.Items = registration.Items.ToList();
            registration.StatusHistory = registration.StatusHistory.ToList();
            registration.Student = registration.Student;
            registration.Semester = registration.Semester;
            registration.Department = registration.Department;

            await _auditLogService.LogAsync(
                dto.ReviewedBy,
                "Admin",
                "Admin",
                "RegistrationReview",
                "Registration",
                id,
                $"Reviewed registration {id} -> {newStatus}.");

            return MapList(registration);
        }

        public async Task<RegistrationListDto> WithdrawRegistrationAsync(int id, int studentId)
        {
            var registration = await _registrationRepo.GetByIdWithDetailsAsync(id);
            if (registration == null || registration.StudentId != studentId)
                throw new NotFoundException(nameof(Registration), id);

            if (registration.Status != RegistrationStatus.Submitted)
                throw new InvalidDataException("Only submitted registrations can be withdrawn.");

            registration.Status = RegistrationStatus.Withdrawn;
            await _registrationRepo.SaveChangesAsync();

            await _auditLogService.LogAsync(
                registration.Student?.ApplicationUserId ?? "unknown",
                registration.Student != null
                    ? $"{registration.Student.FirstName} {registration.Student.LastName}"
                    : "Unknown",
                "Student",
                "RegistrationWithdraw",
                "Registration",
                id,
                $"Withdrew registration {id}.");

            return MapList(registration);
        }

        private static RegistrationStatus GetEffectiveOldStatus(Registration registration)
        {
            var last = registration.StatusHistory.OrderByDescending(h => h.CreatedAt).FirstOrDefault();
            if (last == null)
                return RegistrationStatus.Draft;
            return last.OldStatus == default ? RegistrationStatus.Draft : last.NewStatus;
        }

        private static List<(SemesterOfferedCourse, SemesterOfferedCourse)> FindConflicts(
            List<SemesterOfferedCourse> offerings,
            Dictionary<int, List<OfferedCourseSchedule>> schedules)
        {
            var conflicts = new List<(SemesterOfferedCourse, SemesterOfferedCourse)>();

            for (int i = 0; i < offerings.Count; i++)
            {
                for (int j = i + 1; j < offerings.Count; j++)
                {
                    var a = offerings[i];
                    var b = offerings[j];
                    var schedulesA = schedules.GetValueOrDefault(a.Id) ?? new List<OfferedCourseSchedule>();
                    var schedulesB = schedules.GetValueOrDefault(b.Id) ?? new List<OfferedCourseSchedule>();

                    foreach (var sa in schedulesA)
                    {
                        foreach (var sb in schedulesB)
                        {
                            if (sa.DayOfWeek == sb.DayOfWeek
                                && sa.StartTime < sb.EndTime
                                && sb.StartTime < sa.EndTime)
                            {
                                conflicts.Add((a, b));
                                break;
                            }
                        }
                        if (conflicts.Any(c => c.Item1.Id == a.Id && c.Item2.Id == b.Id))
                            break;
                    }
                }
            }

            return conflicts;
        }

        private static RegistrationDetailDto MapDetail(Registration registration)
        {
            return new RegistrationDetailDto
            {
                Id = registration.Id,
                StudentId = registration.StudentId,
                StudentName = registration.Student != null
                    ? $"{registration.Student.FirstName} {registration.Student.LastName}"
                    : string.Empty,
                StudentNumber = registration.Student?.StudentId ?? string.Empty,
                SemesterId = registration.SemesterId,
                SemesterName = registration.Semester?.Name ?? string.Empty,
                DepartmentId = registration.DepartmentId,
                DepartmentName = registration.Department?.Name ?? string.Empty,
                TotalCredits = registration.TotalCredits,
                Status = registration.Status.ToString(),
                ReviewedAt = registration.ReviewedAt,
                RejectionReason = registration.RejectionReason,
                Items = registration.Items.Select(i => new RegistrationItemDto
                {
                    Id = i.Id,
                    SemesterOfferedCourseId = i.SemesterOfferedCourseId,
                    CourseTitle = i.SemesterOfferedCourse?.Course?.Title ?? string.Empty,
                    CourseCode = i.SemesterOfferedCourse?.Course?.Code ?? string.Empty,
                    Credits = i.Credits,
                    Status = i.Status.ToString(),
                    Note = i.Note
                }).ToList(),
                StatusHistory = registration.StatusHistory.Select(h => new RegistrationStatusHistoryDto
                {
                    Id = h.Id,
                    OldStatus = h.OldStatus.ToString(),
                    NewStatus = h.NewStatus.ToString(),
                    ChangedAt = h.ChangedAt,
                    ChangedBy = h.ChangedBy,
                    Reason = h.Reason
                }).ToList()
            };
        }

        private static RegistrationListDto MapList(Registration registration)
        {
            return new RegistrationListDto
            {
                Id = registration.Id,
                StudentId = registration.StudentId,
                StudentName = registration.Student != null
                    ? $"{registration.Student.FirstName} {registration.Student.LastName}"
                    : string.Empty,
                StudentNumber = registration.Student?.StudentId ?? string.Empty,
                SemesterId = registration.SemesterId,
                SemesterName = registration.Semester?.Name ?? string.Empty,
                DepartmentName = registration.Department?.Name ?? string.Empty,
                TotalCredits = registration.TotalCredits,
                Status = registration.Status.ToString(),
                CreatedAt = registration.CreatedAt,
                ReviewedAt = registration.ReviewedAt,
                RejectionReason = registration.RejectionReason
            };
        }
    }
}
