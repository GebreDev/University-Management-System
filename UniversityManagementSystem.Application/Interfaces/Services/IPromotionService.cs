using UniversityManagementSystem.Application.DTOs.Promotion;
using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IPromotionService
    {
        Task<PromotionOverviewDto> GetPromotionOverviewAsync(int academicYearId);
        Task PromoteSectionAsync(int gradeId, string sectionName, List<AcademicGrade> allGrades, int academicYearId);
        Task PromoteAllEligibleAsync(int academicYearId, string confirmationCode, List<AcademicGrade> allGrades);
    }
}
