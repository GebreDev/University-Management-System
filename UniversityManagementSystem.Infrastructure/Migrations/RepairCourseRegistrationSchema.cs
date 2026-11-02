using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261009111755_RepairCourseRegistrationSchema")]
public class RepairCourseRegistrationSchema : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.Sql(
			"""
			IF OBJECT_ID(N'dbo.SemesterOfferedCourses', N'U') IS NULL
				THROW 51000, 'Course registration schema repair requires dbo.SemesterOfferedCourses.', 1;

			IF OBJECT_ID(N'dbo.RegistrationItems', N'U') IS NULL
				THROW 51000, 'Course registration schema repair requires dbo.RegistrationItems.', 1;

			IF COL_LENGTH(N'dbo.SemesterOfferedCourses', N'Status') IS NULL
			BEGIN
				ALTER TABLE dbo.SemesterOfferedCourses
					ADD Status int NOT NULL
						CONSTRAINT DF_SemesterOfferedCourses_Status_Repair DEFAULT (1);
			END
			ELSE IF EXISTS
			(
				SELECT 1
				FROM sys.columns
				WHERE object_id = OBJECT_ID(N'dbo.SemesterOfferedCourses')
					AND name = N'Status'
					AND is_nullable = 1
			)
			BEGIN
				UPDATE dbo.SemesterOfferedCourses SET Status = 1 WHERE Status IS NULL;
				ALTER TABLE dbo.SemesterOfferedCourses ALTER COLUMN Status int NOT NULL;
			END;

			IF COL_LENGTH(N'dbo.RegistrationItems', N'SemesterOfferedCourseId') IS NULL
			BEGIN
				ALTER TABLE dbo.RegistrationItems
					ADD SemesterOfferedCourseId int NOT NULL
						CONSTRAINT DF_RegistrationItems_SemesterOfferedCourseId_Repair DEFAULT (0);
			END
			ELSE IF EXISTS
			(
				SELECT 1
				FROM sys.columns
				WHERE object_id = OBJECT_ID(N'dbo.RegistrationItems')
					AND name = N'SemesterOfferedCourseId'
					AND is_nullable = 1
			)
			BEGIN
				UPDATE dbo.RegistrationItems
				SET SemesterOfferedCourseId = 0
				WHERE SemesterOfferedCourseId IS NULL;
				ALTER TABLE dbo.RegistrationItems
					ALTER COLUMN SemesterOfferedCourseId int NOT NULL;
			END;

			IF NOT EXISTS
			(
				SELECT 1
				FROM sys.foreign_key_columns AS fkc
				INNER JOIN sys.columns AS c
					ON c.object_id = fkc.parent_object_id
					AND c.column_id = fkc.parent_column_id
				WHERE fkc.parent_object_id = OBJECT_ID(N'dbo.RegistrationItems')
					AND fkc.referenced_object_id = OBJECT_ID(N'dbo.SemesterOfferedCourses')
					AND c.name = N'SemesterOfferedCourseId'
			)
			BEGIN
				ALTER TABLE dbo.RegistrationItems WITH NOCHECK
					ADD CONSTRAINT FK_RegistrationItems_SemesterOfferedCourses_SemesterOfferedCourseId
					FOREIGN KEY (SemesterOfferedCourseId)
					REFERENCES dbo.SemesterOfferedCourses (Id);
			END;
			""");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		// Keep repaired columns: they may have existed before this migration and contain data.
	}
}
