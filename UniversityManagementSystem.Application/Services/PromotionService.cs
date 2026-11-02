using UniversityManagementSystem.Application.DTOs.Promotion;
using UniversityManagementSystem.Application.Exceptions;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAuditLogService _auditLogService;

        public PromotionService(
            IStudentRepository studentRepo,
            IAuditLogService auditLogService)
        {
            _studentRepo = studentRepo;
            _auditLogService = auditLogService;
        }

        public async Task<PromotionOverviewDto> GetPromotionOverviewAsync(int academicYearId)
        {
            // Note: This method is designed to be called with pre-loaded data.
            // The controller is responsible for loading AcademicGrades with Sections.
            throw new NotImplementedException("Use the controller's direct data loading for overview.");
        }

        public async Task PromoteSectionAsync(
            int gradeId,
            string sectionName,
            List<AcademicGrade> allGrades,
            int academicYearId)
        {
            var grade = allGrades.FirstOrDefault(g => g.Id == gradeId);
            if (grade == null)
                throw new NotFoundException(nameof(AcademicGrade), gradeId);

            var section = grade.Sections.FirstOrDefault(s => s.Name == sectionName);
            if (section == null)
                throw new NotFoundException(nameof(AcademicSection), 0);

            var students = await _studentRepo.GetBySectionAsync(section.Id);
            var studentList = students.ToList();

            if (!studentList.Any())
                throw new CustomException($"No students found in {grade.Name} - {section.Name}.");

            var nextGrade = allGrades.FirstOrDefault(g => g.PromotionOrder == grade.PromotionOrder + 1);
            if (nextGrade == null)
                throw new CustomException(
                    $"No higher year available for promotion. {grade.Name} is already the highest year.");

            var nextSection = nextGrade.Sections.FirstOrDefault(s => s.Name == section.Name);

            // Create next section if it doesn't exist (done by caller after this method returns)

            foreach (var student in studentList)
            {
                student.AcademicGradeId = nextGrade.Id;
                student.AcademicSectionId = nextSection != null ? nextSection.Id : 0;
            }
        }

        public async Task PromoteAllEligibleAsync(
            int academicYearId,
            string confirmationCode,
            List<AcademicGrade> allGrades)
        {
            if (!string.Equals(confirmationCode, "PROMOTE ALL", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Confirmation code does not match.");
            }

            int promotedCount = 0;
            int totalPromoted = 0;
            var maxPromotionOrder = allGrades.Max(g => g.PromotionOrder);
            var gradesSorted = allGrades.OrderBy(g => g.PromotionOrder).ToList();

            foreach (var grade in gradesSorted)
            {
                var isGraduating = grade.PromotionOrder == maxPromotionOrder;
                if (isGraduating)
                    continue;

                foreach (var section in grade.Sections)
                {
                    var students = await _studentRepo.GetBySectionAsync(section.Id);
                    var studentList = students.ToList();

                    if (!studentList.Any())
                        continue;

                    var nextGrade = allGrades.FirstOrDefault(g => g.PromotionOrder == grade.PromotionOrder + 1);
                    if (nextGrade == null)
                        continue;

                    var nextSection = nextGrade.Sections.FirstOrDefault(s => s.Name == section.Name);

                    foreach (var student in studentList)
                    {
                        student.AcademicGradeId = nextGrade.Id;
                        student.AcademicSectionId = nextSection?.Id ?? student.AcademicSectionId;
                    }

                    promotedCount++;
                    totalPromoted += studentList.Count;
                }
            }
        }
    }
}
