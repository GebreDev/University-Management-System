using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IInstructorRepository : IRepository<Instructor>
    {
        Task<Instructor?> GetInstructorWithDetailsAsync(int id);
        Task<IEnumerable<Instructor>> GetAllInstructorsWithDetailsAsync();
        Task<Instructor?> GetByApplicationUserIdAsync(string userId);
    }
}