using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface ISemesterOfferedCourseRepository : IRepository<SemesterOfferedCourse>
    {
        Task<IEnumerable<SemesterOfferedCourse>> GetOpenBySemesterIdAsync(int semesterId);
        Task<IEnumerable<SemesterOfferedCourse>> GetBySemesterIdAsync(int semesterId);
        Task<SemesterOfferedCourse?> GetByIdWithDetailsAsync(int id);
        Task<int> GetRegisteredCountAsync(int offeredCourseId);
        Task<bool> IsStudentRegisteredAsync(int studentId, int offeredCourseId);
        Task<IEnumerable<OfferedCourseSchedule>> GetSchedulesAsync(int offeredCourseId);
    }
}
