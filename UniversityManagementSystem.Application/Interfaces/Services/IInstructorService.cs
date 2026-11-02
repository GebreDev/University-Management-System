using UniversityManagementSystem.Application.DTOs.Instructor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorListDto>> GetAllInstructorsAsync();
        Task<InstructorViewDto> GetInstructorByIdAsync(int id);
        Task<InstructorViewDto> CreateInstructorAsync(InstructorCreateDto dto);
        Task UpdateInstructorAsync(InstructorUpdateDto dto);
        Task DeleteInstructorAsync(int id);
        Task<bool> InstructorExistsAsync(int id);
        Task<InstructorViewDto?> GetInstructorByUserIdAsync(string userId);
    }
}