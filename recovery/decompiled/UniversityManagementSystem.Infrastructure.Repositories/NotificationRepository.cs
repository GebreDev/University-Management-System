using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository, IRepository<Notification>
{
	public NotificationRepository(ApplicationDbContext context)
		: base(context)
	{
	}

	public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, bool unreadOnly = false)
	{
		IQueryable<Notification> query = _dbSet.Where((Notification n) => n.UserId == userId);
		if (unreadOnly)
		{
			query = query.Where((Notification n) => !n.IsRead);
		}
		return await query.OrderByDescending((Notification n) => n.CreatedAt).ToListAsync();
	}

	public async Task<int> GetUnreadCountAsync(string userId)
	{
		return await _dbSet.CountAsync((Notification n) => n.UserId == userId && !n.IsRead);
	}

	public async Task MarkAllAsReadAsync(string userId)
	{
		foreach (Notification notification in await _dbSet.Where((Notification n) => n.UserId == userId && !n.IsRead).ToListAsync())
		{
			notification.IsRead = true;
			notification.UpdatedAt = DateTime.UtcNow;
		}
		await SaveChangesAsync();
	}

	public async Task DeleteOldNotificationsAsync(DateTime olderThan)
	{
		List<Notification> oldNotifications = await _dbSet.Where((Notification n) => n.CreatedAt < olderThan).ToListAsync();
		_dbSet.RemoveRange(oldNotifications);
		await SaveChangesAsync();
	}
}
