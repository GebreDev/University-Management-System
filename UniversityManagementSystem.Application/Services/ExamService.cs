using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Exams;
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
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IStudentAnswerRepository _studentAnswerRepo;
        private readonly IExamResultRepository _examResultRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ExamService(
            IExamRepository examRepo,
            IQuestionRepository questionRepo,
            IStudentAnswerRepository studentAnswerRepo,
            IExamResultRepository examResultRepo,
            IStudentRepository studentRepo,
            ICourseRepository courseRepo,
            IGradeRepository gradeRepo,
            INotificationService notificationService,
            IMapper mapper)
        {
            _examRepo = examRepo;
            _questionRepo = questionRepo;
            _studentAnswerRepo = studentAnswerRepo;
            _examResultRepo = examResultRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
            _gradeRepo = gradeRepo;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<ExamDto> CreateExamAsync(ExamDto dto)
        {
            var exam = _mapper.Map<Exam>(dto);
            await _examRepo.AddAsync(exam);
            await _examRepo.SaveChangesAsync();
            return _mapper.Map<ExamDto>(exam);
        }

        public async Task<ExamDto> UpdateExamAsync(ExamDto dto)
        {
            var exam = await _examRepo.GetByIdAsync(dto.Id);
            if (exam == null)
                throw new Exception("Exam not found");
            _mapper.Map(dto, exam);
            await _examRepo.UpdateAsync(exam);
            await _examRepo.SaveChangesAsync();
            return _mapper.Map<ExamDto>(exam);
        }

        // ===== FIXED: Safe deletion with all dependencies =====
        public async Task DeleteExamAsync(int examId)
        {
            var exam = await _examRepo.GetByIdAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found");

            // 1. Delete all Grades referencing this exam
            var grades = await _gradeRepo.Find(g => g.ExamId == examId).ToListAsync();
            foreach (var grade in grades)
                _gradeRepo.Delete(grade);
            await _gradeRepo.SaveChangesAsync();

            // 2. Delete all ExamResults
            var results = await _examResultRepo.Find(r => r.ExamId == examId).ToListAsync();
            foreach (var result in results)
                _examResultRepo.Delete(result);
            await _examResultRepo.SaveChangesAsync();

            // 3. Delete all StudentAnswers linked to questions in this exam
            var questions = await _questionRepo.Find(q => q.ExamId == examId).ToListAsync();
            foreach (var q in questions)
            {
                var answers = await _studentAnswerRepo.Find(sa => sa.QuestionId == q.Id).ToListAsync();
                foreach (var ans in answers)
                    _studentAnswerRepo.Delete(ans);
            }
            await _studentAnswerRepo.SaveChangesAsync();

            // 4. Delete all Questions (cascade deletes QuestionOptions)
            foreach (var q in questions)
                _questionRepo.Delete(q);
            await _questionRepo.SaveChangesAsync();

            // 5. Finally delete the exam itself
            _examRepo.Delete(exam);
            await _examRepo.SaveChangesAsync();
        }

        // ===== All other methods remain unchanged =====

        public async Task<ExamDto> GetExamByIdAsync(int examId)
        {
            var exam = await _examRepo.GetByIdAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found");
            return _mapper.Map<ExamDto>(exam);
        }

        public async Task<ExamDto> GetExamWithQuestionsAsync(int examId)
        {
            var exam = await _examRepo.GetWithQuestionsAsync(examId);
            return _mapper.Map<ExamDto>(exam);
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByCourseIdAsync(int courseId)
        {
            var exams = await _examRepo.GetByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByInstructorIdAsync(int instructorId)
        {
            var exams = await _examRepo.GetByInstructorIdAsync(instructorId);
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }

        public async Task<IEnumerable<ExamDto>> GetExamsForStudentAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null)
                throw new Exception("Student not found");
            var enrollments = await _studentRepo.GetEnrollmentsByStudentIdAsync(studentId);
            var courseIds = enrollments.Select(e => e.CourseId).Distinct();
            var allExams = new List<Exam>();
            foreach (var cid in courseIds)
            {
                var courseExams = await _examRepo.GetByCourseIdAsync(cid);
                allExams.AddRange(courseExams.Where(e => e.IsPublished));
            }
            return _mapper.Map<IEnumerable<ExamDto>>(allExams);
        }

        public async Task PublishExamAsync(int examId)
        {
            var exam = await _examRepo.GetByIdAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found");
            exam.IsPublished = true;
            await _examRepo.UpdateAsync(exam);
            await _examRepo.SaveChangesAsync();

            var students = await _studentRepo.GetStudentsByCourseIdAsync(exam.CourseId);
            foreach (var student in students)
            {
                await _notificationService.SendNotificationAsync(
                    student.ApplicationUserId,
                    "New Exam Published",
                    $"Exam '{exam.Title}' is now available. Starts: {exam.StartDateTime:G}, Ends: {exam.EndDateTime:G}",
                    "Exam",
                    exam.Id.ToString()
                );
            }
        }

        public async Task<QuestionDto> AddQuestionAsync(QuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            await _questionRepo.AddAsync(question);
            await _questionRepo.SaveChangesAsync();

            if (dto.Type == ExamType.MCQ && dto.Options != null && dto.Options.Any())
            {
                foreach (var optDto in dto.Options)
                {
                    var option = new QuestionOption
                    {
                        QuestionId = question.Id,
                        OptionText = optDto.OptionText,
                        IsCorrect = optDto.IsCorrect
                    };
                    await _questionRepo.GetDbContext().Set<QuestionOption>().AddAsync(option);
                }
                await _questionRepo.SaveChangesAsync();
            }

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task UpdateQuestionAsync(QuestionDto dto)
        {
            var question = await _questionRepo.GetByIdAsync(dto.Id);
            if (question == null)
                throw new Exception("Question not found");
            _mapper.Map(dto, question);
            await _questionRepo.UpdateAsync(question);

            if (dto.Type == ExamType.MCQ && dto.Options != null)
            {
                var dbContext = _questionRepo.GetDbContext();
                var existingOptions = await dbContext.Set<QuestionOption>()
                    .Where(o => o.QuestionId == dto.Id).ToListAsync();
                dbContext.Set<QuestionOption>().RemoveRange(existingOptions);

                foreach (var optDto in dto.Options)
                {
                    if (!string.IsNullOrWhiteSpace(optDto.OptionText))
                    {
                        await dbContext.Set<QuestionOption>().AddAsync(new QuestionOption
                        {
                            QuestionId = question.Id,
                            OptionText = optDto.OptionText,
                            IsCorrect = optDto.IsCorrect
                        });
                    }
                }
            }

            await _questionRepo.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                throw new Exception("Question not found");
            _questionRepo.Delete(question);
            await _questionRepo.SaveChangesAsync();
        }

        public async Task<QuestionDto?> GetQuestionWithOptionsAsync(int questionId)
        {
            var question = await _questionRepo.GetByIdWithOptionsAsync(questionId);
            if (question == null) return null;
            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<ExamResultDto> GradeExamAsync(int examId, int studentId, Dictionary<int, string> answers)
        {
            var exam = await _examRepo.GetWithQuestionsAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found");
            int score = 0;
            foreach (var q in exam.Questions)
            {
                var studentAnswerText = answers.ContainsKey(q.Id) ? answers[q.Id] : "";
                bool isCorrect = false;
                int? autoScore = null;

                switch (q.Type)
                {
                    case ExamType.MCQ:
                        isCorrect = string.Equals(q.CorrectAnswer, studentAnswerText, StringComparison.OrdinalIgnoreCase);
                        autoScore = isCorrect ? q.Marks : 0;
                        break;

                    case ExamType.TrueFalse:
                        isCorrect = string.Equals(q.CorrectAnswer, studentAnswerText, StringComparison.OrdinalIgnoreCase);
                        autoScore = isCorrect ? q.Marks : 0;
                        break;

                    case ExamType.ShortAnswer:
                        if (!string.IsNullOrWhiteSpace(q.CorrectAnswer))
                        {
                            isCorrect = q.CorrectAnswer.Trim().Equals(studentAnswerText.Trim(), StringComparison.OrdinalIgnoreCase);
                            autoScore = isCorrect ? q.Marks : 0;
                        }
                        break;

                    case ExamType.Essay:
                        isCorrect = false;
                        autoScore = null;
                        break;
                }

                var studentAns = new StudentAnswer
                {
                    StudentId = studentId,
                    QuestionId = q.Id,
                    AnswerText = studentAnswerText,
                    IsCorrect = isCorrect,
                    Score = autoScore
                };
                await _studentAnswerRepo.AddAsync(studentAns);
                if (studentAns.IsCorrect) score += q.Marks;
            }
            var result = new ExamResult
            {
                StudentId = studentId,
                ExamId = examId,
                TotalScore = score,
                Grade = score >= exam.PassingMarks ? "Pass" : "Fail",
                IsPass = score >= exam.PassingMarks
            };
            await _examResultRepo.AddAsync(result);
            await _examResultRepo.SaveChangesAsync();

            var examGrade = new Grade
            {
                StudentId = studentId,
                CourseId = exam.CourseId,
                Type = GradeType.Exam,
                ExamId = examId,
                Score = score,
                MaxScore = exam.TotalMarks,
                LetterGrade = GradeHelper.CalculateLetterGrade(score, exam.TotalMarks),
                Semester = GradeHelper.GetCurrentSemester(),
                CreatedAt = DateTime.UtcNow,
                Status = EntityStatus.Active
            };
            await _gradeRepo.AddAsync(examGrade);
            await _gradeRepo.SaveChangesAsync();

            return _mapper.Map<ExamResultDto>(result);
        }

        public async Task<ExamResultDto> GetResultByIdAsync(int examResultId)
        {
            var result = await _examResultRepo.GetByIdAsync(examResultId);
            if (result == null)
                throw new Exception("Result not found");
            return _mapper.Map<ExamResultDto>(result);
        }

        public async Task<ExamResultDto?> GetResultByStudentAndExamAsync(int studentId, int examId)
        {
            var result = await _examResultRepo.GetByStudentAndExamAsync(studentId, examId);
            if (result == null)
                return null;
            return _mapper.Map<ExamResultDto>(result);
        }

        public async Task<IEnumerable<ExamResultDto>> GetResultsByExamIdAsync(int examId)
        {
            var results = await _examResultRepo.GetByExamIdAsync(examId);
            return _mapper.Map<IEnumerable<ExamResultDto>>(results);
        }

        public async Task SubmitExamAsync(int examId, int studentId, Dictionary<int, string> answers)
        {
            var existing = await _examResultRepo.GetByStudentAndExamAsync(studentId, examId);
            if (existing != null)
                throw new Exception("You have already submitted this exam.");
            await GradeExamAsync(examId, studentId, answers);

            var student = await _studentRepo.GetByIdAsync(studentId);
            var examWithQuestions = await _examRepo.GetWithQuestionsAsync(examId);
            if (student?.ApplicationUserId != null && examWithQuestions != null)
            {
                var questions = examWithQuestions.Questions ?? new List<Question>();
                var hasManualQuestions = questions.Any(q => q.Type == ExamType.Essay || (q.Type == ExamType.ShortAnswer && string.IsNullOrWhiteSpace(q.CorrectAnswer)));
                var message = hasManualQuestions
                    ? $"Your exam '{examWithQuestions.Title}' has been submitted. Auto-graded results are ready; written answers await instructor review."
                    : $"Your exam '{examWithQuestions.Title}' has been submitted and graded. Check your results.";
                await _notificationService.SendNotificationAsync(
                    student.ApplicationUserId,
                    "Exam Submitted",
                    message,
                    "Exam",
                    examWithQuestions.Id.ToString()
                );
            }
        }

        public async Task<IEnumerable<ExamResultDto>> GetSubmissionsForExamAsync(int examId)
        {
            var results = await _examResultRepo.GetByExamIdAsync(examId);
            return _mapper.Map<IEnumerable<ExamResultDto>>(results);
        }

        // ===== Manual Essay Grading =====

        public async Task<IEnumerable<StudentAnswerGradingDto>> GetStudentAnswersForGradingAsync(int examId, int studentId)
        {
            var answers = await _studentAnswerRepo.GetByStudentAndExamAsync(studentId, examId);
            return _mapper.Map<IEnumerable<StudentAnswerGradingDto>>(answers);
        }

        public async Task<ExamResultDto> ManualGradeEssayAsync(int examId, int studentId, Dictionary<int, int> scores)
        {
            var exam = await _examRepo.GetWithQuestionsAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found");
            var answers = await _studentAnswerRepo.GetByStudentAndExamAsync(studentId, examId);
            var result = await _examResultRepo.GetByStudentAndExamAsync(studentId, examId);
            if (result == null)
                throw new Exception("Exam result not found. Student may not have submitted yet.");

            int totalScore = 0;
            foreach (var answer in answers)
            {
                if ((answer.Question.Type == ExamType.Essay || answer.Question.Type == ExamType.ShortAnswer) && scores.ContainsKey(answer.QuestionId))
                {
                    answer.Score = scores[answer.QuestionId];
                    answer.IsCorrect = answer.Score > 0;
                    await _studentAnswerRepo.UpdateAsync(answer);
                }
                if (answer.Question.Type == ExamType.MCQ || answer.Question.Type == ExamType.TrueFalse)
                {
                    if (answer.IsCorrect) totalScore += answer.Question.Marks;
                }
                else
                {
                    totalScore += answer.Score ?? 0;
                }
            }

            result.TotalScore = totalScore;
            result.Grade = totalScore >= exam.PassingMarks ? "Pass" : "Fail";
            result.IsPass = totalScore >= exam.PassingMarks;
            await _examResultRepo.UpdateAsync(result);
            await _examResultRepo.SaveChangesAsync();

            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student?.ApplicationUserId != null)
            {
                await _notificationService.SendNotificationAsync(
                    student.ApplicationUserId,
                    "Exam Fully Graded",
                    $"Your exam '{exam.Title}' has been fully graded. Score: {totalScore}/{exam.TotalMarks} - {result.Grade}",
                    "Exam",
                    exam.Id.ToString()
                );
            }

            return _mapper.Map<ExamResultDto>(result);
        }

        public async Task<ExamResultDto> UpdateManualExamResultAsync(int examResultId, int totalScore, string remarks)
        {
            var result = await _examResultRepo.GetByIdAsync(examResultId);
            if (result == null)
                throw new Exception("Exam result not found");

            var exam = await _examRepo.GetByIdAsync(result.ExamId);
            if (exam == null)
                throw new Exception("Associated exam not found");
            result.TotalScore = totalScore;
            result.Grade = totalScore >= exam.PassingMarks ? "Pass" : "Fail";
            result.IsPass = totalScore >= exam.PassingMarks;
            result.Remarks = remarks;
            await _examResultRepo.UpdateAsync(result);
            await _examResultRepo.SaveChangesAsync();

            var student = await _studentRepo.GetByIdAsync(result.StudentId);
            if (student?.ApplicationUserId != null)
            {
                await _notificationService.SendNotificationAsync(
                    student.ApplicationUserId,
                    "Exam Result Updated",
                    $"Your exam result for '{exam.Title}' has been updated. Score: {totalScore} - {result.Grade}",
                    "Exam",
                    exam.Id.ToString()
                );
            }

            return _mapper.Map<ExamResultDto>(result);
        }
    }
}