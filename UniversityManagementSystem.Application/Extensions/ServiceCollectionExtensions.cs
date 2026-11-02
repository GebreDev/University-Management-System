using AutoMapper;
using UniversityManagementSystem.Application.AutoMapper;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UniversityManagementSystem.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ISemesterOfferedCourseService, SemesterOfferedCourseService>();
            services.AddScoped<ISemesterRegistrationService, SemesterRegistrationService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<ICourseMaterialService, CourseMaterialService>();
            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
