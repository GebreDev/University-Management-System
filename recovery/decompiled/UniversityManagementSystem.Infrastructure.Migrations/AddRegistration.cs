using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260906120000_AddRegistration")]
public class AddRegistration : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("SemesterOfferedCourses", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			SemesterId = table.Column<int>("int"),
			CourseId = table.Column<int>("int"),
			DepartmentId = table.Column<int>("int"),
			Capacity = table.Column<int>("int"),
			IsOpen = table.Column<bool>("bit"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_SemesterOfferedCourses", x => x.Id);
			table.ForeignKey("FK_SemesterOfferedCourses_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_SemesterOfferedCourses_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_SemesterOfferedCourses_Semesters_SemesterId", x => x.SemesterId, "Semesters", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("OfferedCourseSchedules", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			SemesterOfferedCourseId = table.Column<int>("int"),
			DayOfWeek = table.Column<int>("int"),
			StartTime = table.Column<TimeSpan>("time"),
			EndTime = table.Column<TimeSpan>("time"),
			Room = table.Column<string>("nvarchar(max)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_OfferedCourseSchedules", x => x.Id);
			table.ForeignKey("FK_OfferedCourseSchedules_SemesterOfferedCourses_SemesterOfferedCourseId", x => x.SemesterOfferedCourseId, "SemesterOfferedCourses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Registrations", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			StudentId = table.Column<int>("int"),
			SemesterId = table.Column<int>("int"),
			DepartmentId = table.Column<int>("int"),
			Status = table.Column<int>("int"),
			TotalCredits = table.Column<int>("int"),
			RejectionReason = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			ReviewedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			ReviewedBy = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_Registrations", x => x.Id);
			table.ForeignKey("FK_Registrations_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Registrations_Semesters_SemesterId", x => x.SemesterId, "Semesters", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Registrations_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("RegistrationItems", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			RegistrationId = table.Column<int>("int"),
			SemesterOfferedCourseId = table.Column<int>("int"),
			Credits = table.Column<int>("int"),
			Status = table.Column<int>("int"),
			Note = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_RegistrationItems", x => x.Id);
			table.ForeignKey("FK_RegistrationItems_Registrations_RegistrationId", x => x.RegistrationId, "Registrations", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_RegistrationItems_SemesterOfferedCourses_SemesterOfferedCourseId", x => x.SemesterOfferedCourseId, "SemesterOfferedCourses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("RegistrationStatusHistories", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			RegistrationId = table.Column<int>("int"),
			OldStatus = table.Column<int>("int"),
			NewStatus = table.Column<int>("int"),
			ChangedAt = table.Column<DateTime>("datetime2"),
			ChangedBy = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			Reason = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_RegistrationStatusHistories", x => x.Id);
			table.ForeignKey("FK_RegistrationStatusHistories_Registrations_RegistrationId", x => x.RegistrationId, "Registrations", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateIndex("IX_SemesterOfferedCourses_CourseId", "SemesterOfferedCourses", "CourseId");
		migrationBuilder.CreateIndex("IX_SemesterOfferedCourses_DepartmentId", "SemesterOfferedCourses", "DepartmentId");
		migrationBuilder.CreateIndex("IX_SemesterOfferedCourses_SemesterId", "SemesterOfferedCourses", "SemesterId");
		migrationBuilder.CreateIndex("IX_OfferedCourseSchedules_SemesterOfferedCourseId", "OfferedCourseSchedules", "SemesterOfferedCourseId");
		migrationBuilder.CreateIndex("IX_Registrations_DepartmentId", "Registrations", "DepartmentId");
		migrationBuilder.CreateIndex("IX_Registrations_SemesterId", "Registrations", "SemesterId");
		migrationBuilder.CreateIndex("IX_Registrations_StudentId", "Registrations", "StudentId");
		migrationBuilder.CreateIndex("IX_RegistrationItems_RegistrationId", "RegistrationItems", "RegistrationId");
		migrationBuilder.CreateIndex("IX_RegistrationItems_SemesterOfferedCourseId", "RegistrationItems", "SemesterOfferedCourseId");
		migrationBuilder.CreateIndex("IX_RegistrationStatusHistories_RegistrationId", "RegistrationStatusHistories", "RegistrationId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("RegistrationStatusHistories");
		migrationBuilder.DropTable("RegistrationItems");
		migrationBuilder.DropTable("Registrations");
		migrationBuilder.DropTable("OfferedCourseSchedules");
		migrationBuilder.DropTable("SemesterOfferedCourses");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "10.0.0");
	}
}
