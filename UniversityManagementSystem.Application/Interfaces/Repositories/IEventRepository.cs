using UniversityManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IEventRepository : IRepository<UniversityEvent>
    {
        Task<IEnumerable<UniversityEvent>> GetUpcomingAsync(int count);
        Task<IEnumerable<UniversityEvent>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
