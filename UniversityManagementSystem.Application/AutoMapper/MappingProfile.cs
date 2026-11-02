using AutoMapper;
using UniversityManagementSystem.Application.DTOs.Audit;
using UniversityManagementSystem.Application.DTOs.Assignments;
using UniversityManagementSystem.Application.DTOs.Attendance;
using UniversityManagementSystem.Application.DTOs.Chat;
using UniversityManagementSystem.Application.DTOs.Course;
using UniversityManagementSystem.Application.DTOs.Department;
using UniversityManagementSystem.Application.DTOs.Enrollment;
using UniversityManagementSystem.Application.DTOs.Exams;
using UniversityManagementSystem.Application.DTOs.Grades;
using UniversityManagementSystem.Application.DTOs.Notifications;
using UniversityManagementSystem.Application.DTOs.Student;
using UniversityManagementSystem.Application.DTOs.Instructor;
using UniversityManagementSystem.Application.DTOs.Registration;
using UniversityManagementSystem.Domain.Entities;
using System.Linq;

namespace UniversityManagementSystem.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Department mappings
            CreateMap<Department, DepartmentListDto>()
                .ForMember(dest => dest.InstructorCount, opt => opt.MapFrom(src => src.Instructors.Count))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Students.Count))
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count));

            CreateMap<Department, DepartmentViewDto>()
                .ForMember(dest => dest.Instructors, opt => opt.MapFrom(src => src.Instructors))
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<DepartmentCreateDto, Department>();
            CreateMap<DepartmentUpdateDto, Department>();

            CreateMap<Instructor, DepartmentInstructorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Student, DepartmentStudentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Course, DepartmentCourseDto>();

            // Instructor mappings
            CreateMap<Instructor, InstructorListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count));

            CreateMap<Instructor, InstructorViewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser.UserName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ApplicationUser.ProfilePictureUrl))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses));

            CreateMap<Course, InstructorCourseDto>();

            CreateMap<InstructorCreateDto, Instructor>();
            CreateMap<InstructorUpdateDto, Instructor>();

            // Student mappings
            CreateMap<Student, StudentListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count));

            CreateMap<Student, StudentViewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser.UserName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ApplicationUser.ProfilePictureUrl))
                .ForMember(dest => dest.Enrollments, opt => opt.MapFrom(src => src.Enrollments));

            CreateMap<Enrollment, StudentEnrollmentDto>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<StudentCreateDto, Student>();
            CreateMap<StudentUpdateDto, Student>();

            // Course mappings
            CreateMap<Course, CourseListDto>()
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Instructor.FirstName} {src.Instructor.LastName}"))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Approved)));

            CreateMap<Course, CourseViewDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Instructor.FirstName} {src.Instructor.LastName}"))
                .ForMember(dest => dest.Enrollments, opt => opt.MapFrom(src => src.Enrollments.Where(e => e.Status == Domain.Enums.EnrollmentStatus.Approved)));

            CreateMap<Enrollment, CourseEnrollmentDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.StudentId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CourseCreateDto, Course>();
            CreateMap<CourseUpdateDto, Course>();

            // Enrollment mappings
            CreateMap<Enrollment, EnrollmentListDto>()
                .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.StudentId))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Enrollment, EnrollmentViewDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.Id))           // int FK
                .ForMember(dest => dest.StudentNumber, opt => opt.MapFrom(src => src.Student.StudentId)) // string student number
                .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Course.Id))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<EnrollmentCreateDto, Enrollment>();
            // For update, we'll manually map in service

            // ===== Exam Mappings =====
            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<ExamDto, Exam>()
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.AttachmentPath, opt => opt.MapFrom(src => src.AttachmentPath));

            CreateMap<Question, QuestionDto>();
            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.Options, opt => opt.Ignore());

            CreateMap<QuestionOption, QuestionOptionDto>();
            CreateMap<QuestionOptionDto, QuestionOption>();

            CreateMap<ExamResult, ExamResultDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title));

            // StudentAnswer -> StudentAnswerGradingDto (for essay grading view)
            CreateMap<StudentAnswer, StudentAnswerGradingDto>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.QuestionText))
                .ForMember(dest => dest.QuestionMarks, opt => opt.MapFrom(src => src.Question.Marks))
                .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Question.Type));

            // ===== Assignment Answer/Result Mappings =====
            CreateMap<AssignmentAnswer, AssignmentAnswerDto>();
            CreateMap<AssignmentAnswerDto, AssignmentAnswer>();

            CreateMap<AssignmentAnswer, AssignmentAnswerGradingDto>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.AssignmentQuestion.QuestionText))
                .ForMember(dest => dest.QuestionMarks, opt => opt.MapFrom(src => src.AssignmentQuestion.Marks))
                .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.AssignmentQuestion.Type));

            CreateMap<AssignmentResult, AssignmentResultDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.AssignmentTitle, opt => opt.MapFrom(src => src.Assignment.Title));

            // ===== Assignment Mappings =====
            CreateMap<Assignment, AssignmentDto>()
                .ForMember(dest => dest.SubmissionsCount, opt => opt.MapFrom(src => src.Submissions.Count))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Instructor.FirstName} {src.Instructor.LastName}"))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.AssignmentQuestions))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.AssignmentQuestions.Count));
            CreateMap<AssignmentCreateDto, Assignment>()
                .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore())
                .ForMember(dest => dest.Submissions, opt => opt.Ignore())
                .ForMember(dest => dest.AssignmentQuestions, opt => opt.Ignore());
            CreateMap<AssignmentUpdateDto, Assignment>()
                .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore())
                .ForMember(dest => dest.Submissions, opt => opt.Ignore())
                .ForMember(dest => dest.AssignmentQuestions, opt => opt.Ignore());

            // Assignment Question mappings
            CreateMap<AssignmentQuestion, AssignmentQuestionDto>();
            CreateMap<AssignmentQuestionDto, AssignmentQuestion>()
                .ForMember(dest => dest.Options, opt => opt.Ignore());

            CreateMap<AssignmentQuestionOption, AssignmentQuestionOptionDto>();
            CreateMap<AssignmentQuestionOptionDto, AssignmentQuestionOption>();

            // ===== Attendance Mappings =====
            CreateMap<Attendance, AttendanceListDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<AttendanceCreateDto, Attendance>();
            CreateMap<AttendanceUpdateDto, Attendance>();

            // ===== Chat Mappings =====
            CreateMap<ChatConversation, ConversationDto>()
                .ForMember(dest => dest.OtherUserId, opt => opt.Ignore())
                .ForMember(dest => dest.OtherUserFullName, opt => opt.Ignore())
                .ForMember(dest => dest.OtherUserRole, opt => opt.Ignore())
                .ForMember(dest => dest.UnreadCount, opt => opt.Ignore());

            CreateMap<ChatMessage, MessageDto>()
                .ForMember(dest => dest.SenderName, opt => opt.Ignore())
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId));

            // ===== Notification Mappings =====
            CreateMap<Notification, NotificationDto>();

            // ===== Audit Log Mappings =====
            CreateMap<AuditLog, AuditLogDto>();

            // ===== Registration mappings =====
            CreateMap<SemesterOfferedCourse, SemesterOfferedCourseListDto>();
            CreateMap<SemesterRegistration, SemesterRegistrationDto>();

            // ===== Grade Mappings =====
            CreateMap<Grade, GradeDto>();
            CreateMap<GradeCreateDto, Grade>();
            CreateMap<GradeUpdateDto, Grade>();
        }
    }
}
