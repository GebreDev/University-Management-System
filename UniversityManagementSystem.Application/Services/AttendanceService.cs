using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Attendance;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AttendanceService(IAttendanceRepository attRepo, IStudentRepository studentRepo, ICourseRepository courseRepo, IMapper mapper, INotificationService notificationService)
        {
            _attRepo = attRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task MarkAttendanceAsync(AttendanceCreateDto dto)
        {
            var existing = await _attRepo.GetByStudentAndDateAsync(dto.StudentId, dto.CourseId, dto.Date);
            if (existing != null)
            {
                // Saving the same class date again edits the existing record instead of failing.
                existing.Status = dto.Status;
                existing.Remarks = dto.Remarks;
                await _attRepo.UpdateAsync(existing);
                await _attRepo.SaveChangesAsync();
                return;
            }

            var attendance = _mapper.Map<Attendance>(dto);
            await _attRepo.AddAsync(attendance);
            await _attRepo.SaveChangesAsync();

            // Best-effort notifications — failure must never break the attendance save
            if (dto.Status == AttendanceStatus.Absent || dto.Status == AttendanceStatus.Late)
            {
                try
                {
                    var student = await _studentRepo.GetByIdAsync(dto.StudentId);
                    if (student != null)
                    {
                        await _notificationService.SendNotificationAsync(student.ApplicationUserId, "Attendance Warning", $"You were marked {dto.Status} on {dto.Date.ToShortDateString()}", "Attendance", dto.StudentId.ToString());

                        // Check attendance threshold (75%)
                        var summary = await GetAttendanceSummaryAsync(dto.StudentId, dto.CourseId);
                        if (summary.Percentage < 75.0)
                        {
                            await _notificationService.SendNotificationAsync(
                                student.ApplicationUserId,
                                "Low Attendance Alert",
                                $"Your attendance is below the 75% threshold (currently {summary.Percentage:F1}%). Please contact your instructor.",
                                "Attendance",
                                dto.StudentId.ToString()
                            );
                        }
                    }
                }
                catch
                {
                    // Notification failure must not break attendance saving
                }
            }
        }

        public async Task MarkBulkAttendanceAsync(BulkAttendanceDto dto)
        {
            foreach (var att in dto.Attendances)
            {
                await MarkAttendanceAsync(att);
            }
        }

        public async Task UpdateAttendanceAsync(AttendanceUpdateDto dto)
        {
            var att = await _attRepo.GetByIdAsync(dto.Id);
            if (att == null) throw new Exception("Attendance not found.");
            _mapper.Map(dto, att);
            await _attRepo.UpdateAsync(att);
            await _attRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<AttendanceListDto>> GetAttendanceByStudentIdAsync(int studentId)
        {
            var list = await _attRepo.GetByStudentIdAsync(studentId);
            return _mapper.Map<IEnumerable<AttendanceListDto>>(list);
        }

        public async Task<IEnumerable<AttendanceListDto>> GetAttendanceByCourseIdAsync(int courseId)
        {
            var list = await _attRepo.GetByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<AttendanceListDto>>(list);
        }

        public async Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(int studentId, int? courseId = null)
        {
            var all = await _attRepo.GetByStudentIdAsync(studentId);
            if (courseId.HasValue)
                all = all.Where(a => a.CourseId == courseId.Value);

            var summary = new AttendanceSummaryDto
            {
                TotalDays = all.Count(),
                Present = all.Count(a => a.Status == AttendanceStatus.Present),
                Absent = all.Count(a => a.Status == AttendanceStatus.Absent),
                Late = all.Count(a => a.Status == AttendanceStatus.Late),
                Excused = all.Count(a => a.Status == AttendanceStatus.Excused),
                Percentage = all.Any() ? (double)all.Count(a => a.Status == AttendanceStatus.Present) / all.Count() * 100 : 0
            };
            return summary;
        }

        public async Task<AttendanceStatsDto> GetAttendanceStatsAsync()
        {
            var today = DateTime.Today;
            var todayRecords = await _attRepo.GetByDateAsync(today);
            var todayTotal = todayRecords.Count();
            var todayPresent = todayRecords.Count(a => a.Status == AttendanceStatus.Present);
            var todayAbsent = todayRecords.Count(a => a.Status == AttendanceStatus.Absent);

            // Monthly stats
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var allMonthRecords = await _attRepo.GetByDateAsync(monthStart);
            // Fallback: iterate days to get all month records
            var monthRecords = new List<Attendance>();
            // Get all attendances for the current month more broadly
            var courses = (await _courseRepo.GetAllAsync()).ToList();
            foreach (var course in courses)
            {
                var courseAtt = await _attRepo.GetByCourseIdAsync(course.Id);
                monthRecords.AddRange(courseAtt.Where(a => a.Date.Year == today.Year && a.Date.Month == today.Month));
            }
            var totalMonthAttendances = monthRecords.Count;
            var presentMonth = monthRecords.Count(a => a.Status == AttendanceStatus.Present);
            var monthlyAverage = totalMonthAttendances > 0 ? (double)presentMonth / totalMonthAttendances * 100 : 0;

            // Frequently absent (students with > 3 absences this month)
            var frequentlyAbsent = monthRecords
                .Where(a => a.Status == AttendanceStatus.Absent)
                .GroupBy(a => a.StudentId)
                .Count(g => g.Count() > 3);

            // Daily trend for last 7 days
            var dailyLabels = new List<string>();
            var dailyPresent = new List<int>();
            for (int i = 6; i >= 0; i--)
            {
                var day = today.AddDays(-i);
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                dailyLabels.Add(day.ToString("ddd"));
                var dayRecords = await _attRepo.GetByDateAsync(day);
                dailyPresent.Add(dayRecords.Count(a => a.Status == AttendanceStatus.Present));
            }

            return new AttendanceStatsDto
            {
                TodayPresent = todayPresent,
                TodayTotal = todayTotal,
                MonthlyAverage = Math.Round(monthlyAverage, 1),
                AbsentToday = todayAbsent,
                FrequentlyAbsentCount = frequentlyAbsent,
                DailyLabels = dailyLabels,
                DailyPresent = dailyPresent
            };
        }

        public async Task<IEnumerable<AttendanceReportDto>> GetAllAttendanceReportsAsync()
        {
            var reports = new List<AttendanceReportDto>();
            var courses = (await _courseRepo.GetAllAsync()).ToList();
            foreach (var course in courses)
            {
                var courseAtt = await _attRepo.GetByCourseIdAsync(course.Id);
                var studentGroups = courseAtt.GroupBy(a => a.StudentId);
                foreach (var group in studentGroups)
                {
                    var student = await _studentRepo.GetByIdAsync(group.Key);
                    var monthlyGroups = group.GroupBy(a => $"{a.Date.Year}-{a.Date.Month}");
                    foreach (var monthGroup in monthlyGroups)
                    {
                        var parts = monthGroup.Key.Split('-');
                        var date = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), 1);
                        var total = monthGroup.Count();
                        var present = monthGroup.Count(a => a.Status == AttendanceStatus.Present);
                        var absent = monthGroup.Count(a => a.Status == AttendanceStatus.Absent);
                        var late = monthGroup.Count(a => a.Status == AttendanceStatus.Late);
                        var excused = monthGroup.Count(a => a.Status == AttendanceStatus.Excused);

                        reports.Add(new AttendanceReportDto
                        {
                            StudentId = group.Key,
                            StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                            CourseId = course.Id,
                            CourseName = course.Title,
                            Month = date.ToString("MMM yyyy"),
                            Present = present,
                            Absent = absent,
                            Late = late,
                            Excused = excused,
                            Percentage = total > 0 ? Math.Round((double)present / total * 100, 1) : 0
                        });
                    }
                }
            }
            return reports;
        }

        public async Task<AttendanceListDto> GetAttendanceByIdAsync(int id)
        {
            var att = await _attRepo.GetByIdAsync(id);
            if (att == null)
                throw new Exception("Attendance not found.");
            return _mapper.Map<AttendanceListDto>(att);
        }
    }
}
