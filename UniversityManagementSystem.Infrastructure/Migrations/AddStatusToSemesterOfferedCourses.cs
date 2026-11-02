using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261009105601_AddStatusToSemesterOfferedCourses")]
public class AddStatusToSemesterOfferedCourses : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<int>(
			name: "Status",
			table: "SemesterOfferedCourses",
			type: "int",
			nullable: false,
			defaultValue: 1);
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "Status",
			table: "SemesterOfferedCourses");
	}
}
