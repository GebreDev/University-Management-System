using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<Enrollment?> GetEnrollmentWithDetailsAsync(int id);
        Task<IEnumerable<Enrollment>> GetAllEnrollmentsWithDetailsAsync();
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdsAsync(IEnumerable<int> courseIds);
        Task<IEnumerable<Enrollment>> GetPendingEnrollmentsAsync();
        Task<bool> IsStudentEnrolledInCourseAsync(int studentId, int courseId);
    }
}