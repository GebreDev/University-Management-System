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
[Migration("20260810100955_AddCourseResults")]
public class AddCourseResults : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("CourseAssessments", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> courseId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> name = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, "");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				CourseId = courseId,
				Name = name,
				Weight = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				Semester = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_CourseAssessments", x => x.Id);
			table.ForeignKey("FK_CourseAssessments_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("CourseResults", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> courseId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> studentId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> semester = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true);
			int? precision = 18;
			int? scale = 2;
			OperationBuilder<AddColumnOperation> totalMark = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale);
			OperationBuilder<AddColumnOperation> letterGrade = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, "");
			scale = 18;
			precision = 2;
			return new
			{
				Id = id,
				CourseId = courseId,
				StudentId = studentId,
				Semester = semester,
				TotalMark = totalMark,
				LetterGrade = letterGrade,
				GradePoint = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, scale, precision),
				Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true, ""),
				IsPublished = table.Column<bool>("bit"),
				PublishedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_CourseResults", x => x.Id);
			table.ForeignKey("FK_CourseResults_Courses_CourseId", x => x.CourseId, "Courses", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_CourseResults_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("AssessmentMarks", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> courseAssessmentId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> studentId = table.Column<int>("int");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				CourseAssessmentId = courseAssessmentId,
				StudentId = studentId,
				Score = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_AssessmentMarks", x => x.Id);
			table.ForeignKey("FK_AssessmentMarks_CourseAssessments_CourseAssessmentId", x => x.CourseAssessmentId, "CourseAssessments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_AssessmentMarks_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateIndex("IX_AssessmentMarks_CourseAssessmentId_StudentId", "AssessmentMarks", new string[2] { "CourseAssessmentId", "StudentId" }, null, unique: true);
		migrationBuilder.CreateIndex("IX_AssessmentMarks_StudentId", "AssessmentMarks", "StudentId");
		migrationBuilder.CreateIndex("IX_CourseAssessments_CourseId", "CourseAssessments", "CourseId");
		migrationBuilder.CreateIndex("IX_CourseResults_CourseId_StudentId_Semester", "CourseResults", new string[3] { "CourseId", "StudentId", "Semester" }, null, unique: true, "[Semester] IS NOT NULL");
		migrationBuilder.CreateIndex("IX_CourseResults_StudentId", "CourseResults", "StudentId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("AssessmentMarks");
		migrationBuilder.DropTable("CourseResults");
		migrationBuilder.DropTable("CourseAssessments");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "10.0.10").HasAnnotation("Relational:MaxIdentifierLength", 128);
		modelBuilder.UseIdentityColumns(1L);
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssessmentMark", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseAssessmentId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<decimal>("Score").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("StudentId");
			b.HasIndex("CourseAssessmentId", "StudentId").IsUnique();
			b.ToTable("AssessmentMarks");
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
			b.Property<int>("InstructorId").HasColumnType("int");
			b.Property<bool>("IsPublished").HasColumnType("bit");
			b.Property<int?>("LateSubmissionPenalty").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("TotalMarks").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("InstructorId");
			b.ToTable("Assignments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentAnswer", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("AnswerText").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("AssignmentQuestionId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<bool>("IsCorrect").HasColumnType("bit");
			b.Property<int?>("Score").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AssignmentQuestionId");
			b.HasIndex("StudentId");
			b.ToTable("AssignmentAnswers");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentQuestion", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AssignmentId").HasColumnType("int");
			b.Property<string>("CorrectAnswer").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("DifficultyLevel").HasColumnType("nvarchar(max)");
			b.Property<string>("Explanation").HasColumnType("nvarchar(max)");
			b.Property<int>("Marks").HasColumnType("int");
			b.Property<string>("QuestionText").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("Type").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AssignmentId");
			b.ToTable("AssignmentQuestions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentQuestionOption", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AssignmentQuestionId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<bool>("IsCorrect").HasColumnType("bit");
			b.Property<string>("OptionText").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AssignmentQuestionId");
			b.ToTable("AssignmentQuestionOptions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentResult", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AssignmentId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
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
			b.HasIndex("AssignmentId");
			b.HasIndex("StudentId");
			b.ToTable("AssignmentResults");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Attendance", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("Date").HasColumnType("datetime2");
			b.Property<int?>("InstructorId").HasColumnType("int");
			b.Property<string>("Remarks").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("InstructorId");
			b.HasIndex("StudentId");
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
			b.Property<int>("InstructorId").HasColumnType("int");
			b.Property<DateTime>("StartDate").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("Code").IsUnique();
			b.HasIndex("DepartmentId");
			b.HasIndex("InstructorId");
			b.ToTable("Courses");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseAssessment", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Name").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int?>("Semester").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<decimal>("Weight").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.ToTable("CourseAssessments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseResult", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<decimal>("GradePoint").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<bool>("IsPublished").HasColumnType("bit");
			b.Property<string>("LetterGrade").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<DateTime?>("PublishedAt").HasColumnType("datetime2");
			b.Property<string>("Remarks").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<int?>("Semester").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<decimal>("TotalMark").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("StudentId");
			b.HasIndex("CourseId", "StudentId", "Semester").IsUnique().HasFilter("[Semester] IS NOT NULL");
			b.ToTable("CourseResults");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseSchedule", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DayOfWeek").HasColumnType("int");
			b.Property<TimeSpan>("EndTime").HasColumnType("time");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<string>("Room").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<TimeSpan>("StartTime").HasColumnType("time");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.ToTable("CourseSchedules");
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
			b.Property<int>("InstructorId").HasColumnType("int");
			b.Property<bool>("IsPublished").HasColumnType("bit");
			b.Property<bool>("IsRandomized").HasColumnType("bit");
			b.Property<int>("PassingMarks").HasColumnType("int");
			b.Property<DateTime>("StartDateTime").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
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
			b.Property<string>("DifficultyLevel").HasColumnType("nvarchar(max)");
			b.Property<int>("ExamId").HasColumnType("int");
			b.Property<string>("Explanation").HasColumnType("nvarchar(max)");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.UniversityEvent", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("Color").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Description").ValueGeneratedOnAdd().HasColumnType("nvarchar(max)")
				.HasDefaultValue("");
			b.Property<DateTime>("EndDateTime").HasColumnType("datetime2");
			b.Property<string>("EventType").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<bool>("IsPublic").HasColumnType("bit");
			b.Property<string>("Location").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("StartDateTime").HasColumnType("datetime2");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("StartDateTime");
			b.ToTable("UniversityEvents");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssessmentMark", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.CourseAssessment", "CourseAssessment").WithMany("Marks").HasForeignKey("CourseAssessmentId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("CourseAssessment");
			b.Navigation("Student");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentAnswer", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AssignmentQuestion", "AssignmentQuestion").WithMany().HasForeignKey("AssignmentQuestionId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("AssignmentQuestion");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentQuestion", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Assignment", "Assignment").WithMany("AssignmentQuestions").HasForeignKey("AssignmentId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Assignment");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentQuestionOption", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AssignmentQuestion", "AssignmentQuestion").WithMany("Options").HasForeignKey("AssignmentQuestionId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("AssignmentQuestion");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentResult", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Assignment", "Assignment").WithMany().HasForeignKey("AssignmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Assignment");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Attendance", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Instructor", "Instructor").WithMany().HasForeignKey("InstructorId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Instructor");
			b.Navigation("Student");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseAssessment", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Course");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseResult", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseSchedule", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Course");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ApplicationUser", (EntityTypeBuilder b) =>
		{
			b.Navigation("Instructor");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Assignment", (EntityTypeBuilder b) =>
		{
			b.Navigation("AssignmentQuestions");
			b.Navigation("Submissions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AssignmentQuestion", (EntityTypeBuilder b) =>
		{
			b.Navigation("Options");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ChatConversation", (EntityTypeBuilder b) =>
		{
			b.Navigation("Messages");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Course", (EntityTypeBuilder b) =>
		{
			b.Navigation("Enrollments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseAssessment", (EntityTypeBuilder b) =>
		{
			b.Navigation("Marks");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Department", (EntityTypeBuilder b) =>
		{
			b.Navigation("Courses");
			b.Navigation("Instructors");
			b.Navigation("Students");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Exam", (EntityTypeBuilder b) =>
		{
			b.Navigation("Questions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Instructor", (EntityTypeBuilder b) =>
		{
			b.Navigation("Courses");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Question", (EntityTypeBuilder b) =>
		{
			b.Navigation("Options");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.Navigation("Enrollments");
		});
	}
}
