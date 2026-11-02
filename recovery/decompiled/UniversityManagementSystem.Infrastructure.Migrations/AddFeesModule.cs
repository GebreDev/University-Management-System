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
[Migration("20260930160359_AddFeesModule")]
public class AddFeesModule : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<int>("PaymentCategory", "Students", "int", null, null, rowVersion: false, null, nullable: false, 0);
		migrationBuilder.CreateTable("FeeStructures", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> academicYearId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> departmentId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> itemName = table.Column<string>("nvarchar(max)");
			int? precision = 18;
			int? scale = 2;
			OperationBuilder<AddColumnOperation> amount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale);
			scale = 18;
			precision = 2;
			return new
			{
				Id = id,
				AcademicYearId = academicYearId,
				DepartmentId = departmentId,
				ItemName = itemName,
				Amount = amount,
				AmountForGovernmentSponsored = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, scale, precision),
				DueDate = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				IsActive = table.Column<bool>("bit"),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_FeeStructures", x => x.Id);
			table.ForeignKey("FK_FeeStructures_AcademicYears_AcademicYearId", x => x.AcademicYearId, "AcademicYears", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_FeeStructures_Departments_DepartmentId", x => x.DepartmentId, "Departments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("FinanceSettings", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> registrationGateEnabled = table.Column<bool>("bit");
			int? precision = 5;
			int? scale = 2;
			OperationBuilder<AddColumnOperation> firstInstallmentPercent = table.Column<decimal>("decimal(5,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale);
			scale = 300;
			return new
			{
				Id = id,
				RegistrationGateEnabled = registrationGateEnabled,
				FirstInstallmentPercent = firstInstallmentPercent,
				ChapaBaseUrl = table.Column<string>("nvarchar(300)", null, scale),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_FinanceSettings", x => x.Id);
		});
		migrationBuilder.CreateTable("Invoices", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> invoiceNumber = table.Column<string>("nvarchar(450)");
			OperationBuilder<AddColumnOperation> studentId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> semesterId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> paymentCategory = table.Column<int>("int");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				InvoiceNumber = invoiceNumber,
				StudentId = studentId,
				SemesterId = semesterId,
				PaymentCategory = paymentCategory,
				TotalAmount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				DueDate = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int"),
				Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Invoices", x => x.Id);
			table.ForeignKey("FK_Invoices_Semesters_SemesterId", x => x.SemesterId, "Semesters", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Invoices_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("InvoiceLines", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> invoiceId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> itemName = table.Column<string>("nvarchar(max)");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				InvoiceId = invoiceId,
				ItemName = itemName,
				Amount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				FeeStructureId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_InvoiceLines", x => x.Id);
			table.ForeignKey("FK_InvoiceLines_FeeStructures_FeeStructureId", x => x.FeeStructureId, "FeeStructures", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_InvoiceLines_Invoices_InvoiceId", x => x.InvoiceId, "Invoices", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Payments", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> reference = table.Column<string>("nvarchar(450)");
			OperationBuilder<AddColumnOperation> gatewayTransactionId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> invoiceId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> studentId = table.Column<int>("int");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				Reference = reference,
				GatewayTransactionId = gatewayTransactionId,
				InvoiceId = invoiceId,
				StudentId = studentId,
				Amount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				Method = table.Column<int>("int"),
				Status = table.Column<int>("int"),
				SlipNumber = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				PayerName = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				Remarks = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				VerifiedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				VerifiedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				CheckoutUrl = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Payments", x => x.Id);
			table.ForeignKey("FK_Payments_Invoices_InvoiceId", x => x.InvoiceId, "Invoices", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_Payments_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Waivers", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> invoiceId = table.Column<int>("int");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				InvoiceId = invoiceId,
				Amount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				Reason = table.Column<string>("nvarchar(max)"),
				GrantedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				GrantedAt = table.Column<DateTime>("datetime2"),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Waivers", x => x.Id);
			table.ForeignKey("FK_Waivers_Invoices_InvoiceId", x => x.InvoiceId, "Invoices", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("LedgerEntries", (ColumnsBuilder table) =>
		{
			OperationBuilder<AddColumnOperation> id = table.Column<int>("int").Annotation("SqlServer:Identity", "1, 1");
			OperationBuilder<AddColumnOperation> studentId = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> invoiceId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> paymentId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> waiverId = table.Column<int>("int", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> entryType = table.Column<string>("nvarchar(max)");
			int? precision = 18;
			int? scale = 2;
			return new
			{
				Id = id,
				StudentId = studentId,
				InvoiceId = invoiceId,
				PaymentId = paymentId,
				WaiverId = waiverId,
				EntryType = entryType,
				Amount = table.Column<decimal>("decimal(18,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale),
				Description = table.Column<string>("nvarchar(max)"),
				CreatedAt = table.Column<DateTime>("datetime2"),
				UpdatedAt = table.Column<DateTime>("datetime2", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_LedgerEntries", x => x.Id);
			table.ForeignKey("FK_LedgerEntries_Invoices_InvoiceId", x => x.InvoiceId, "Invoices", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_LedgerEntries_Payments_PaymentId", x => x.PaymentId, "Payments", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_LedgerEntries_Students_StudentId", x => x.StudentId, "Students", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
			table.ForeignKey("FK_LedgerEntries_Waivers_WaiverId", x => x.WaiverId, "Waivers", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateIndex("IX_FeeStructures_AcademicYearId", "FeeStructures", "AcademicYearId");
		migrationBuilder.CreateIndex("IX_FeeStructures_DepartmentId", "FeeStructures", "DepartmentId");
		migrationBuilder.CreateIndex("IX_InvoiceLines_FeeStructureId", "InvoiceLines", "FeeStructureId");
		migrationBuilder.CreateIndex("IX_InvoiceLines_InvoiceId", "InvoiceLines", "InvoiceId");
		migrationBuilder.CreateIndex("IX_Invoices_InvoiceNumber", "Invoices", "InvoiceNumber", null, unique: true);
		migrationBuilder.CreateIndex("IX_Invoices_SemesterId", "Invoices", "SemesterId");
		migrationBuilder.CreateIndex("IX_Invoices_StudentId_SemesterId", "Invoices", new string[2] { "StudentId", "SemesterId" }, null, unique: true);
		migrationBuilder.CreateIndex("IX_LedgerEntries_InvoiceId", "LedgerEntries", "InvoiceId");
		migrationBuilder.CreateIndex("IX_LedgerEntries_PaymentId", "LedgerEntries", "PaymentId");
		migrationBuilder.CreateIndex("IX_LedgerEntries_StudentId", "LedgerEntries", "StudentId");
		migrationBuilder.CreateIndex("IX_LedgerEntries_WaiverId", "LedgerEntries", "WaiverId");
		migrationBuilder.CreateIndex("IX_Payments_InvoiceId", "Payments", "InvoiceId");
		migrationBuilder.CreateIndex("IX_Payments_Reference", "Payments", "Reference", null, unique: true);
		migrationBuilder.CreateIndex("IX_Payments_StudentId", "Payments", "StudentId");
		migrationBuilder.CreateIndex("IX_Waivers_InvoiceId", "Waivers", "InvoiceId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("FinanceSettings");
		migrationBuilder.DropTable("InvoiceLines");
		migrationBuilder.DropTable("LedgerEntries");
		migrationBuilder.DropTable("FeeStructures");
		migrationBuilder.DropTable("Payments");
		migrationBuilder.DropTable("Waivers");
		migrationBuilder.DropTable("Invoices");
		migrationBuilder.DropColumn("PaymentCategory", "Students");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AcademicGrade", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("Name").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<int>("PromotionOrder").HasColumnType("int");
			b.HasKey("Id");
			b.ToTable("AcademicGrades");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AcademicSection", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("GradeId").HasColumnType("int");
			b.Property<int?>("InstructorId").HasColumnType("int");
			b.Property<string>("Name").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.HasKey("Id");
			b.HasIndex("GradeId");
			b.HasIndex("InstructorId");
			b.ToTable("AcademicSections");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AcademicYear", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("EndDate").HasColumnType("datetime2");
			b.Property<bool>("IsCurrent").HasColumnType("bit");
			b.Property<string>("Name").IsRequired().HasMaxLength(20)
				.HasColumnType("nvarchar(20)");
			b.Property<DateTime>("StartDate").HasColumnType("datetime2");
			b.HasKey("Id");
			b.ToTable("AcademicYears");
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
			b.Property<string>("UserId").HasMaxLength(450).HasColumnType("nvarchar(450)");
			b.Property<string>("UserName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("UserRole").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("Timestamp");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseGrade", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<int>("GradeId").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("GradeId");
			b.ToTable("CourseGrades");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseMaterial", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Description").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
			b.Property<int>("DownloadCount").HasColumnType("int");
			b.Property<string>("FileName").IsRequired().HasMaxLength(255)
				.HasColumnType("nvarchar(255)");
			b.Property<string>("FilePath").IsRequired().HasMaxLength(500)
				.HasColumnType("nvarchar(500)");
			b.Property<long>("FileSize").HasColumnType("bigint");
			b.Property<string>("FileType").IsRequired().HasMaxLength(20)
				.HasColumnType("nvarchar(20)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").IsRequired().HasMaxLength(200)
				.HasColumnType("nvarchar(200)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("UploadedById").IsRequired().HasColumnType("nvarchar(450)");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("UploadedById");
			b.ToTable("CourseMaterials");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FeeStructure", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AcademicYearId").HasColumnType("int");
			b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<decimal>("AmountForGovernmentSponsored").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<DateTime?>("DueDate").HasColumnType("datetime2");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<string>("ItemName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AcademicYearId");
			b.HasIndex("DepartmentId");
			b.ToTable("FeeStructures");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FinanceSetting", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ChapaBaseUrl").IsRequired().HasMaxLength(300)
				.HasColumnType("nvarchar(300)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<decimal>("FirstInstallmentPercent").HasPrecision(5, 2).HasColumnType("decimal(5,2)");
			b.Property<bool>("RegistrationGateEnabled").HasColumnType("bit");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.ToTable("FinanceSettings");
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
			b.Property<DateTime>("DateOfBirth").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<string>("Email").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("FirstName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Gender").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("HireDate").HasColumnType("datetime2");
			b.Property<int?>("KebeleId").HasColumnType("int");
			b.Property<string>("LastName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("MiddleName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Phone").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("ProfilePictureUrl").HasColumnType("nvarchar(max)");
			b.Property<string>("Qualification").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int?>("RegionId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<int?>("WoredaId").HasColumnType("int");
			b.Property<int?>("ZoneId").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("ApplicationUserId").IsUnique();
			b.HasIndex("DepartmentId");
			b.HasIndex("KebeleId");
			b.HasIndex("RegionId");
			b.HasIndex("WoredaId");
			b.HasIndex("ZoneId");
			b.ToTable("Instructors");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Invoice", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime?>("DueDate").HasColumnType("datetime2");
			b.Property<string>("InvoiceNumber").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<int>("PaymentCategory").HasColumnType("int");
			b.Property<string>("Remarks").HasColumnType("nvarchar(max)");
			b.Property<int>("SemesterId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<decimal>("TotalAmount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("InvoiceNumber").IsUnique();
			b.HasIndex("SemesterId");
			b.HasIndex("StudentId", "SemesterId").IsUnique();
			b.ToTable("Invoices");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.InvoiceLine", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int?>("FeeStructureId").HasColumnType("int");
			b.Property<int>("InvoiceId").HasColumnType("int");
			b.Property<string>("ItemName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("FeeStructureId");
			b.HasIndex("InvoiceId");
			b.ToTable("InvoiceLines");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Kebele", (EntityTypeBuilder b) =>
		{
			b.Property<int>("KebeleId").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("KebeleId").UseIdentityColumn(1L);
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<int>("WoredaId").HasColumnType("int");
			b.HasKey("KebeleId");
			b.HasIndex("WoredaId");
			b.ToTable("Kebeles");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.LedgerEntry", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("EntryType").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int?>("InvoiceId").HasColumnType("int");
			b.Property<int?>("PaymentId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<int?>("WaiverId").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("InvoiceId");
			b.HasIndex("PaymentId");
			b.HasIndex("StudentId");
			b.HasIndex("WaiverId");
			b.ToTable("LedgerEntries");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.OfferedCourseSchedule", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DayOfWeek").HasColumnType("int");
			b.Property<TimeSpan>("EndTime").HasColumnType("time");
			b.Property<string>("Room").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("SemesterOfferedCourseId").HasColumnType("int");
			b.Property<TimeSpan>("StartTime").HasColumnType("time");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("SemesterOfferedCourseId");
			b.ToTable("OfferedCourseSchedules");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Parent", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("ApplicationUserId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("EconomicStatus").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Email").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("FirstName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("LastName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("MiddleName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Nationality").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Phone").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("ProfilePictureUrl").HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<string>("Username").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("ApplicationUserId").IsUnique();
			b.HasIndex("StudentId");
			b.ToTable("Parents");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Payment", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<string>("CheckoutUrl").HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<string>("GatewayTransactionId").HasColumnType("nvarchar(max)");
			b.Property<int>("InvoiceId").HasColumnType("int");
			b.Property<int>("Method").HasColumnType("int");
			b.Property<string>("PayerName").HasColumnType("nvarchar(max)");
			b.Property<string>("Reference").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<string>("Remarks").HasColumnType("nvarchar(max)");
			b.Property<string>("SlipNumber").HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<DateTime?>("VerifiedAt").HasColumnType("datetime2");
			b.Property<string>("VerifiedByUserId").HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("InvoiceId");
			b.HasIndex("Reference").IsUnique();
			b.HasIndex("StudentId");
			b.ToTable("Payments");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Period", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("Description").HasMaxLength(100).HasColumnType("nvarchar(100)");
			b.Property<int>("DisplayOrder").HasColumnType("int");
			b.Property<TimeOnly>("EndTime").HasColumnType("time");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<string>("Name").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<TimeOnly>("StartTime").HasColumnType("time");
			b.HasKey("Id");
			b.ToTable("Periods");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Region", (EntityTypeBuilder b) =>
		{
			b.Property<int>("RegionId").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("RegionId").UseIdentityColumn(1L);
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.HasKey("RegionId");
			b.ToTable("Regions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Registration", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<string>("RejectionReason").HasColumnType("nvarchar(max)");
			b.Property<DateTime?>("ReviewedAt").HasColumnType("datetime2");
			b.Property<string>("ReviewedBy").HasColumnType("nvarchar(max)");
			b.Property<int>("SemesterId").HasColumnType("int");
			b.Property<int?>("SemesterOfferedCourseId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<int>("StudentId").HasColumnType("int");
			b.Property<int>("TotalCredits").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("DepartmentId");
			b.HasIndex("SemesterId");
			b.HasIndex("SemesterOfferedCourseId");
			b.HasIndex("StudentId");
			b.ToTable("Registrations");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.RegistrationItem", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("Credits").HasColumnType("int");
			b.Property<string>("Note").HasColumnType("nvarchar(max)");
			b.Property<int>("RegistrationId").HasColumnType("int");
			b.Property<int>("SemesterOfferedCourseId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("RegistrationId");
			b.HasIndex("SemesterOfferedCourseId");
			b.ToTable("RegistrationItems");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.RegistrationStatusHistory", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<DateTime>("ChangedAt").HasColumnType("datetime2");
			b.Property<string>("ChangedBy").HasColumnType("nvarchar(max)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("NewStatus").HasColumnType("int");
			b.Property<int>("OldStatus").HasColumnType("int");
			b.Property<string>("Reason").HasColumnType("nvarchar(max)");
			b.Property<int>("RegistrationId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("RegistrationId");
			b.ToTable("RegistrationStatusHistories");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.RolePermission", (EntityTypeBuilder b) =>
		{
			b.Property<int>("RolePermissionId").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("RolePermissionId").UseIdentityColumn(1L);
			b.Property<string>("Permission").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("RoleId").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("RolePermissionId");
			b.ToTable("RolePermissions");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Semester", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AcademicYearId").HasColumnType("int");
			b.Property<DateTime>("EndDate").HasColumnType("datetime2");
			b.Property<string>("Name").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<DateTime>("StartDate").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AcademicYearId");
			b.ToTable("Semesters");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.SemesterOfferedCourse", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("Capacity").HasColumnType("int");
			b.Property<int>("CourseId").HasColumnType("int");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<bool>("IsOpen").HasColumnType("bit");
			b.Property<int>("SemesterId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("CourseId");
			b.HasIndex("DepartmentId");
			b.HasIndex("SemesterId");
			b.ToTable("SemesterOfferedCourses");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int?>("AcademicGradeId").HasColumnType("int");
			b.Property<int?>("AcademicSectionId").HasColumnType("int");
			b.Property<string>("ApplicationUserId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("DateOfBirth").HasColumnType("datetime2");
			b.Property<int>("DepartmentId").HasColumnType("int");
			b.Property<string>("Email").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTime>("EnrollmentDate").HasColumnType("datetime2");
			b.Property<string>("FirstName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Gender").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int?>("KebeleId").HasColumnType("int");
			b.Property<string>("LastName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("MiddleName").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Nationality").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("PaymentCategory").HasColumnType("int");
			b.Property<string>("Phone").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("ProfilePictureUrl").HasColumnType("nvarchar(max)");
			b.Property<int?>("RegionId").HasColumnType("int");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("StudentDocumentPath").HasColumnType("nvarchar(max)");
			b.Property<string>("StudentId").IsRequired().HasColumnType("nvarchar(450)");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.Property<int?>("WoredaId").HasColumnType("int");
			b.Property<int?>("ZoneId").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("AcademicGradeId");
			b.HasIndex("AcademicSectionId");
			b.HasIndex("ApplicationUserId").IsUnique();
			b.HasIndex("DepartmentId");
			b.HasIndex("KebeleId");
			b.HasIndex("RegionId");
			b.HasIndex("StudentId").IsUnique();
			b.HasIndex("WoredaId");
			b.HasIndex("ZoneId");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.TimetableEntry", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<int>("AcademicYearId").HasColumnType("int");
			b.Property<string>("Day").IsRequired().HasMaxLength(20)
				.HasColumnType("nvarchar(20)");
			b.Property<int>("GradeId").HasColumnType("int");
			b.Property<string>("Instructor").HasMaxLength(100).HasColumnType("nvarchar(100)");
			b.Property<int>("PeriodId").HasColumnType("int");
			b.Property<string>("Room").HasMaxLength(50).HasColumnType("nvarchar(50)");
			b.Property<int>("SectionId").HasColumnType("int");
			b.Property<int>("SemesterId").HasColumnType("int");
			b.Property<string>("Subject").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.HasKey("Id");
			b.HasIndex("AcademicYearId");
			b.HasIndex("GradeId");
			b.HasIndex("PeriodId");
			b.HasIndex("SectionId");
			b.HasIndex("SemesterId");
			b.ToTable("TimetableEntries");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.TimetableSetting", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<TimeOnly>("DayEndTime").HasColumnType("time");
			b.Property<TimeOnly>("DayStartTime").HasColumnType("time");
			b.Property<bool>("FridayEnabled").HasColumnType("bit");
			b.Property<string>("FridayLabel").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<int>("LunchBreakDurationMinutes").HasColumnType("int");
			b.Property<TimeOnly?>("LunchBreakEnd").HasColumnType("time");
			b.Property<TimeOnly?>("LunchBreakStart").HasColumnType("time");
			b.Property<bool>("MondayEnabled").HasColumnType("bit");
			b.Property<string>("MondayLabel").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<int>("PeriodsPerDay").HasColumnType("int");
			b.Property<bool>("SaturdayEnabled").HasColumnType("bit");
			b.Property<string>("SaturdayLabel").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<bool>("SundayEnabled").HasColumnType("bit");
			b.Property<bool>("ThursdayEnabled").HasColumnType("bit");
			b.Property<string>("ThursdayLabel").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<bool>("TuesdayEnabled").HasColumnType("bit");
			b.Property<string>("TuesdayLabel").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<string>("UniversityName").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<bool>("WednesdayEnabled").HasColumnType("bit");
			b.Property<string>("WednesdayLabel").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<int>("WorkingDaysPerWeek").HasColumnType("int");
			b.HasKey("Id");
			b.ToTable("TimetableSettings");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.UniversitySetting", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<string>("DetailedAddress").HasMaxLength(500).HasColumnType("nvarchar(500)");
			b.Property<string>("Email").HasMaxLength(150).HasColumnType("nvarchar(150)");
			b.Property<int?>("KebeleId").HasColumnType("int");
			b.Property<string>("Phone").HasMaxLength(30).HasColumnType("nvarchar(30)");
			b.Property<int?>("RegionId").HasColumnType("int");
			b.Property<string>("UniversityName").IsRequired().HasMaxLength(200)
				.HasColumnType("nvarchar(200)");
			b.Property<int?>("WoredaId").HasColumnType("int");
			b.Property<int?>("ZoneId").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("KebeleId");
			b.HasIndex("RegionId");
			b.HasIndex("WoredaId");
			b.HasIndex("ZoneId");
			b.ToTable("UniversitySettings");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Waiver", (EntityTypeBuilder b) =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("Id").UseIdentityColumn(1L);
			b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
			b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
			b.Property<DateTime>("GrantedAt").HasColumnType("datetime2");
			b.Property<string>("GrantedByUserId").HasColumnType("nvarchar(max)");
			b.Property<int>("InvoiceId").HasColumnType("int");
			b.Property<string>("Reason").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTime?>("UpdatedAt").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("InvoiceId");
			b.ToTable("Waivers");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Woreda", (EntityTypeBuilder b) =>
		{
			b.Property<int>("WoredaId").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("WoredaId").UseIdentityColumn(1L);
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<int>("ZoneId").HasColumnType("int");
			b.HasKey("WoredaId");
			b.HasIndex("ZoneId");
			b.ToTable("Woredas");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Zone", (EntityTypeBuilder b) =>
		{
			b.Property<int>("ZoneId").ValueGeneratedOnAdd().HasColumnType("int");
			b.Property<int>("ZoneId").UseIdentityColumn(1L);
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<int>("RegionId").HasColumnType("int");
			b.HasKey("ZoneId");
			b.HasIndex("RegionId");
			b.ToTable("Zones");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AcademicSection", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicGrade", "Grade").WithMany("Sections").HasForeignKey("GradeId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Instructor", "Instructor").WithMany().HasForeignKey("InstructorId");
			b.Navigation("Grade");
			b.Navigation("Instructor");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseGrade", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany("CourseGrades").HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicGrade", "Grade").WithMany().HasForeignKey("GradeId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Grade");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.CourseMaterial", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "UploadedBy").WithMany().HasForeignKey("UploadedById")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("UploadedBy");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.FeeStructure", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicYear", "AcademicYear").WithMany().HasForeignKey("AcademicYearId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany().HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("AcademicYear");
			b.Navigation("Department");
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
			b.HasOne("UniversityManagementSystem.Domain.Entities.Kebele", "Kebele").WithMany().HasForeignKey("KebeleId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Region", "Region").WithMany().HasForeignKey("RegionId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Woreda", "Woreda").WithMany().HasForeignKey("WoredaId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Zone", "Zone").WithMany().HasForeignKey("ZoneId")
				.OnDelete(DeleteBehavior.Restrict);
			b.Navigation("ApplicationUser");
			b.Navigation("Department");
			b.Navigation("Kebele");
			b.Navigation("Region");
			b.Navigation("Woreda");
			b.Navigation("Zone");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Invoice", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Semester", "Semester").WithMany().HasForeignKey("SemesterId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Semester");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.InvoiceLine", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.FeeStructure", "FeeStructure").WithMany().HasForeignKey("FeeStructureId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Invoice", "Invoice").WithMany("Lines").HasForeignKey("InvoiceId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("FeeStructure");
			b.Navigation("Invoice");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Kebele", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Woreda", "Woreda").WithMany("Kebeles").HasForeignKey("WoredaId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Woreda");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.LedgerEntry", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Invoice", "Invoice").WithMany().HasForeignKey("InvoiceId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Payment", "Payment").WithMany().HasForeignKey("PaymentId")
				.OnDelete(DeleteBehavior.Restrict);
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Waiver", "Waiver").WithMany().HasForeignKey("WaiverId")
				.OnDelete(DeleteBehavior.Restrict);
			b.Navigation("Invoice");
			b.Navigation("Payment");
			b.Navigation("Student");
			b.Navigation("Waiver");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Notification", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "User").WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.OfferedCourseSchedule", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.SemesterOfferedCourse", "SemesterOfferedCourse").WithMany("Schedules").HasForeignKey("SemesterOfferedCourseId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("SemesterOfferedCourse");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Parent", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "ApplicationUser").WithOne("Parent").HasForeignKey("UniversityManagementSystem.Domain.Entities.Parent", "ApplicationUserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany("Parents").HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("ApplicationUser");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Payment", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Invoice", "Invoice").WithMany("Payments").HasForeignKey("InvoiceId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Invoice");
			b.Navigation("Student");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Registration", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany().HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Semester", "Semester").WithMany().HasForeignKey("SemesterId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.SemesterOfferedCourse", null).WithMany("Registrations").HasForeignKey("SemesterOfferedCourseId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Student", "Student").WithMany().HasForeignKey("StudentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Department");
			b.Navigation("Semester");
			b.Navigation("Student");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.RegistrationItem", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Registration", "Registration").WithMany("Items").HasForeignKey("RegistrationId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.SemesterOfferedCourse", "SemesterOfferedCourse").WithMany().HasForeignKey("SemesterOfferedCourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Registration");
			b.Navigation("SemesterOfferedCourse");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.RegistrationStatusHistory", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Registration", "Registration").WithMany("StatusHistory").HasForeignKey("RegistrationId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Registration");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Semester", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicYear", "AcademicYear").WithMany().HasForeignKey("AcademicYearId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("AcademicYear");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.SemesterOfferedCourse", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Course", "Course").WithMany().HasForeignKey("CourseId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany().HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Semester", "Semester").WithMany().HasForeignKey("SemesterId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Course");
			b.Navigation("Department");
			b.Navigation("Semester");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicGrade", "AcademicGrade").WithMany().HasForeignKey("AcademicGradeId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicSection", "AcademicSection").WithMany().HasForeignKey("AcademicSectionId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.ApplicationUser", "ApplicationUser").WithOne("Student").HasForeignKey("UniversityManagementSystem.Domain.Entities.Student", "ApplicationUserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Department", "Department").WithMany("Students").HasForeignKey("DepartmentId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Kebele", "Kebele").WithMany().HasForeignKey("KebeleId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Region", "Region").WithMany().HasForeignKey("RegionId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Woreda", "Woreda").WithMany().HasForeignKey("WoredaId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Zone", "Zone").WithMany().HasForeignKey("ZoneId");
			b.Navigation("AcademicGrade");
			b.Navigation("AcademicSection");
			b.Navigation("ApplicationUser");
			b.Navigation("Department");
			b.Navigation("Kebele");
			b.Navigation("Region");
			b.Navigation("Woreda");
			b.Navigation("Zone");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.TimetableEntry", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicYear", "AcademicYear").WithMany().HasForeignKey("AcademicYearId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicGrade", "Grade").WithMany().HasForeignKey("GradeId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Period", "Period").WithMany().HasForeignKey("PeriodId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.AcademicSection", "Section").WithMany().HasForeignKey("SectionId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.HasOne("UniversityManagementSystem.Domain.Entities.Semester", "Semester").WithMany().HasForeignKey("SemesterId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("AcademicYear");
			b.Navigation("Grade");
			b.Navigation("Period");
			b.Navigation("Section");
			b.Navigation("Semester");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.UniversitySetting", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Kebele", "Kebele").WithMany().HasForeignKey("KebeleId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Region", "Region").WithMany().HasForeignKey("RegionId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Woreda", "Woreda").WithMany().HasForeignKey("WoredaId");
			b.HasOne("UniversityManagementSystem.Domain.Entities.Zone", "Zone").WithMany().HasForeignKey("ZoneId");
			b.Navigation("Kebele");
			b.Navigation("Region");
			b.Navigation("Woreda");
			b.Navigation("Zone");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Waiver", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Invoice", "Invoice").WithMany("Waivers").HasForeignKey("InvoiceId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Invoice");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Woreda", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Zone", "Zone").WithMany("Woredas").HasForeignKey("ZoneId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Zone");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Zone", (EntityTypeBuilder b) =>
		{
			b.HasOne("UniversityManagementSystem.Domain.Entities.Region", "Region").WithMany("Zones").HasForeignKey("RegionId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("Region");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.AcademicGrade", (EntityTypeBuilder b) =>
		{
			b.Navigation("Sections");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.ApplicationUser", (EntityTypeBuilder b) =>
		{
			b.Navigation("Instructor");
			b.Navigation("Parent");
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
			b.Navigation("CourseGrades");
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
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Invoice", (EntityTypeBuilder b) =>
		{
			b.Navigation("Lines");
			b.Navigation("Payments");
			b.Navigation("Waivers");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Question", (EntityTypeBuilder b) =>
		{
			b.Navigation("Options");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Region", (EntityTypeBuilder b) =>
		{
			b.Navigation("Zones");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Registration", (EntityTypeBuilder b) =>
		{
			b.Navigation("Items");
			b.Navigation("StatusHistory");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.SemesterOfferedCourse", (EntityTypeBuilder b) =>
		{
			b.Navigation("Registrations");
			b.Navigation("Schedules");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Student", (EntityTypeBuilder b) =>
		{
			b.Navigation("Enrollments");
			b.Navigation("Parents");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Woreda", (EntityTypeBuilder b) =>
		{
			b.Navigation("Kebeles");
		});
		modelBuilder.Entity("UniversityManagementSystem.Domain.Entities.Zone", (EntityTypeBuilder b) =>
		{
			b.Navigation("Woredas");
		});
	}
}
