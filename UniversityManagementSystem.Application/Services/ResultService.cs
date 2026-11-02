using UniversityManagementSystem.Application.DTOs.Results;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Helpers;
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
    public class ResultService : IResultService
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ICourseAssessmentRepository _assessmentRepo;
        private readonly ICourseResultRepository _resultRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly INotificationService _notificationService;

        public ResultService(
            ICourseRepository courseRepo,
            IEnrollmentRepository enrollmentRepo,
            ICourseAssessmentRepository assessmentRepo,
            ICourseResultRepository resultRepo,
            IStudentRepository studentRepo,
            IDepartmentRepository departmentRepo,
            INotificationService notificationService)
        {
            _courseRepo = courseRepo;
            _enrollmentRepo = enrollmentRepo;
            _assessmentRepo = assessmentRepo;
            _resultRepo = resultRepo;
            _studentRepo = studentRepo;
            _departmentRepo = departmentRepo;
            _notificationService = notificationService;
        }

        // ================= Instructor: course list =================

        public async Task<IEnumerable<InstructorCourseResultDto>> GetInstructorCourseResultsAsync(int instructorId)
        {
            var courses = (await _courseRepo.GetCoursesByInstructorIdAsync(instructorId)).ToList();
            if (!courses.Any()) return new List<InstructorCourseResultDto>();

            var courseIds = courses.Select(c => c.Id).ToList();
            var assessments = (await _assessmentRepo.GetByCourseIdsWithMarksAsync(courseIds)).ToList();
            var published = (await _resultRepo.GetByCourseIdsAsync(courseIds)).Where(r => r.IsPublished).ToList();
            var enrollments = (await _enrollmentRepo.GetEnrollmentsByCourseIdsAsync(courseIds)).ToList();
            var approvedCountByCourse = enrollments
                .Where(e => e.Status == EnrollmentStatus.Approved)
                .GroupBy(e => e.CourseId)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new List<InstructorCourseResultDto>();
            foreach (var course in courses)
            {
                var courseAssessments = assessments.Where(a => a.CourseId == course.Id).ToList();
                var courseResults = published.Where(r => r.CourseId == course.Id).ToList();

                // Average total mark across students that have at least one mark
                var gradedStudentIds = courseAssessments
                    .SelectMany(a => a.Marks.Select(m => m.StudentId))
                    .Distinct()
                    .ToList();

                var averages = gradedStudentIds
                    .Select(sid => ComputeStudentTotal(courseAssessments, sid))
                    .Where(t => t > 0)
                    .ToList();

                result.Add(new InstructorCourseResultDto
                {
                    CourseId = course.Id,
                    CourseTitle = course.Title,
                    CourseCode = course.Code,
                    Credits = course.Credits,
                    Ects = GradeHelper.EctsForCredits(course.Credits),
                    EnrolledCount = approvedCountByCourse.TryGetValue(course.Id, out var enrolled)
                        ? enrolled
                        : 0,
                    AverageMark = averages.Any() ? Math.Round(averages.Average(), 2) : 0,
                    GradedCount = averages.Count,
                    HasDraft = courseAssessments.Any(a => a.Marks.Any()),
                    IsPublished = courseResults.Any(),
                    PublishedAt = courseResults.Any() ? courseResults.Max(r => r.PublishedAt) : null
                });
            }

            return result;
        }

        // ================= Instructor: assessment board =================

        public async Task<CourseAssessmentBoardDto> GetCourseAssessmentBoardAsync(int courseId)
        {
            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null) throw new NotFoundException("Course", courseId);

            var assessments = (await _assessmentRepo.GetByCourseIdWithMarksAsync(courseId)).ToList();
            var students = await GetApprovedStudentsAsync(courseId);
            var published = (await _resultRepo.GetByCourseIdAsync(courseId)).Where(r => r.IsPublished).ToList();

            return new CourseAssessmentBoardDto
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                CourseCode = course.Code,
                Credits = course.Credits,
                Ects = GradeHelper.EctsForCredits(course.Credits),
                IsPublished = published.Any(),
                PublishedAt = published.Any() ? published.Max(r => r.PublishedAt) : null,
                Assessments = assessments
                    .OrderBy(a => a.CreatedAt)
                    .Select(a => new CourseAssessmentDto { Id = a.Id, Name = a.Name, Weight = a.Weight })
                    .ToList(),
                Students = students.Select(s =>
                {
                    var total = ComputeStudentTotal(assessments, s.Id);
                    return new AssessmentBoardStudentDto
                    {
                        StudentId = s.Id,
                        StudentName = $"{s.FirstName} {s.LastName}",
                        StudentNumber = s.StudentId,
                        Marks = assessments.Select(a => new AssessmentMarkDto
                        {
                            AssessmentId = a.Id,
                            Score = a.Marks.FirstOrDefault(m => m.StudentId == s.Id)?.Score
                        }).ToList(),
                        TotalMark = total,
                        LetterGrade = total > 0 ? GradeHelper.ComputeGrade(total).Letter : "—"
                    };
                }).ToList()
            };
        }

        // ================= Instructor: save draft =================

        public async Task SaveDraftAsync(int courseId, SaveCourseAssessmentDto dto)
        {
            await ReplaceAssessmentsAndMarksAsync(courseId, dto);
        }

        // ================= Instructor: publish (send) =================

        public async Task<int> PublishResultsAsync(int courseId, SaveCourseAssessmentDto dto)
        {
            var course = await ReplaceAssessmentsAndMarksAsync(courseId, dto);
            var semester = GradeHelper.GetCurrentSemester();

            var assessments = (await _assessmentRepo.GetByCourseIdWithMarksAsync(courseId)).ToList();
            var students = await GetApprovedStudentsAsync(courseId);
            var existingResults = (await _resultRepo.GetByCourseIdAsync(courseId)).ToList();

            var now = DateTime.UtcNow;
            var publishedCount = 0;

            foreach (var student in students)
            {
                var total = ComputeStudentTotal(assessments, student.Id);
                if (total <= 0) continue; // only students with entered marks get a result

                var (letter, point) = GradeHelper.ComputeGrade(total);
                var result = existingResults.FirstOrDefault(r => r.StudentId == student.Id && r.Semester == semester);
                if (result == null)
                {
                    result = new CourseResult
                    {
                        CourseId = courseId,
                        StudentId = student.Id,
                        Semester = semester,
                        TotalMark = total,
                        LetterGrade = letter,
                        GradePoint = point,
                        Remarks = string.Empty,
                        IsPublished = true,
                        PublishedAt = now
                    };
                    await _resultRepo.AddAsync(result);
                }
                else
                {
                    result.Semester = semester;
                    result.TotalMark = total;
                    result.LetterGrade = letter;
                    result.GradePoint = point;
                    result.IsPublished = true;
                    result.PublishedAt = now;
                    await _resultRepo.UpdateAsync(result);
                }

                // Notify the student that their result is ready (best effort)
                try
                {
                    await _notificationService.SendNotificationAsync(
                        student.ApplicationUserId,
                        "Results Published",
                        $"Your result for {course.Title} ({course.Code}) has been published: {letter}.",
                        "Result",
                        courseId.ToString(),
                        "/Student/MyGrades");
                }
                catch
                {
                    // Notification failures must never block result publication.
                }

                publishedCount++;
            }

            await _resultRepo.SaveChangesAsync();
            return publishedCount;
        }

        // ================= Student: transcript =================

        public async Task<StudentTranscriptDto> GetStudentTranscriptAsync(int studentId, int? semester = null)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null) throw new NotFoundException("Student", studentId);

            var results = (await _resultRepo.GetPublishedByStudentIdAsync(studentId)).ToList();
            var courseIds = results.Select(r => r.CourseId).Distinct().ToList();
            var courses = courseIds.Any()
                ? await _courseRepo.Find(c => courseIds.Contains(c.Id)).ToListAsync()
                : new List<Course>();
            var courseMap = courses.ToDictionary(c => c.Id);

            var department = await _departmentRepo.GetByIdAsync(student.DepartmentId);

            var filtered = semester.HasValue
                ? results.Where(r => r.Semester == semester.Value).ToList()
                : results;

            var latestSemester = results.Where(r => r.Semester.HasValue).Select(r => r.Semester!.Value)
                .DefaultIfEmpty(GradeHelper.GetCurrentSemester()).Max();
            var headerSemester = semester ?? latestSemester;

            // Semester codes are not sequential across academic years (Fall 2025 = 20251,
            // Spring 2026 = 20262), so derive the year number from the year part only.
            var enrollmentYear = SemesterForDate(student.EnrollmentDate) / 10;
            var yearNumber = (headerSemester / 10) - enrollmentYear + 1;
            if (yearNumber < 1) yearNumber = 1;

            var rows = filtered
                .Where(r => courseMap.ContainsKey(r.CourseId))
                .Select(r => new StudentTranscriptRowDto
                {
                    CourseId = r.CourseId,
                    CourseTitle = courseMap[r.CourseId].Title,
                    CourseCode = courseMap[r.CourseId].Code,
                    Credits = courseMap[r.CourseId].Credits,
                    Ects = GradeHelper.EctsForCredits(courseMap[r.CourseId].Credits),
                    TotalMark = r.TotalMark,
                    LetterGrade = r.LetterGrade,
                    GradePoint = r.GradePoint
                })
                .OrderBy(r => r.CourseTitle)
                .ToList();

            var semesterRows = filtered.Where(r => courseMap.ContainsKey(r.CourseId)).ToList();
            var sgp = semesterRows.Sum(r => r.GradePoint * GradeHelper.EctsForCredits(courseMap[r.CourseId].Credits));
            var sgpEcts = semesterRows.Sum(r => GradeHelper.EctsForCredits(courseMap[r.CourseId].Credits));

            var allRows = results.Where(r => courseMap.ContainsKey(r.CourseId)).ToList();
            var cgp = allRows.Sum(r => r.GradePoint * GradeHelper.EctsForCredits(courseMap[r.CourseId].Credits));
            var cgpEcts = allRows.Sum(r => GradeHelper.EctsForCredits(courseMap[r.CourseId].Credits));

            var sgpa = sgpEcts > 0 ? Math.Round(sgp / sgpEcts, 2) : 0;
            var cgpa = cgpEcts > 0 ? Math.Round(cgp / cgpEcts, 2) : 0;

            string status = cgpa >= 2.0m ? "Promoted"
                : cgpa >= 1.5m ? "Academic Probation"
                : "At Risk";

            // Only offer semesters where the student actually has published results.
            var availableSemesters = results.Where(r => r.Semester.HasValue)
                .Select(r => r.Semester!.Value)
                .Distinct()
                .OrderByDescending(c => c)
                .ToList();

            return new StudentTranscriptDto
            {
                StudentName = $"{student.FirstName} {student.LastName}",
                StudentNumber = student.StudentId,
                DepartmentName = department?.Name ?? string.Empty,
                AcademicYear = GradeHelper.GetAcademicYearLabel(headerSemester),
                SemesterLabel = headerSemester % 10 == 1 ? "Semester One" : "Semester Two",
                YearLabel = YearLabel(yearNumber),
                SelectedSemester = semester,
                AvailableSemesters = availableSemesters,
                Sgp = Math.Round(sgp, 2),
                Cgp = Math.Round(cgp, 2),
                Sgpa = sgpa,
                Cgpa = cgpa,
                AcademicStatus = status,
                Rows = rows
            };
        }

        // ================= Student: result breakdown =================

        public async Task<StudentCourseResultDetailDto?> GetStudentCourseResultDetailAsync(int studentId, int courseId)
        {
            var result = await _resultRepo.GetByStudentAndCourseAsync(studentId, courseId);
            if (result == null || !result.IsPublished) return null;

            var course = await _courseRepo.GetByIdAsync(courseId);
            var assessments = (await _assessmentRepo.GetByCourseIdWithMarksAsync(courseId)).ToList();

            return new StudentCourseResultDetailDto
            {
                CourseId = courseId,
                CourseTitle = course?.Title ?? string.Empty,
                CourseCode = course?.Code ?? string.Empty,
                TotalMark = result.TotalMark,
                LetterGrade = result.LetterGrade,
                GradePoint = result.GradePoint,
                Assessments = assessments
                    .OrderBy(a => a.CreatedAt)
                    .Select(a => new CourseAssessmentResultDto
                    {
                        Name = a.Name,
                        Weight = a.Weight,
                        Score = a.Marks.FirstOrDefault(m => m.StudentId == studentId)?.Score ?? 0
                    })
                    .ToList()
            };
        }

        // ================= Internals =================

        private async Task<Course> ReplaceAssessmentsAndMarksAsync(int courseId, SaveCourseAssessmentDto dto)
        {
            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null) throw new NotFoundException("Course", courseId);

            var inputs = (dto.Assessments ?? new List<CourseAssessmentInputDto>())
                .Where(a => !string.IsNullOrWhiteSpace(a.Name) && a.Weight > 0)
                .ToList();
            if (!inputs.Any())
                throw new InvalidOperationException("Add at least one assessment column with a name and weight.");

            // --- Validate everything BEFORE touching the database ---
            foreach (var input in inputs)
            {
                if (input.Weight > 100)
                    throw new InvalidOperationException($"The weight for '{input.Name.Trim()}' cannot exceed 100%.");
            }

            var weightByKey = inputs.ToDictionary(a => a.Key ?? string.Empty, a => a.Weight);
            foreach (var mark in dto.Marks ?? new List<AssessmentMarkInputDto>())
            {
                if (!weightByKey.TryGetValue(mark.AssessmentKey ?? string.Empty, out var max))
                    continue; // stale cell (column removed) - ignore
                if (mark.Score < 0)
                    throw new InvalidOperationException("Marks cannot be negative.");
                if (mark.Score > max)
                    throw new InvalidOperationException($"A mark of {mark.Score} exceeds the maximum ({max}) for an assessment column.");
            }

            // Only marks for students actually enrolled (and approved) in the course are accepted.
            var approvedStudentIds = (await GetApprovedStudentsAsync(courseId)).Select(s => s.Id).ToHashSet();
            foreach (var mark in dto.Marks ?? new List<AssessmentMarkInputDto>())
            {
                if (!weightByKey.ContainsKey(mark.AssessmentKey ?? string.Empty)) continue;
                if (!approvedStudentIds.Contains(mark.StudentId))
                    throw new InvalidOperationException($"Student ID {mark.StudentId} is not enrolled in this course.");
            }

            // --- Replace all assessment columns for the course (marks cascade) ---
            await _assessmentRepo.DeleteByCourseIdAsync(courseId);
            await _assessmentRepo.SaveChangesAsync();

            var semester = GradeHelper.GetCurrentSemester();
            var entityByKey = new Dictionary<string, CourseAssessment>();
            foreach (var input in inputs)
            {
                var entity = new CourseAssessment
                {
                    CourseId = courseId,
                    Name = input.Name.Trim(),
                    Weight = input.Weight,
                    Semester = semester,
                    CreatedAt = DateTime.UtcNow
                };
                await _assessmentRepo.AddAsync(entity);
                entityByKey[input.Key ?? string.Empty] = entity;
            }
            await _assessmentRepo.SaveChangesAsync();

            // --- Add marks through the tracked assessment entities (ids are now assigned) ---
            // Empty cells are never sent by the client; a stored 0 is a legitimate zero mark.
            foreach (var mark in dto.Marks ?? new List<AssessmentMarkInputDto>())
            {
                if (!entityByKey.TryGetValue(mark.AssessmentKey ?? string.Empty, out var assessment))
                    continue;

                assessment.Marks.Add(new AssessmentMark
                {
                    CourseAssessmentId = assessment.Id,
                    StudentId = mark.StudentId,
                    Score = mark.Score,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _assessmentRepo.SaveChangesAsync();

            return course;
        }

        private async Task<List<Student>> GetApprovedStudentsAsync(int courseId)
        {
            var enrollments = (await _enrollmentRepo.GetEnrollmentsByCourseIdAsync(courseId))
                .Where(e => e.Status == EnrollmentStatus.Approved)
                .ToList();
            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            if (!studentIds.Any()) return new List<Student>();

            return await _studentRepo.Find(s => studentIds.Contains(s.Id)).ToListAsync();
        }

        /// <summary>
        /// Total mark out of 100 for a student across the given assessments.
        /// Partial data is handled by normalising against the weights of the
        /// assessments the student actually has marks for.
        /// </summary>
        private static decimal ComputeStudentTotal(List<CourseAssessment> assessments, int studentId)
        {
            var scored = assessments
                .Where(a => a.Marks.Any(m => m.StudentId == studentId))
                .ToList();
            if (!scored.Any()) return 0;

            var scoreSum = scored.Sum(a => a.Marks.Where(m => m.StudentId == studentId).Sum(m => m.Score));
            var weightSum = scored.Sum(a => a.Weight);
            if (weightSum <= 0) return 0;

            return Math.Round(scoreSum / weightSum * 100m, 2);
        }

        private static int SemesterForDate(DateTime date)
        {
            var month = date.Month;
            if (month >= 9 || month <= 2)
                return month >= 9 ? date.Year * 10 + 1 : (date.Year - 1) * 10 + 1;
            return date.Year * 10 + 2;
        }

        private static string YearLabel(int yearNumber)
        {
            string[] names = { "Year I", "Year II", "Year III", "Year IV", "Year V", "Year VI" };
            if (yearNumber >= 1 && yearNumber <= names.Length) return names[yearNumber - 1];
            return $"Year {yearNumber}";
        }
    }
}
