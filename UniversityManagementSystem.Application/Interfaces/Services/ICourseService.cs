using UniversityManagementSystem.Application.DTOs.Course;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseListDto>> GetAllCoursesAsync();
        Task<CourseViewDto> GetCourseByIdAsync(int id);
        Task<CourseViewDto> CreateCourseAsync(CourseCreateDto dto);
        Task UpdateCourseAsync(CourseUpdateDto dto);
        Task DeleteCourseAsync(int id);
        Task<bool> CourseExistsAsync(int id);
        Task<IEnumerable<CourseListDto>> GetCoursesByInstructorAsync(int instructorId);
        Task<IEnumerable<CourseListDto>> GetCoursesByDepartmentAsync(int departmentId);
        Task<bool> IsCourseCodeUniqueAsync(string code, int? excludeId = null);
    }
}