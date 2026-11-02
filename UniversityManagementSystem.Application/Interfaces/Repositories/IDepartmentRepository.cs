using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetDepartmentWithDetailsAsync(int id);
        Task<IEnumerable<Department>> GetAllDepartmentsWithDetailsAsync();
        Task<Department?> GetByCodeAsync(string code);
    }
}