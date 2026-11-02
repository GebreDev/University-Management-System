using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using UniversityManagementSystem.Infrastructure.Data;

namespace UniversityManagementSystem.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260729083033_InitialCreate")]
public class InitialCreate : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("AspNetRoles", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<string>("nvarchar(450)");
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			return new
			{
				Id = id,
				Name = name,
				NormalizedName = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true),
				ConcurrencyStamp = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetRoles", x => x.Id);
		});
		migrationBuilder.CreateTable("AspNetUsers", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<string>("nvarchar(450)");
			OperationBuilder<AddColumnOperation> firstName = table.Column<string>("nvarchar(max)");
			OperationBuilder<AddColumnOperation> lastName = table.Column<string>("nvarchar(max)");
			OperationBuilder<AddColumnOperation> dateOfBirth = table.Column<DateTime>("datetime2");
			OperationBuilder<AddColumnOperation> address = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> accountStatus = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> createdAt = table.Column<DateTime>("datetime2");
			OperationBuilder<AddColumnOperation> updatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> profilePictureUrl = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true);
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> userName = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> normalizedUserName = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> email = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			return new
			{
				Id = id,
				FirstName = firstName,
				LastName = lastName,
				DateOfBirth = dateOfBirth,
				Address = address,
				AccountStatus = accountStatus,
				CreatedAt = createdAt,
				UpdatedAt = updatedAt,
				ProfilePictureUrl = profilePictureUrl,
				UserName = userName,
				NormalizedUserName = normalizedUserName,
				Email = email,
				NormalizedEmail = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true),
				EmailConfirmed = table.Column<bool>("bit"),
				PasswordHash = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				SecurityStamp = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				ConcurrencyStamp = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				PhoneNumber = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				PhoneNumberConfirmed = table.Column<bool>("bit"),
				TwoFactorEnabled = table.Column<bool>("bit"),
				LockoutEnd = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				LockoutEnabled = table.Column<bool>("bit"),
				AccessFailedCount = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetUsers", x => x.Id);
		});
		migrationBuilder.CreateTable("Departments", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			Name = table.Column<string>("nvarchar(max)"),
			Description = table.Column<string>("nvarchar(max)"),
			Code = table.Column<string>("nvarchar(max)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Departments", x => x.Id);
		});
		migrationBuilder.CreateTable("AspNetRoleClaims", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			RoleId = table.Column<string>("nvarchar(450)"),
			ClaimType = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			ClaimValue = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
			table.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("AspNetUserClaims", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			UserId = table.Column<string>("nvarchar(450)"),
			ClaimType = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			ClaimValue = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
			table.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("AspNetUserLogins", (ColumnsBuilder table) => new
		{
			LoginProvider = table.Column<string>("nvarchar(450)"),
			ProviderKey = table.Column<string>("nvarchar(450)"),
			ProviderDisplayName = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			UserId = table.Column<string>("nvarchar(450)")
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
			table.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("AspNetUserRoles", (ColumnsBuilder table) => new
		{
			UserId = table.Column<string>("nvarchar(450)"),
			RoleId = table.Column<string>("nvarchar(450)")
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
			table.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("AspNetUserTokens", (ColumnsBuilder table) => new
		{
			UserId = table.Column<string>("nvarchar(450)"),
			LoginProvider = table.Column<string>("nvarchar(450)"),
			Name = table.Column<string>("nvarchar(450)"),
			Value = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
			table.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("AuditLogs", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			UserId = table.Column<string>("nvarchar(450)"),
			UserName = table.Column<string>("nvarchar(max)"),
			UserRole = table.Column<string>("nvarchar(max)"),
			Action = table.Column<string>("nvarchar(max)"),
			Entity = table.Column<string>("nvarchar(max)"),
			EntityId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			Details = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			IpAddress = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			DeviceInfo = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			Timestamp = table.Column<DateTime>("datetime2"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_AuditLogs", x => x.Id);
			table.ForeignKey("FK_AuditLogs_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("ChatConversations", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			User1Id = table.Column<string>("nvarchar(450)"),
			User2Id = table.Column<string>("nvarchar(450)"),
			User1Role = table.Column<string>("nvarchar(max)"),
			User2Role = table.Column<string>("nvarchar(max)"),
			LastMessage = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			LastMessageTime = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_ChatConversations", x => x.Id);
			table.ForeignKey("FK_ChatConversations_AspNetUsers_User1Id", x => x.User1Id, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_ChatConversations_AspNetUsers_User2Id", x => x.User2Id, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Notifications", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			UserId = table.Column<string>("nvarchar(450)"),
			Title = table.Column<string>("nvarchar(max)"),
			Message = table.Column<string>("nvarchar(max)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			IsRead = table.Column<bool>("bit"),
			ReadAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Type = table.Column<string>("nvarchar(max)"),
			RelatedId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			LinkUrl = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Notifications", x => x.Id);
			table.ForeignKey("FK_Notifications_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Students", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			FirstName = table.Column<string>("nvarchar(max)"),
			LastName = table.Column<string>("nvarchar(max)"),
			Email = table.Column<string>("nvarchar(max)"),
			Phone = table.Column<string>("nvarchar(max)"),
			EnrollmentDate = table.Column<DateTime>("datetime2"),
			StudentId = table.Column<string>("nvarchar(450)"),
			DepartmentId = table.Column<int>("int"),
			ApplicationUserId = table.Column<string>("nvarchar(450)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Students", x => x.Id);
			table.ForeignKey("FK_Students_AspNetUsers_ApplicationUserId", x => x.ApplicationUserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Students_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Instructors", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			FirstName = table.Column<string>("nvarchar(max)"),
			LastName = table.Column<string>("nvarchar(max)"),
			Email = table.Column<string>("nvarchar(max)"),
			Phone = table.Column<string>("nvarchar(max)"),
			Qualification = table.Column<string>("nvarchar(max)"),
			HireDate = table.Column<DateTime>("datetime2"),
			DepartmentId = table.Column<int>("int"),
			ApplicationUserId = table.Column<string>("nvarchar(450)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Instructors", x => x.Id);
			table.ForeignKey("FK_Instructors_AspNetUsers_ApplicationUserId", x => x.ApplicationUserId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Instructors_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("ChatMessages", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			ConversationId = table.Column<int>("int"),
			SenderId = table.Column<string>("nvarchar(450)"),
			Content = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			SentAt = table.Column<DateTime>("datetime2"),
			IsRead = table.Column<bool>("bit"),
			ReadAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			AttachmentPath = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			AttachmentType = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_ChatMessages", x => x.Id);
			table.ForeignKey("FK_ChatMessages_AspNetUsers_SenderId", x => x.SenderId, "AspNetUsers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_ChatMessages_ChatConversations_ConversationId", x => x.ConversationId, "ChatConversations", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("FeeStructures", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			Name = table.Column<string>("nvarchar(max)"),
			Description = table.Column<string>("nvarchar(max)"),
			Amount = table.Column<decimal>("decimal(18,2)"),
			DueDate = table.Column<DateTime>("datetime2"),
			IsActive = table.Column<bool>("bit"),
			DepartmentId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			StudentId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_FeeStructures", x => x.Id);
			table.ForeignKey("FK_FeeStructures_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id");
			table.ForeignKey("FK_FeeStructures_Students_StudentId", x => x.StudentId, "Students", "Id");
		});
		migrationBuilder.CreateTable("Courses", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			Title = table.Column<string>("nvarchar(max)"),
			Description = table.Column<string>("nvarchar(max)"),
			Code = table.Column<string>("nvarchar(450)"),
			Credits = table.Column<int>("int"),
			StartDate = table.Column<DateTime>("datetime2"),
			EndDate = table.Column<DateTime>("datetime2"),
			DepartmentId = table.Column<int>("int"),
			InstructorId = table.Column<int>("int"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Courses", x => x.Id);
			table.ForeignKey("FK_Courses_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Courses_Instructors_InstructorId", x => x.InstructorId, "Instructors", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("FeePayments", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			StudentId = table.Column<int>("int"),
			FeeStructureId = table.Column<int>("int"),
			AmountPaid = table.Column<decimal>("decimal(18,2)"),
			PaymentDate = table.Column<DateTime>("datetime2"),
			Status = table.Column<int>("int"),
			ReceiptNumber = table.Column<string>("nvarchar(max)"),
			PaymentMethod = table.Column<string>("nvarchar(max)"),
			TransactionId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			DiscountAmount = table.Column<decimal>("decimal(18,2)"),
			ScholarshipAmount = table.Column<decimal>("decimal(18,2)"),
			LatePenalty = table.Column<decimal>("decimal(18,2)"),
			Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_FeePayments", x => x.Id);
			table.ForeignKey("FK_FeePayments_FeeStructures_FeeStructureId", x => x.FeeStructureId, "FeeStructures", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_FeePayments_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Assignments", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			Title = table.Column<string>("nvarchar(max)"),
			Description = table.Column<string>("nvarchar(max)"),
			CourseId = table.Column<int>("int"),
			InstructorId = table.Column<int>("int"),
			Deadline = table.Column<DateTime>("datetime2"),
			TotalMarks = table.Column<int>("int"),
			AttachmentPath = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			IsPublished = table.Column<bool>("bit"),
			AllowLateSubmission = table.Column<bool>("bit"),
			LateSubmissionPenalty = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Assignments", x => x.Id);
			table.ForeignKey("FK_Assignments_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Assignments_Instructors_InstructorId", x => x.InstructorId, "Instructors", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Attendances", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			StudentId = table.Column<int>("int"),
			CourseId = table.Column<int>("int"),
			Date = table.Column<DateTime>("datetime2"),
			Status = table.Column<int>("int"),
			Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			InstructorId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_Attendances", x => x.Id);
			table.ForeignKey("FK_Attendances_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Attendances_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Attendances_Instructors_InstructorId", x => x.InstructorId, "Instructors", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Enrollments", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			EnrollmentDate = table.Column<DateTime>("datetime2"),
			Status = table.Column<int>("int"),
			ApprovalDate = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			RejectionReason = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			StudentId = table.Column<int>("int"),
			CourseId = table.Column<int>("int"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_Enrollments", x => x.Id);
			table.ForeignKey("FK_Enrollments_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_Enrollments_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Exams", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			Title = table.Column<string>("nvarchar(max)"),
			Description = table.Column<string>("nvarchar(max)"),
			CourseId = table.Column<int>("int"),
			InstructorId = table.Column<int>("int"),
			Type = table.Column<int>("int"),
			StartDateTime = table.Column<DateTime>("datetime2"),
			EndDateTime = table.Column<DateTime>("datetime2"),
			DurationMinutes = table.Column<int>("int"),
			TotalMarks = table.Column<int>("int"),
			PassingMarks = table.Column<int>("int"),
			IsPublished = table.Column<bool>("bit"),
			IsRandomized = table.Column<bool>("bit"),
			Instructions = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			AttachmentPath = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Exams", x => x.Id);
			table.ForeignKey("FK_Exams_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Exams_Instructors_InstructorId", x => x.InstructorId, "Instructors", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Submissions", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			AssignmentId = table.Column<int>("int"),
			StudentId = table.Column<int>("int"),
			SubmittedAt = table.Column<DateTime>("datetime2"),
			FilePath = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			Grade = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			Feedback = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			IsGraded = table.Column<bool>("bit"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Submissions", x => x.Id);
			table.ForeignKey("FK_Submissions_Assignments_AssignmentId", x => x.AssignmentId, "Assignments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_Submissions_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("ExamResults", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			StudentId = table.Column<int>("int"),
			ExamId = table.Column<int>("int"),
			TotalScore = table.Column<int>("int"),
			Grade = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			IsPass = table.Column<bool>("bit"),
			Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_ExamResults", x => x.Id);
			table.ForeignKey("FK_ExamResults_Exams_ExamId", x => x.ExamId, "Exams", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_ExamResults_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Questions", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			ExamId = table.Column<int>("int"),
			QuestionText = table.Column<string>("nvarchar(max)"),
			Type = table.Column<int>("int"),
			Marks = table.Column<int>("int"),
			CorrectAnswer = table.Column<string>("nvarchar(max)"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Questions", x => x.Id);
			table.ForeignKey("FK_Questions_Exams_ExamId", x => x.ExamId, "Exams", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Grades", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			StudentId = table.Column<int>("int"),
			CourseId = table.Column<int>("int"),
			Type = table.Column<int>("int"),
			ExamId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			AssignmentId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			SubmissionId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			Score = table.Column<decimal>("decimal(18,2)"),
			MaxScore = table.Column<decimal>("decimal(18,2)"),
			LetterGrade = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			Semester = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_Grades", x => x.Id);
			table.ForeignKey("FK_Grades_Assignments_AssignmentId", x => x.AssignmentId, "Assignments", "Id");
			table.ForeignKey("FK_Grades_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Grades_Exams_ExamId", x => x.ExamId, "Exams", "Id");
			table.ForeignKey("FK_Grades_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Grades_Submissions_SubmissionId", x => x.SubmissionId, "Submissions", "Id");
		});
		migrationBuilder.CreateTable("QuestionOptions", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			QuestionId = table.Column<int>("int"),
			OptionText = table.Column<string>("nvarchar(max)"),
			IsCorrect = table.Column<bool>("bit"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_QuestionOptions", x => x.Id);
			table.ForeignKey("FK_QuestionOptions_Questions_QuestionId", x => x.QuestionId, "Questions", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("StudentAnswers", (ColumnsBuilder table) => new
		{
			Id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1"),
			StudentId = table.Column<int>("int"),
			QuestionId = table.Column<int>("int"),
			AnswerText = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
			Score = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
			IsCorrect = table.Column<bool>("bit"),
			CreatedAt = table.Column<DateTime>("datetime2"),
			UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
			Status = table.Column<int>("int")
		}, null, table =>
		{
			table.PrimaryKey("PK_StudentAnswers", x => x.Id);
			table.ForeignKey("FK_StudentAnswers_Questions_QuestionId", x => x.QuestionId, "Questions", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_StudentAnswers_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateIndex("IX_AspNetRoleClaims_RoleId", "AspNetRoleClaims", "RoleId");
		migrationBuilder.CreateIndex("RoleNameIndex", "AspNetRoles", "NormalizedName", null, unique: true, "[NormalizedName] IS NOT NULL");
		migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId");
		migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId");
		migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId");
		migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail");
		migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", null, unique: true, "[NormalizedUserName] IS NOT NULL");
		migrationBuilder.CreateIndex("IX_Assignments_CourseId", "Assignments", "CourseId");
		migrationBuilder.CreateIndex("IX_Assignments_InstructorId", "Assignments", "InstructorId");
		migrationBuilder.CreateIndex("IX_Attendances_CourseId", "Attendances", "CourseId");
		migrationBuilder.CreateIndex("IX_Attendances_StudentId", "Attendances", "StudentId");
		migrationBuilder.CreateIndex("IX_Attendances_InstructorId", "Attendances", "InstructorId");
		migrationBuilder.CreateIndex("IX_AuditLogs_UserId", "AuditLogs", "UserId");
		migrationBuilder.CreateIndex("IX_ChatConversations_User1Id", "ChatConversations", "User1Id");
		migrationBuilder.CreateIndex("IX_ChatConversations_User2Id", "ChatConversations", "User2Id");
		migrationBuilder.CreateIndex("IX_ChatMessages_ConversationId", "ChatMessages", "ConversationId");
		migrationBuilder.CreateIndex("IX_ChatMessages_SenderId", "ChatMessages", "SenderId");
		migrationBuilder.CreateIndex("IX_Courses_Code", "Courses", "Code", null, unique: true);
		migrationBuilder.CreateIndex("IX_Courses_DepartmentId", "Courses", "DepartmentId");
		migrationBuilder.CreateIndex("IX_Courses_InstructorId", "Courses", "InstructorId");
		migrationBuilder.CreateIndex("IX_Enrollments_CourseId", "Enrollments", "CourseId");
		migrationBuilder.CreateIndex("IX_Enrollments_StudentId", "Enrollments", "StudentId");
		migrationBuilder.CreateIndex("IX_ExamResults_ExamId", "ExamResults", "ExamId");
		migrationBuilder.CreateIndex("IX_ExamResults_StudentId", "ExamResults", "StudentId");
		migrationBuilder.CreateIndex("IX_Exams_CourseId", "Exams", "CourseId");
		migrationBuilder.CreateIndex("IX_Exams_InstructorId", "Exams", "InstructorId");
		migrationBuilder.CreateIndex("IX_FeePayments_FeeStructureId", "FeePayments", "FeeStructureId");
		migrationBuilder.CreateIndex("IX_FeePayments_StudentId", "FeePayments", "StudentId");
		migrationBuilder.CreateIndex("IX_FeeStructures_DepartmentId", "FeeStructures", "DepartmentId");
		migrationBuilder.CreateIndex("IX_FeeStructures_StudentId", "FeeStructures", "StudentId");
		migrationBuilder.CreateIndex("IX_Grades_AssignmentId", "Grades", "AssignmentId");
		migrationBuilder.CreateIndex("IX_Grades_CourseId", "Grades", "CourseId");
		migrationBuilder.CreateIndex("IX_Grades_ExamId", "Grades", "ExamId");
		migrationBuilder.CreateIndex("IX_Grades_StudentId", "Grades", "StudentId");
		migrationBuilder.CreateIndex("IX_Grades_SubmissionId", "Grades", "SubmissionId");
		migrationBuilder.CreateIndex("IX_Notifications_UserId", "Notifications", "UserId");
		migrationBuilder.CreateIndex("IX_QuestionOptions_QuestionId", "QuestionOptions", "QuestionId");
		migrationBuilder.CreateIndex("IX_Questions_ExamId", "Questions", "ExamId");
		migrationBuilder.CreateIndex("IX_StudentAnswers_QuestionId", "StudentAnswers", "QuestionId");
		migrationBuilder.CreateIndex("IX_StudentAnswers_StudentId", "StudentAnswers", "StudentId");
		migrationBuilder.CreateIndex("IX_Students_ApplicationUserId", "Students", "ApplicationUserId", null, unique: true);
		migrationBuilder.CreateIndex("IX_Students_DepartmentId", "Students", "DepartmentId");
		migrationBuilder.CreateIndex("IX_Students_StudentId", "Students", "StudentId", null, unique: true);
		migrationBuilder.CreateIndex("IX_Submissions_AssignmentId", "Submissions", "AssignmentId");
		migrationBuilder.CreateIndex("IX_Submissions_StudentId", "Submissions", "StudentId");
		migrationBuilder.CreateIndex("IX_Instructors_ApplicationUserId", "Instructors", "ApplicationUserId", null, unique: true);
		migrationBuilder.CreateIndex("IX_Instructors_DepartmentId", "Instructors", "DepartmentId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("AspNetRoleClaims");
		migrationBuilder.DropTable("AspNetUserClaims");
		migrationBuilder.DropTable("AspNetUserLogins");
		migrationBuilder.DropTable("AspNetUserRoles");
		migrationBuilder.DropTable("AspNetUserTokens");
		migrationBuilder.DropTable("Attendances");
		migrationBuilder.DropTable("AuditLogs");
		migrationBuilder.DropTable("ChatMessages");
		migrationBuilder.DropTable("Enrollments");
		migrationBuilder.DropTable("ExamResults");
		migrationBuilder.DropTable("FeePayments");
		migrationBuilder.DropTable("Grades");
		migrationBuilder.DropTable("Notifications");
		migrationBuilder.DropTable("QuestionOptions");
		migrationBuilder.DropTable("StudentAnswers");
		migrationBuilder.DropTable("AspNetRoles");
		migrationBuilder.DropTable("ChatConversations");
		migrationBuilder.DropTable("FeeStructures");
		migrationBuilder.DropTable("Submissions");
		migrationBuilder.DropTable("Questions");
		migrationBuilder.DropTable("Assignments");
		migrationBuilder.DropTable("Students");
		migrationBuilder.DropTable("Exams");
		migrationBuilder.DropTable("Courses");
		migrationBuilder.DropTable("Instructors");
		migrationBuilder.DropTable("AspNetUsers");
		migrationBuilder.DropTable("Departments");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "10.0.10").HasAnnotation("Relational:MaxIdentifierLength", 128);
		modelBuilder.UseIdentityColumns(1L);
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ApplicationUser", (EntityTypeBuilder b) =>
		{
			b.Property<string>("Id").HasColumnType("nvarchar(450)");
			b.Property<int>("AccessFailedCount").HasColumnType("int");
			b.Property<int>("AccountStatus").HasColumnType("int");
			b.Property<string>("Address").HasColumnType("nvarchar(max)");
			b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("DateOfBirth").HasColumnType("datetime2");
			b.Property<string>("Email").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<bool>("EmailConfirmed").HasColumnType("bit");
			b.Property<string>("FirstName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("LastName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<bool>("LockoutEnabled").HasColumnType("bit");
			b.Property<DateTimeOffset?>("LockoutEnd").HasColumnType("datetimeoffset");
			b.Property<string>("NormalizedEmail").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("NormalizedUserName").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("PasswordHash").HasColumnType("nvarchar(max)");
			b.Property<string>("PhoneNumber").HasColumnType("nvarchar(max)");
			b.Property<bool>("PhoneNumberConfirmed").HasColumnType("bit");
			b.Property<string>("ProfilePictureUrl").HasColumnType("nvarchar(max)");
			b.Property<string>("SecurityStamp").HasColumnType("nvarchar(max)");
			b.Property<bool>("TwoFactorEnabled").HasColumnType("bit");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("UserName").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.HasKey("Id");
			b.HasIndex("NormalizedEmail").HasDatabaseName("EmailIndex");
			b.HasIndex("NormalizedUserName").IsUnique().HasDatabaseName("UserNameIndex")
				.HasFilter("[NormalizedUserName] IS NOT NULL");
			b.ToTable("AspNetUsers", (string?)null);
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Assignment", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<bool>("AllowLateSubmission").HasColumnType("bit");
			b.Property<string>("AttachmentPath").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("Deadline").HasColumnType("datetime2");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<bool>("IsPublished").HasColumnType("bit");
			b.Property<int?>("LateSubmissionPenalty").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("InstructorId").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("TotalMarks").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("InstructorId");
			b.ToTable("Assignments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Attendance", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("Date").HasColumnType("datetime2");
			b.Property<string>("Remarks").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<int?>("InstructorId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("StudentId");
			b.HasIndex("InstructorId");
			b.ToTable("Attendances");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AuditLog", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("Action").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Details").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<string>("DeviceInfo").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<string>("Entity").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int?>("EntityId").HasColumnType("int");
			b.Property<string>("IpAddress").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime>("Timestamp").HasColumnType("datetime2");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<string>("UserName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("UserRole").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("AuditLogs");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ChatConversation", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("LastMessage").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<DateTime?>("LastMessageTime").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("User1Id").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<string>("User1Role").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("User2Id").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<string>("User2Role").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("User1Id");
			b.HasIndex("User2Id");
			b.ToTable("ChatConversations");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ChatMessage", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("AttachmentPath").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<string>("AttachmentType").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<string>("Content").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("ConversationId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<bool>("IsRead").HasColumnType("bit");
			b.Property<DateTime?>("ReadAt").HasColumnType("datetime2");
			b.Property<string>("SenderId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime>("SentAt").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("ConversationId");
			b.HasIndex("SenderId");
			b.ToTable("ChatMessages");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Course", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("Code").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("Credits").HasColumnType("int");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("EndDate").HasColumnType("datetime2");
			b.Property<DateTime>("StartDate").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("InstructorId").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("Code").IsUnique();
			b.HasIndex("DepartmentId");
			b.HasIndex("InstructorId");
			b.ToTable("Courses");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Department", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("Code").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Name").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.ToTable("Departments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Enrollment", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime?>("ApprovalDate").HasColumnType("datetime2");
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("EnrollmentDate").HasColumnType("datetime2");
			b.Property<string>("RejectionReason").HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("StudentId");
			b.ToTable("Enrollments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Exam", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("AttachmentPath").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("DurationMinutes").HasColumnType("int");
			b.Property<DateTime>("EndDateTime").HasColumnType("datetime2");
			b.Property<string>("Instructions").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<bool>("IsPublished").HasColumnType("bit");
			b.Property<bool>("IsRandomized").HasColumnType("bit");
			b.Property<int>("PassingMarks").HasColumnType("int");
			b.Property<DateTime>("StartDateTime").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("InstructorId").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("TotalMarks").HasColumnType("int");
			b.Property<int>("Type").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("InstructorId");
			b.ToTable("Exams");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ExamResult", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("ExamId").HasColumnType("int");
			b.Property<string>("Grade").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<bool>("IsPass").HasColumnType("bit");
			b.Property<string>("Remarks").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<int>("TotalScore").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("ExamId");
			b.HasIndex("StudentId");
			b.ToTable("ExamResults");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FeePayment", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<decimal>("AmountPaid").HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<decimal>("DiscountAmount").HasColumnType("decimal(18,2)");
			b.Property<int>("FeeStructureId").HasColumnType("int");
			b.Property<decimal>("LatePenalty").HasColumnType("decimal(18,2)");
			b.Property<DateTime>("PaymentDate").HasColumnType("datetime2");
			b.Property<string>("PaymentMethod").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("ReceiptNumber").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Remarks").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<decimal>("ScholarshipAmount").HasColumnType("decimal(18,2)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<string>("TransactionId").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("FeeStructureId");
			b.HasIndex("StudentId");
			b.ToTable("FeePayments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FeeStructure", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<decimal>("Amount").HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int?>("DepartmentId").HasColumnType("int");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("DueDate").HasColumnType("datetime2");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<string>("Name").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int?>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("DepartmentId");
			b.HasIndex("StudentId");
			b.ToTable("FeeStructures");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Grade", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int?>("AssignmentId").HasColumnType("int");
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int?>("ExamId").HasColumnType("int");
			b.Property<string>("LetterGrade").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<decimal>("MaxScore").HasColumnType("decimal(18,2)");
			b.Property<string>("Remarks").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<decimal>("Score").HasColumnType("decimal(18,2)");
			b.Property<int?>("Semester").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<int?>("SubmissionId").HasColumnType("int");
			b.Property<int>("Type").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AssignmentId");
			b.HasIndex("CourseId");
			b.HasIndex("ExamId");
			b.HasIndex("StudentId");
			b.HasIndex("SubmissionId");
			b.ToTable("Grades");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Notification", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<bool>("IsRead").HasColumnType("bit");
			b.Property<string>("LinkUrl").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<string>("Message").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime?>("ReadAt").HasColumnType("datetime2");
			b.Property<string>("RelatedId").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Type").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("Notifications");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Question", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("CorrectAnswer").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("ExamId").HasColumnType("int");
			b.Property<int>("Marks").HasColumnType("int");
			b.Property<string>("QuestionText").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("Type").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("ExamId");
			b.ToTable("Questions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.QuestionOption", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<bool>("IsCorrect").HasColumnType("bit");
			b.Property<string>("OptionText").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("QuestionId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("QuestionId");
			b.ToTable("QuestionOptions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ApplicationUserId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<string>("Email").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("EnrollmentDate").HasColumnType("datetime2");
			b.Property<string>("FirstName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("LastName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Phone").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("StudentId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("ApplicationUserId").IsUnique();
			b.HasIndex("DepartmentId");
			b.HasIndex("StudentId").IsUnique();
			b.ToTable("Students");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.StudentAnswer", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("AnswerText").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<bool>("IsCorrect").HasColumnType("bit");
			b.Property<int>("QuestionId").HasColumnType("int");
			b.Property<int?>("Score").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("QuestionId");
			b.HasIndex("StudentId");
			b.ToTable("StudentAnswers");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Submission", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AssignmentId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Feedback").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<string>("FilePath").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int?>("Grade").HasColumnType("int");
			b.Property<bool>("IsGraded").HasColumnType("bit");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime>("SubmittedAt").HasColumnType("datetime2");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AssignmentId");
			b.HasIndex("StudentId");
			b.ToTable("Submissions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Instructor", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ApplicationUserId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<string>("Email").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("FirstName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("HireDate").HasColumnType("datetime2");
			b.Property<string>("LastName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Phone").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Qualification").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("ApplicationUserId").IsUnique();
			b.HasIndex("DepartmentId");
			b.ToTable("Instructors");
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", (EntityTypeBuilder b) =>
		{
			b.Property<string>("Id").HasColumnType("nvarchar(450)");
			b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
			b.Property<string>("Name").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("NormalizedName").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.HasKey("Id");
			b.HasIndex("NormalizedName").IsUnique().HasDatabaseName("RoleNameIndex")
				.HasFilter("[NormalizedName] IS NOT NULL");
			b.ToTable("AspNetRoles", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
			b.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
			b.Property<string>("RoleId").IsRequired().HasColumnType("nvarchar(450)");
			b.HasKey("Id");
			b.HasIndex("RoleId");
			b.ToTable("AspNetRoleClaims", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
			b.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
			b.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("AspNetUserClaims", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", (EntityTypeBuilder b) =>
		{
			b.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
			b.Property<string>("ProviderKey").HasColumnType("nvarchar(450)");
			b.Property<string>("ProviderDisplayName").HasColumnType("nvarchar(max)");
			b.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
			b.HasKey("LoginProvider", "ProviderKey");
			b.HasIndex("UserId");
			b.ToTable("AspNetUserLogins", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", (EntityTypeBuilder b) =>
		{
			b.Property<string>("UserId").HasColumnType("nvarchar(450)");
			b.Property<string>("RoleId").HasColumnType("nvarchar(450)");
			b.HasKey("UserId", "RoleId");
			b.HasIndex("RoleId");
			b.ToTable("AspNetUserRoles", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", (EntityTypeBuilder b) =>
		{
			b.Property<string>("UserId").HasColumnType("nvarchar(450)");
			b.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
			b.Property<string>("Name").HasColumnType("nvarchar(450)");
			b.Property<string>("Value").HasColumnType("nvarchar(max)");
			b.HasKey("UserId", "LoginProvider", "Name");
			b.ToTable("AspNetUserTokens", (string?)null);
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Assignment", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Instructor", "Instructor").WithMany().HasForeignKey("InstructorId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Instructor");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Attendance", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Instructor", "Instructor").WithMany().HasForeignKey("InstructorId")
				.OnDelete(DeleteBehavior.Restrict);
			b.Navigation("Course");
			b.Navigation("Student");
			b.Navigation("Instructor");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AuditLog", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "User").WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ChatConversation", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "User1").WithMany().HasForeignKey("User1Id")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "User2").WithMany().HasForeignKey("User2Id")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User1");
			b.Navigation("User2");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ChatMessage", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ChatConversation", "Conversation").WithMany("Messages").HasForeignKey("ConversationId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "Sender").WithMany().HasForeignKey("SenderId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Conversation");
			b.Navigation("Sender");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Course", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany("Courses").HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Instructor", "Instructor").WithMany("Courses").HasForeignKey("InstructorId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Department");
			b.Navigation("Instructor");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Enrollment", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany("Enrollments").HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany("Enrollments").HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Exam", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Instructor", "Instructor").WithMany().HasForeignKey("InstructorId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Instructor");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ExamResult", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Exam", "Exam").WithMany().HasForeignKey("ExamId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Exam");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FeePayment", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.FeeStructure", "FeeStructure").WithMany().HasForeignKey("FeeStructureId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("FeeStructure");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FeeStructure", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany().HasForeignKey("DepartmentId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId");
			b.Navigation("Department");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Grade", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Assignment", "Assignment").WithMany().HasForeignKey("AssignmentId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Exam", "Exam").WithMany().HasForeignKey("ExamId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Submission", "Submission").WithMany().HasForeignKey("SubmissionId");
			b.Navigation("Assignment");
			b.Navigation("Course");
			b.Navigation("Exam");
			b.Navigation("Student");
			b.Navigation("Submission");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Notification", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "User").WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Question", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Exam", "Exam").WithMany("Questions").HasForeignKey("ExamId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Exam");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.QuestionOption", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Question", "Question").WithMany("Options").HasForeignKey("QuestionId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Question");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "ApplicationUser").WithOne("Student").HasForeignKey("UniversityManagementSystem.Domain.Entities.Student", "ApplicationUserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany("Students").HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("ApplicationUser");
			b.Navigation("Department");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.StudentAnswer", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Question", "Question").WithMany().HasForeignKey("QuestionId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Question");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Submission", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Assignment", "Assignment").WithMany("Submissions").HasForeignKey("AssignmentId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Assignment");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Instructor", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "ApplicationUser").WithOne("Instructor").HasForeignKey("UniversityManagementSystem.Domain.Entities.Instructor", "ApplicationUserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany("Instructors").HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("ApplicationUser");
			b.Navigation("Department");
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", (EntityTypeBuilder b) =>
		{
			b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null).WithMany().HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", (EntityTypeBuilder b) =>
		{
			b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null).WithMany().HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ApplicationUser", (EntityTypeBuilder b) =>
		{
			b.Navigation("Student");
			b.Navigation("Instructor");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Assignment", (EntityTypeBuilder b) =>
		{
			b.Navigation("Submissions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ChatConversation", (EntityTypeBuilder b) =>
		{
			b.Navigation("Messages");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Course", (EntityTypeBuilder b) =>
		{
			b.Navigation("Enrollments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Department", (EntityTypeBuilder b) =>
		{
			b.Navigation("Courses");
			b.Navigation("Students");
			b.Navigation("Instructors");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Exam", (EntityTypeBuilder b) =>
		{
			b.Navigation("Questions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Question", (EntityTypeBuilder b) =>
		{
			b.Navigation("Options");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.Navigation("Enrollments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Instructor", (EntityTypeBuilder b) =>
		{
			b.Navigation("Courses");
		});
	}
}
