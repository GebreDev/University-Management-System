using UniversityManagementSystem.Application.DTOs.Grades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IGradeService
    {
        Task<GradeDto> AddGradeAsync(GradeCreateDto dto);
        Task UpdateGradeAsync(GradeUpdateDto dto);
        Task DeleteGradeAsync(int id);
        Task<IEnumerable<GradeDto>> GetGradesByStudentIdAsync(int studentId);
        Task<IEnumerable<GradeDto>> GetGradesByCourseIdAsync(int courseId);
        Task<GpaDto> CalculateGpaAsync(int studentId, int? semester = null);
        Task<GradeReportDto> GetGradeReportAsync(int studentId);
        Task<IEnumerable<GradeDto>> GetGradesByInstructorIdAsync(int instructorId);
    }
}