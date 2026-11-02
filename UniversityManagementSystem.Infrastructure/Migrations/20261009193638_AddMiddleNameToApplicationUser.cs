using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UniversityManagementSystem.Infrastructure.Data;

#nullable disable

namespace UniversityManagementSystem.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261009193638_AddMiddleNameToApplicationUser")]
    public partial class AddMiddleNameToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE users
                SET MiddleName = students.MiddleName
                FROM AspNetUsers AS users
                INNER JOIN Students AS students ON students.ApplicationUserId = users.Id
                WHERE NULLIF(LTRIM(RTRIM(students.MiddleName)), '') IS NOT NULL;

                UPDATE users
                SET MiddleName = instructors.MiddleName
                FROM AspNetUsers AS users
                INNER JOIN Instructors AS instructors ON instructors.ApplicationUserId = users.Id
                WHERE NULLIF(LTRIM(RTRIM(instructors.MiddleName)), '') IS NOT NULL
                  AND NULLIF(LTRIM(RTRIM(users.MiddleName)), '') IS NULL;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AspNetUsers");
        }
    }
}
