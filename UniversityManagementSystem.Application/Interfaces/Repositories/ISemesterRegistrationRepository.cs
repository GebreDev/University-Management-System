using UniversityManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories;

public interface ISemesterRegistrationRepository : IRepository<SemesterRegistration>
{
    new Task<SemesterRegistration?> GetByIdAsync(int id);
    new Task<IEnumerable<SemesterRegistration>> GetAllAsync();
    Task<IEnumerable<SemesterRegistration>> GetByStudentIdAsync(int studentId);
    Task<SemesterRegistration?> GetByStudentAndSemesterAsync(int studentId, int semesterId);
    new Task<SemesterRegistration> AddAsync(SemesterRegistration entity);
    new Task<SemesterRegistration> UpdateAsync(SemesterRegistration entity);
    new Task SaveChangesAsync();
}
