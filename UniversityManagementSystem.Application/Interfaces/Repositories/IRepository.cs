using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllAsync();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int id);
        Microsoft.EntityFrameworkCore.DbContext GetDbContext();
    }
}