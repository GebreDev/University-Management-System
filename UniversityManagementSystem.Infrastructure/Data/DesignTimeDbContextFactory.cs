using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UniversityManagementSystem.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		string text = Directory.GetCurrentDirectory();
		string text2 = Path.Combine(text, "..", "UniversityManagementSystem.Web");
		if (Directory.Exists(text2) && File.Exists(Path.Combine(text2, "appsettings.json")))
		{
			text = Path.GetFullPath(text2);
		}
		else if (!File.Exists(Path.Combine(text, "appsettings.json")))
		{
			string fullPath = Path.GetFullPath(Path.Combine(text, ".."));
			if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
			{
				text = fullPath;
			}
		}
		IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(text).AddJsonFile("appsettings.json").Build();
		DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		string connectionString = configuration.GetConnectionString("DefaultConnection");
		dbContextOptionsBuilder.UseSqlServer(connectionString);
		return new ApplicationDbContext(dbContextOptionsBuilder.Options);
	}
}
