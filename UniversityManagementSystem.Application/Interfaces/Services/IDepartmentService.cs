using UniversityManagementSystem.Application.DTOs.Department;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync();
        Task<DepartmentViewDto> GetDepartmentByIdAsync(int id);
        Task<DepartmentViewDto> CreateDepartmentAsync(DepartmentCreateDto dto);
        Task UpdateDepartmentAsync(DepartmentUpdateDto dto);
        Task DeleteDepartmentAsync(int id);
        Task<bool> DepartmentExistsAsync(int id);
        Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null);
    }
}