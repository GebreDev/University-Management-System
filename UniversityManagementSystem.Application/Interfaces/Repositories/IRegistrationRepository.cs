using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IRegistrationRepository : IRepository<Registration>
    {
        Task<Registration?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Registration>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Registration>> GetBySemesterIdAsync(int semesterId);
        Task<IEnumerable<Registration>> GetPendingBySemesterIdAsync(int semesterId);
        Task<int> GetPendingRegistrationCountAsync();
        Task<IEnumerable<RegistrationItem>> GetApprovedItemsByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<IEnumerable<RegistrationItem>> GetApprovedItemsByStudentAsync(int studentId);
        Task<int> GetApprovedCourseCountAsync(int studentId, int semesterId);
        Task AddStatusHistoryAsync(RegistrationStatusHistory history);
        Task<Registration?> GetByStudentAndSemesterAsync(int studentId, int semesterId);
    }
}
