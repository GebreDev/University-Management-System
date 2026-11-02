using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityManagementSystem.Application.Interfaces.Repositories;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Domain.Entities;
using UniversityManagementSystem.Infrastructure.Data;
using UniversityManagementSystem.Infrastructure.Repositories;
using UniversityManagementSystem.Infrastructure.Services;

namespace UniversityManagementSystem.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		string connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<ApplicationDbContext>((DbContextOptionsBuilder options) =>
		{
			options.UseSqlServer(connectionString).ConfigureWarnings((WarningsConfigurationBuilder warnings) =>
			{
				warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
			});
		});
		services.AddHttpClient("Chapa");
		services.AddDefaultIdentity<ApplicationUser>((IdentityOptions options) =>
		{
			options.SignIn.RequireConfirmedAccount = false;
			options.Password.RequireDigit = true;
			options.Password.RequiredLength = 6;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = true;
			options.Password.RequireLowercase = true;
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5L);
			options.Lockout.MaxFailedAccessAttempts = 5;
			options.Lockout.AllowedForNewUsers = true;
		}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
		services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
		services.AddScoped<IDepartmentRepository, DepartmentRepository>();
		services.AddScoped<IInstructorRepository, InstructorRepository>();
		services.AddScoped<IStudentRepository, StudentRepository>();
		services.AddScoped<ICourseRepository, CourseRepository>();
		services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
		services.AddScoped<IAssignmentRepository, AssignmentRepository>();
		services.AddScoped<IAttendanceRepository, AttendanceRepository>();
		services.AddScoped<IAuditLogRepository, AuditLogRepository>();
		services.AddScoped<IChatRepository, ChatRepository>();
		services.AddScoped<IExamRepository, ExamRepository>();
		services.AddScoped<IGradeRepository, GradeRepository>();
		services.AddScoped<INotificationRepository, NotificationRepository>();
		services.AddScoped<ISubmissionRepository, SubmissionRepository>();
		services.AddScoped<IQuestionRepository, QuestionRepository>();
		services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
		services.AddScoped<IExamResultRepository, ExamResultRepository>();
		services.AddScoped<IAssignmentQuestionRepository, AssignmentQuestionRepository>();
		services.AddScoped<IAssignmentAnswerRepository, AssignmentAnswerRepository>();
		services.AddScoped<IAssignmentResultRepository, AssignmentResultRepository>();
		services.AddScoped<IEventRepository, EventRepository>();
		services.AddScoped<ICourseScheduleRepository, CourseScheduleRepository>();
		services.AddScoped<ICourseAssessmentRepository, CourseAssessmentRepository>();
		services.AddScoped<ICourseResultRepository, CourseResultRepository>();
		services.AddScoped<ISemesterRegistrationRepository, SemesterRegistrationRepository>();
		services.AddScoped<ISemesterOfferedCourseRepository, SemesterOfferedCourseRepository>();
		services.AddScoped<IFinanceRepository, FinanceRepository>();
		services.AddScoped<IRegistrationRepository, RegistrationRepository>();
		services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
		services.AddScoped<IEmailSenderService, SmtpEmailSender>();
		return services;
	}
}
