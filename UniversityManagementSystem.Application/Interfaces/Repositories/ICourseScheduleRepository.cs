using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ICourseScheduleRepository : IRepository<CourseSchedule>
    {
        Task<IEnumerable<CourseSchedule>> GetByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<IEnumerable<CourseSchedule>> GetByCourseIdAsync(int courseId);
    }
}
