using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
	protected readonly ApplicationDbContext _context;

	protected readonly DbSet<T> _dbSet;

	public GenericRepository(ApplicationDbContext context)
	{
		_context = context;
		_dbSet = context.Set<T>();
	}

	public virtual async Task<T?> GetByIdAsync(int id)
	{
		return await _dbSet.FindAsync(id);
	}

	public virtual async Task<IQueryable<T>> GetAllAsync()
	{
		return await Task.FromResult(_dbSet.AsQueryable());
	}

	public virtual IQueryable<T> Find(Expression<Func<T, bool>> predicate)
	{
		return _dbSet.Where(predicate);
	}

	public virtual async Task<T> AddAsync(T entity)
	{
		await _dbSet.AddAsync(entity);
		return entity;
	}

	public virtual async Task<T> UpdateAsync(T entity)
	{
		_dbSet.Update(entity);
		return entity;
	}

	public virtual void Delete(T entity)
	{
		if (_context.Entry(entity).State == EntityState.Detached)
		{
			EntityEntry<T> entityEntry = _context.ChangeTracker.Entries<T>().FirstOrDefault((EntityEntry<T> e) => e.Entity.Id == entity.Id);
			if (entityEntry != null)
			{
				entityEntry.State = EntityState.Deleted;
				return;
			}
			_dbSet.Attach(entity);
		}
		_dbSet.Remove(entity);
	}

	public virtual async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await _dbSet.AnyAsync((T e) => e.Id == id);
	}

	DbContext IRepository<T>.GetDbContext()
	{
		return _context;
	}
}
