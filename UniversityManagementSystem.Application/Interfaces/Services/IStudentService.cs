using UniversityManagementSystem.Application.DTOs.Student;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentListDto>> GetAllStudentsAsync();
        Task<StudentViewDto> GetStudentByIdAsync(int id);
        Task<StudentViewDto> CreateStudentAsync(StudentCreateDto dto);
        Task UpdateStudentAsync(StudentUpdateDto dto);
        Task DeleteStudentAsync(int id);
        Task<bool> StudentExistsAsync(int id);
        Task<StudentViewDto?> GetStudentByUserIdAsync(string userId);
        Task<IEnumerable<StudentListDto>> GetStudentsByCourseAsync(int courseId);
    }
}