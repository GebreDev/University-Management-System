using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Domain.Entities;

namespace UniversityManagementSystem.Infrastructure.Data;

public static class FeeSettingsHelper
{
	public static async Task<FinanceSetting> GetAsync(ApplicationDbContext context)
	{
		FinanceSetting settings = await context.FinanceSettings.FirstOrDefaultAsync();
		if (settings == null)
		{
			settings = new FinanceSetting();
			context.FinanceSettings.Add(settings);
			await context.SaveChangesAsync();
		}
		return settings;
	}
}
