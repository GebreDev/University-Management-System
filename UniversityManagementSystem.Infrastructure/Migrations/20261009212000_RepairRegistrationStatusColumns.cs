using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UniversityManagementSystem.Infrastructure.Data;

#nullable disable

namespace UniversityManagementSystem.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261009212000_RepairRegistrationStatusColumns")]
    public partial class RepairRegistrationStatusColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'dbo.OfferedCourseSchedules', N'U') IS NULL
                    THROW 51000, 'Registration schema repair requires dbo.OfferedCourseSchedules.', 1;

                IF COL_LENGTH(N'dbo.OfferedCourseSchedules', N'Status') IS NULL
                BEGIN
                    ALTER TABLE dbo.OfferedCourseSchedules
                        ADD Status int NOT NULL
                            CONSTRAINT DF_OfferedCourseSchedules_Status_Repair DEFAULT (1);
                END;

                IF OBJECT_ID(N'dbo.RegistrationStatusHistories', N'U') IS NULL
                    THROW 51000, 'Registration schema repair requires dbo.RegistrationStatusHistories.', 1;

                IF COL_LENGTH(N'dbo.RegistrationStatusHistories', N'Status') IS NULL
                BEGIN
                    ALTER TABLE dbo.RegistrationStatusHistories
                        ADD Status int NOT NULL
                            CONSTRAINT DF_RegistrationStatusHistories_Status_Repair DEFAULT (1);
                END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Keep repaired columns because they may already have existed and contain data.
        }
    }
}
