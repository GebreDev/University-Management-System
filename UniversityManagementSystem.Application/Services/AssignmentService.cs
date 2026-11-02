using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Assignments;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Helpers;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;
        private readonly IAssignmentQuestionRepository _assignmentQuestionRepo;
        private readonly IAssignmentAnswerRepository _assignmentAnswerRepo;
        private readonly IAssignmentResultRepository _assignmentResultRepo;
        private readonly IMapper _mapper;

        public AssignmentService(
            IAssignmentRepository assignmentRepo,
            ISubmissionRepository submissionRepo,
            IStudentRepository studentRepo,
            ICourseRepository courseRepo,
            IGradeRepository gradeRepo,
            IFileService fileService,
            INotificationService notificationService,
            IAssignmentQuestionRepository assignmentQuestionRepo,
            IAssignmentAnswerRepository assignmentAnswerRepo,
            IAssignmentResultRepository assignmentResultRepo,
            IMapper mapper)
        {
            _assignmentRepo = assignmentRepo;
            _submissionRepo = submissionRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
            _gradeRepo = gradeRepo;
            _fileService = fileService;
            _notificationService = notificationService;
            _assignmentQuestionRepo = assignmentQuestionRepo;
            _assignmentAnswerRepo = assignmentAnswerRepo;
            _assignmentResultRepo = assignmentResultRepo;
            _mapper = mapper;
        }

        // ===== Assignments =====
        public async Task<AssignmentDto> CreateAssignmentAsync(AssignmentCreateDto dto, int instructorId)
        {
            var course = await _courseRepo.GetByIdAsync(dto.CourseId);
            if (course == null)
                throw new NotFoundException("Course", dto.CourseId);

            string? filePath = null;
            if (dto.Attachment != null && dto.Attachment.Length > 0)
                filePath = await _fileService.SaveFileAsync(dto.Attachment, "assignments");
            else if (!string.IsNullOrEmpty(dto.FileBase64))
                filePath = await _fileService.SaveFileAsync(dto.FileBase64, "assignments", dto.FileName);

            var assignment = _mapper.Map<Assignment>(dto);
            assignment.InstructorId = instructorId;
            assignment.IsPublished = false;
            assignment.AttachmentPath = filePath;
            await _assignmentRepo.AddAsync(assignment);
            await _assignmentRepo.SaveChangesAsync();
            return _mapper.Map<AssignmentDto>(assignment);
        }

        public async Task UpdateAssignmentAsync(AssignmentUpdateDto dto)
        {
            var assignment = await _assignmentRepo.GetByIdAsync(dto.Id);
            if (assignment == null)
                throw new NotFoundException("Assignment", dto.Id);

            if (dto.Attachment != null && dto.Attachment.Length > 0)
            {
                if (!string.IsNullOrEmpty(assignment.AttachmentPath))
                    _fileService.DeleteFile(assignment.AttachmentPath);
                assignment.AttachmentPath = await _fileService.SaveFileAsync(dto.Attachment, "assignments");
            }
            else if (!string.IsNullOrEmpty(dto.FileBase64))
            {
                if (!string.IsNullOrEmpty(assignment.AttachmentPath))
                    _fileService.DeleteFile(assignment.AttachmentPath);
                assignment.AttachmentPath = await _fileService.SaveFileAsync(dto.FileBase64, "assignments", dto.FileName);
            }
            _mapper.Map(dto, assignment);
            await _assignmentRepo.UpdateAsync(assignment);
            await _assignmentRepo.SaveChangesAsync();
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            var assignment = await _assignmentRepo.GetByIdAsync(id);
            if (assignment == null)
                throw new NotFoundException("Assignment", id);
            if (!string.IsNullOrEmpty(assignment.AttachmentPath))
                _fileService.DeleteFile(assignment.AttachmentPath);
            _assignmentRepo.Delete(assignment);
            await _assignmentRepo.SaveChangesAsync();
        }		public async Task PublishAssignmentAsync(int id)
        {
            var assignment = await _assignmentRepo.GetByIdAsync(id);
            if (assignment == null)
                throw new NotFoundException("Assignment", id);
            assignment.IsPublished = true;
            await _assignmentRepo.UpdateAsync(assignment);
            await _assignmentRepo.SaveChangesAsync();

            // Notify enrolled students about the published assignment
            var students = await _studentRepo.GetStudentsByCourseIdAsync(assignment.CourseId);
            foreach (var student in students)
            {
                await _notificationService.SendNotificationAsync(
                    student.ApplicationUserId,
                    "New Assignment Published",
                    $"Assignment '{assignment.Title}' is now available. Deadline: {assignment.Deadline.ToShortDateString()}",
                    "Assignment",
                    assignment.Id.ToString()
                );
            }
        }

        public async Task<IEnumerable<AssignmentDto>> GetAssignmentsByCourseIdAsync(int courseId)
        {
            var assignments = await _assignmentRepo.GetByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<AssignmentDto>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDto>> GetAssignmentsForStudentAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                throw new NotFoundException("Student", studentId);
            var enrollments = await _studentRepo.GetEnrollmentsByStudentIdAsync(studentId);
            var courseIds = enrollments.Select(e => e.CourseId).Distinct();
            var allAssignments = new List<Assignment>();
            foreach (var cid in courseIds)
                allAssignments.AddRange(await _assignmentRepo.GetByCourseIdAsync(cid));
            // Only show published assignments to students
            var publishedAssignments = allAssignments.Where(a => a.IsPublished);
            return _mapper.Map<IEnumerable<AssignmentDto>>(publishedAssignments);
        }

        public async Task<AssignmentDto> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _assignmentRepo.GetByIdAsync(id);
            if (assignment == null)
                throw new NotFoundException("Assignment", id);
            return _mapper.Map<AssignmentDto>(assignment);
        }

        public async Task<IEnumerable<AssignmentDto>> GetAssignmentsByInstructorIdAsync(int instructorId)
        {
            var assignments = await _assignmentRepo.GetByInstructorIdAsync(instructorId);
            return _mapper.Map<IEnumerable<AssignmentDto>>(assignments);
        }

        // ===== Submissions =====
        public async Task SubmitAssignmentAsync(SubmissionCreateDto dto, int studentId)
        {
            var assignment = await _assignmentRepo.GetByIdAsync(dto.AssignmentId);
            if (assignment == null)
                throw new NotFoundException("Assignment", dto.AssignmentId);

            bool isLate = DateTime.UtcNow > assignment.Deadline;
            if (isLate)
            {
                if (!assignment.AllowLateSubmission)
                    throw new Exception("Submission deadline has passed. Late submissions are not allowed.");
                if (assignment.LateSubmissionPenalty.GetValueOrDefault() >= 100)
                    throw new Exception("Late submission penalty is 100%. You cannot submit after the deadline.");
            }

            var existing = await _submissionRepo.GetByStudentAndAssignmentAsync(studentId, dto.AssignmentId);
            if (existing != null)
            {
                if (!string.IsNullOrEmpty(existing.FilePath))
                    _fileService.DeleteFile(existing.FilePath);
                existing.FilePath = await SaveSubmissionFileAsync(dto);
                existing.SubmittedAt = DateTime.UtcNow;
                await _submissionRepo.UpdateAsync(existing);
                await _submissionRepo.SaveChangesAsync();
                return;
            }

            var submission = new Submission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                SubmittedAt = DateTime.UtcNow,
                IsGraded = false,
                FilePath = await SaveSubmissionFileAsync(dto)
            };
            await _submissionRepo.AddAsync(submission);
            await _submissionRepo.SaveChangesAsync();
        }

        private async Task<string?> SaveSubmissionFileAsync(SubmissionCreateDto dto)
        {
            if (dto.File != null && dto.File.Length > 0)
                return await _fileService.SaveFileAsync(dto.File, "submissions");
            if (!string.IsNullOrWhiteSpace(dto.FileBase64))
                return await _fileService.SaveFileAsync(dto.FileBase64, "submissions", dto.FileName);
            throw new ArgumentException("Please choose a file before submitting the assignment.");
        }

        public async Task GradeSubmissionAsync(SubmissionGradeDto dto)
        {
            var submission = await _submissionRepo.GetByIdAsync(dto.SubmissionId);
            if (submission == null)
                throw new NotFoundException("Submission", dto.SubmissionId);
            submission.Grade = dto.Grade;
            submission.Feedback = dto.Feedback;
            submission.IsGraded = true;
            await _submissionRepo.UpdateAsync(submission);
            await _submissionRepo.SaveChangesAsync();

            var student = await _studentRepo.GetByIdAsync(submission.StudentId);
            var assignment = await _assignmentRepo.GetByIdAsync(submission.AssignmentId);
            if (student != null)
            {
                await _notificationService.SendNotificationAsync(
                    student.ApplicationUserId,
                    "Assignment Graded",
                    $"Your assignment has been graded. Score: {dto.Grade}",
                    "Grade",
                    submission.AssignmentId.ToString()
                );
            }

            // Auto-create Grade record for assignment
            if (assignment != null)
            {
                var grade = new Grade
                {
                    StudentId = submission.StudentId,
                    CourseId = assignment.CourseId,
                    Type = GradeType.Assignment,
                    AssignmentId = assignment.Id,
                    SubmissionId = submission.Id,
                    Score = dto.Grade,
                    MaxScore = assignment.TotalMarks,
                    LetterGrade = GradeHelper.CalculateLetterGrade(dto.Grade, assignment.TotalMarks),
                    CreatedAt = DateTime.UtcNow,
                    Status = EntityStatus.Active
                };
                await _gradeRepo.AddAsync(grade);
                await _gradeRepo.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SubmissionDto>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            var submissions = await _submissionRepo.GetByAssignmentIdAsync(assignmentId);
            return _mapper.Map<IEnumerable<SubmissionDto>>(submissions);
        }

        public async Task<SubmissionDto> GetSubmissionByStudentAndAssignmentAsync(int studentId, int assignmentId)
        {
            var submission = await _submissionRepo.GetByStudentAndAssignmentAsync(studentId, assignmentId);
            return _mapper.Map<SubmissionDto>(submission);
        }

        public async Task<SubmissionDto> GetSubmissionByIdAsync(int submissionId)
        {
            var submission = await _submissionRepo.GetByIdAsync(submissionId);
            if (submission == null)
                throw new NotFoundException("Submission", submissionId);
            return _mapper.Map<SubmissionDto>(submission);
        }

        // ===== Assignment Questions =====

        public async Task<AssignmentQuestionDto> AddAssignmentQuestionAsync(AssignmentQuestionDto dto)
        {
            var question = _mapper.Map<AssignmentQuestion>(dto);
            await _assignmentQuestionRepo.AddAsync(question);
            await _assignmentQuestionRepo.SaveChangesAsync();

            // Manually save MCQ options since AutoMapper ignores them for the AssignmentQuestion mapping
            if (dto.Type == ExamType.MCQ && dto.Options != null && dto.Options.Any())
            {
                foreach (var optDto in dto.Options)
                {
                    var option = new AssignmentQuestionOption
                    {
                        AssignmentQuestionId = question.Id,
                        OptionText = optDto.OptionText,
                        IsCorrect = optDto.IsCorrect
                    };
                    await _assignmentQuestionRepo.GetDbContext().Set<AssignmentQuestionOption>().AddAsync(option);
                }
                await _assignmentQuestionRepo.SaveChangesAsync();
            }

            return _mapper.Map<AssignmentQuestionDto>(question);
        }

        public async Task UpdateAssignmentQuestionAsync(AssignmentQuestionDto dto)
        {
            var question = await _assignmentQuestionRepo.GetByIdAsync(dto.Id);
            if (question == null)
                throw new Exception("Assignment question not found");
            _mapper.Map(dto, question);
            await _assignmentQuestionRepo.UpdateAsync(question);

            // Update MCQ options (batch all changes before SaveChangesAsync)
            if (dto.Type == ExamType.MCQ && dto.Options != null)
            {
                var dbContext = _assignmentQuestionRepo.GetDbContext();
                // Remove existing options
                var existingOptions = await dbContext.Set<AssignmentQuestionOption>()
                    .Where(o => o.AssignmentQuestionId == dto.Id).ToListAsync();
                dbContext.Set<AssignmentQuestionOption>().RemoveRange(existingOptions);

                // Add new options
                foreach (var optDto in dto.Options)
                {
                    if (!string.IsNullOrWhiteSpace(optDto.OptionText))
                    {
                        await dbContext.Set<AssignmentQuestionOption>().AddAsync(new AssignmentQuestionOption
                        {
                            AssignmentQuestionId = question.Id,
                            OptionText = optDto.OptionText,
                            IsCorrect = optDto.IsCorrect
                        });
                    }
                }
            }

            await _assignmentQuestionRepo.SaveChangesAsync();
        }

        public async Task DeleteAssignmentQuestionAsync(int questionId)
        {
            var question = await _assignmentQuestionRepo.GetByIdAsync(questionId);
            if (question == null)
                throw new Exception("Assignment question not found");
            _assignmentQuestionRepo.Delete(question);
            await _assignmentQuestionRepo.SaveChangesAsync();
        }

        public async Task<AssignmentQuestionDto?> GetAssignmentQuestionWithOptionsAsync(int questionId)
        {
            var question = await _assignmentQuestionRepo.GetByIdWithOptionsAsync(questionId);
            if (question == null) return null;
            return _mapper.Map<AssignmentQuestionDto>(question);
        }

        // ===== Assignment Answering & Grading =====

        public async Task<AssignmentResultDto> SubmitAssignmentAnswersAsync(int assignmentId, int studentId, Dictionary<int, string> answers)
        {
            var existing = await _assignmentResultRepo.GetByStudentAndAssignmentAsync(studentId, assignmentId);
            if (existing != null)
                throw new Exception("You have already submitted answers for this assignment.");

            var assignment = await _assignmentRepo.GetByIdAsync(assignmentId);
            if (assignment == null) throw new Exception("Assignment not found");

            // Get questions with options
            var questions = (await _assignmentQuestionRepo.GetByAssignmentIdAsync(assignmentId)).ToList();
            int score = 0;

            foreach (var q in questions)
            {
                var studentAnswer = answers.GetValueOrDefault(q.Id) ?? "";
                bool isCorrect = false;
                int? questionScore = null;

                switch (q.Type)
                {
                    case ExamType.MCQ:
                        // Auto-grade: compare answer text with the correct option text
                        isCorrect = q.CorrectAnswer.Equals(studentAnswer, StringComparison.OrdinalIgnoreCase);
                        questionScore = isCorrect ? q.Marks : 0;
                        break;

                    case ExamType.TrueFalse:
                        // Auto-grade: compare "True" or "False" with the correct answer
                        isCorrect = q.CorrectAnswer.Equals(studentAnswer, StringComparison.OrdinalIgnoreCase);
                        questionScore = isCorrect ? q.Marks : 0;
                        break;

                    case ExamType.ShortAnswer:
                        // Auto-grade: case-insensitive exact match with the correct answer
                        // If no correct answer is set, leave for manual grading
                        if (!string.IsNullOrWhiteSpace(q.CorrectAnswer))
                        {
                            isCorrect = q.CorrectAnswer.Trim().Equals(studentAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
                            questionScore = isCorrect ? q.Marks : 0;
                        }
                        break;

                    case ExamType.Essay:
                        // Essay: always needs manual grading, score stays null
                        isCorrect = false;
                        questionScore = null;
                        break;
                }

                var ans = new AssignmentAnswer
                {
                    StudentId = studentId,
                    AssignmentQuestionId = q.Id,
                    AnswerText = studentAnswer,
                    IsCorrect = isCorrect,
                    Score = questionScore
                };
                await _assignmentAnswerRepo.AddAsync(ans);
                if (ans.Score.HasValue) score += ans.Score.Value;
            }

            var result = new AssignmentResult
            {
                StudentId = studentId,
                AssignmentId = assignmentId,
                TotalScore = score,
                Grade = score >= (assignment.TotalMarks / 2) ? "Pass" : "Fail",
                IsPass = score >= (assignment.TotalMarks / 2)
            };
            await _assignmentResultRepo.AddAsync(result);
            await _assignmentResultRepo.SaveChangesAsync();

            // Auto-create Grade record
            var grade = new Grade
            {
                StudentId = studentId,
                CourseId = assignment.CourseId,
                Type = GradeType.Assignment,
                AssignmentId = assignmentId,
                Score = score,
                MaxScore = assignment.TotalMarks,
                LetterGrade = GradeHelper.CalculateLetterGrade(score, assignment.TotalMarks),
                Semester = GradeHelper.GetCurrentSemester(),
                CreatedAt = DateTime.UtcNow,
                Status = EntityStatus.Active
            };
            await _gradeRepo.AddAsync(grade);
            await _gradeRepo.SaveChangesAsync();

            return _mapper.Map<AssignmentResultDto>(result);
        }

        public async Task<AssignmentResultDto> GetAssignmentResultByIdAsync(int resultId)
        {
            var result = await _assignmentResultRepo.GetByIdAsync(resultId);
            if (result == null) throw new Exception("Result not found");
            return _mapper.Map<AssignmentResultDto>(result);
        }

        public async Task<AssignmentResultDto?> GetAssignmentResultByStudentAndAssignmentAsync(int studentId, int assignmentId)
        {
            var result = await _assignmentResultRepo.GetByStudentAndAssignmentAsync(studentId, assignmentId);
            if (result == null) return null;
            return _mapper.Map<AssignmentResultDto>(result);
        }

        public async Task<IEnumerable<AssignmentResultDto>> GetAssignmentResultsByAssignmentIdAsync(int assignmentId)
        {
            var results = await _assignmentResultRepo.GetByAssignmentIdAsync(assignmentId);
            return _mapper.Map<IEnumerable<AssignmentResultDto>>(results);
        }

        public async Task<IEnumerable<AssignmentAnswerGradingDto>> GetStudentAssignmentAnswersForGradingAsync(int assignmentId, int studentId)
        {
            var answers = await _assignmentAnswerRepo.GetByStudentAndAssignmentAsync(studentId, assignmentId);
            return _mapper.Map<IEnumerable<AssignmentAnswerGradingDto>>(answers);
        }

        public async Task<AssignmentResultDto> ManualGradeAssignmentEssayAsync(int assignmentId, int studentId, Dictionary<int, int> scores)
        {
            var assignment = await _assignmentRepo.GetByIdAsync(assignmentId);
            if (assignment == null) throw new Exception("Assignment not found");

            var questions = (await _assignmentQuestionRepo.GetByAssignmentIdAsync(assignmentId)).ToList();
            var answers = (await _assignmentAnswerRepo.GetByStudentAndAssignmentAsync(studentId, assignmentId)).ToList();
            var result = await _assignmentResultRepo.GetByStudentAndAssignmentAsync(studentId, assignmentId);
            if (result == null) throw new Exception("Result not found. Student may not have submitted yet.");

            int totalScore = 0;
            foreach (var answer in answers)
            {
                var q = questions.FirstOrDefault(x => x.Id == answer.AssignmentQuestionId);
                if (q == null) continue;

                // For manually graded types (Essay, ShortAnswer without auto-grade), apply the score from the form
                if ((q.Type == ExamType.Essay || q.Type == ExamType.ShortAnswer) && scores.ContainsKey(q.Id))
                {
                    answer.Score = scores[q.Id];
                    answer.IsCorrect = answer.Score > 0;
                    await _assignmentAnswerRepo.UpdateAsync(answer);
                }

                // For auto-graded types (MCQ, TrueFalse), use their existing auto-graded score
                totalScore += (q.Type == ExamType.MCQ || q.Type == ExamType.TrueFalse)
                    ? (answer.IsCorrect ? q.Marks : 0)
                    : (answer.Score ?? 0);
            }

            result.TotalScore = totalScore;
            result.Grade = totalScore >= (assignment.TotalMarks / 2) ? "Pass" : "Fail";
            result.IsPass = totalScore >= (assignment.TotalMarks / 2);
            await _assignmentResultRepo.UpdateAsync(result);
            await _assignmentResultRepo.SaveChangesAsync();

            return _mapper.Map<AssignmentResultDto>(result);
        }

        public async Task<AssignmentResultDto> UpdateAssignmentResultAsync(int resultId, int totalScore, string remarks)
        {
            var result = await _assignmentResultRepo.GetByIdAsync(resultId);
            if (result == null) throw new Exception("Result not found");

            var assignment = await _assignmentRepo.GetByIdAsync(result.AssignmentId);
            if (assignment == null) throw new Exception("Assignment not found");

            result.TotalScore = totalScore;
            result.Grade = totalScore >= (assignment.TotalMarks / 2) ? "Pass" : "Fail";
            result.IsPass = totalScore >= (assignment.TotalMarks / 2);            result.Remarks = remarks;
            await _assignmentResultRepo.UpdateAsync(result);
            await _assignmentResultRepo.SaveChangesAsync();

            return _mapper.Map<AssignmentResultDto>(result);
        }
    }
}
