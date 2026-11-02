using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ICourseMaterialRepository : IRepository<CourseMaterial>
    {
        Task<IEnumerable<CourseMaterial>?> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<CourseMaterial>?> GetByUploaderIdAsync(string uploaderId);
        Task<IEnumerable<CourseMaterial>?> GetByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<CourseMaterial?> GetByIdWithDetailsAsync(int id);
    }
}
