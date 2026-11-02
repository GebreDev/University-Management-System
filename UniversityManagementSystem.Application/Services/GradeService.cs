using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Grades;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Helpers;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IExamRepository _examRepo;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly IMapper _mapper;

        public GradeService(
            IGradeRepository gradeRepo,
            IStudentRepository studentRepo,
            ICourseRepository courseRepo,
            IExamRepository examRepo,
            IAssignmentRepository assignmentRepo,
            ISubmissionRepository submissionRepo,
            IInstructorRepository instructorRepo,
            IMapper mapper)
        {
            _gradeRepo = gradeRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
            _examRepo = examRepo;
            _assignmentRepo = assignmentRepo;
            _submissionRepo = submissionRepo;
            _instructorRepo = instructorRepo;
            _mapper = mapper;
        }

        public async Task<GradeDto> AddGradeAsync(GradeCreateDto dto)
        {
            var student = await _studentRepo.GetByIdAsync(dto.StudentId);
            if (student == null)
                throw new NotFoundException("Student", dto.StudentId);

            var course = await _courseRepo.GetByIdAsync(dto.CourseId);
            if (course == null)
                throw new NotFoundException("Course", dto.CourseId);

            var grade = _mapper.Map<Grade>(dto);
            grade.LetterGrade = GradeHelper.CalculateLetterGrade(grade.Score, grade.MaxScore);
            grade.Semester = GradeHelper.GetCurrentSemester();
            await _gradeRepo.AddAsync(grade);
            await _gradeRepo.SaveChangesAsync();
            return _mapper.Map<GradeDto>(grade);
        }

        public async Task UpdateGradeAsync(GradeUpdateDto dto)
        {
            var grade = await _gradeRepo.GetByIdAsync(dto.Id);
            if (grade == null)
                throw new NotFoundException("Grade", dto.Id);
            _mapper.Map(dto, grade);
            grade.LetterGrade = GradeHelper.CalculateLetterGrade(grade.Score, grade.MaxScore);
            await _gradeRepo.UpdateAsync(grade);
            await _gradeRepo.SaveChangesAsync();
        }

        public async Task DeleteGradeAsync(int id)
        {
            var grade = await _gradeRepo.GetByIdAsync(id);
            if (grade == null)
                throw new NotFoundException("Grade", id);
            _gradeRepo.Delete(grade);
            await _gradeRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<GradeDto>> GetGradesByStudentIdAsync(int studentId)
        {
            var grades = await _gradeRepo.GetByStudentIdAsync(studentId);
            return _mapper.Map<IEnumerable<GradeDto>>(grades);
        }

        public async Task<IEnumerable<GradeDto>> GetGradesByCourseIdAsync(int courseId)
        {
            var grades = await _gradeRepo.GetByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<GradeDto>>(grades);
        }

        public async Task<IEnumerable<GradeDto>> GetGradesByInstructorIdAsync(int instructorId)
        {
            // Get all courses taught by this instructor
            var courses = await _courseRepo.GetByInstructorIdAsync(instructorId);
            var courseIds = courses.Select(c => c.Id);
            // Get grades for those courses
            var grades = await _gradeRepo.GetByCourseIdsAsync(courseIds);
            return _mapper.Map<IEnumerable<GradeDto>>(grades);
        }        public async Task<GpaDto> CalculateGpaAsync(int studentId, int? semester = null)
        {
            var grades = await _gradeRepo.GetByStudentIdAsync(studentId);
            if (semester.HasValue)
                grades = grades.Where(g => g.Semester == semester.Value);

            if (!grades.Any())
                return new GpaDto { StudentId = studentId, Gpa = 0, TotalCredits = 0 };

            decimal totalPoints = 0m;
            int totalCredits = 0;
            foreach (var g in grades)
            {
                var course = await _courseRepo.GetByIdAsync(g.CourseId);
                if (course == null) continue;
                var gradePoint = GradeHelper.ComputeGrade(g.Score).Point;
                totalPoints += gradePoint * course.Credits;
                totalCredits += course.Credits;
            }

            var gpa = totalCredits > 0 ? totalPoints / totalCredits : 0m;
            return new GpaDto
            {
                StudentId = studentId,
                Gpa = (double)Math.Round(gpa, 2),
                TotalCredits = totalCredits
            };
        }

        public async Task<GradeReportDto> GetGradeReportAsync(int studentId)
        {
            var grades = await GetGradesByStudentIdAsync(studentId);
            var gpa = await CalculateGpaAsync(studentId);
            return new GradeReportDto
            {
                StudentId = studentId,
                Grades = grades.ToList(),
                Gpa = gpa
            };
        }


    }
}