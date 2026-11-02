using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Entities;

namespace UniversityManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public DbSet<Department> Departments { get; set; }

	public DbSet<Instructor> Instructors { get; set; }

	public DbSet<Student> Students { get; set; }

	public DbSet<Course> Courses { get; set; }

	public DbSet<Enrollment> Enrollments { get; set; }

	public DbSet<SemesterOfferedCourse> SemesterOfferedCourses { get; set; }

	public DbSet<OfferedCourseSchedule> OfferedCourseSchedules { get; set; }

	public DbSet<Registration> Registrations { get; set; }

	public DbSet<RegistrationItem> RegistrationItems { get; set; }

	public DbSet<RegistrationStatusHistory> RegistrationStatusHistories { get; set; }

	public DbSet<ChatConversation> ChatConversations { get; set; }

	public DbSet<ChatMessage> ChatMessages { get; set; }

	public DbSet<Attendance> Attendances { get; set; }

	public DbSet<Exam> Exams { get; set; }

	public DbSet<Question> Questions { get; set; }

	public DbSet<QuestionOption> QuestionOptions { get; set; }

	public DbSet<StudentAnswer> StudentAnswers { get; set; }

	public DbSet<ExamResult> ExamResults { get; set; }

	public DbSet<Assignment> Assignments { get; set; }

	public DbSet<Submission> Submissions { get; set; }

	public DbSet<AssignmentQuestion> AssignmentQuestions { get; set; }

	public DbSet<AssignmentQuestionOption> AssignmentQuestionOptions { get; set; }

	public DbSet<AssignmentAnswer> AssignmentAnswers { get; set; }

	public DbSet<AssignmentResult> AssignmentResults { get; set; }

	public DbSet<Grade> Grades { get; set; }

	public DbSet<Notification> Notifications { get; set; }

	public DbSet<AuditLog> AuditLogs { get; set; }

	public DbSet<UniversityEvent> UniversityEvents { get; set; }

	public DbSet<CourseSchedule> CourseSchedules { get; set; }

	public DbSet<CourseAssessment> CourseAssessments { get; set; }

	public DbSet<AssessmentMark> AssessmentMarks { get; set; }

	public DbSet<CourseResult> CourseResults { get; set; }

	public DbSet<CourseGrade> CourseGrades { get; set; }

	public DbSet<CourseMaterial> CourseMaterials { get; set; }

	public DbSet<Region> Regions { get; set; }

	public DbSet<Zone> Zones { get; set; }

	public DbSet<Woreda> Woredas { get; set; }

	public DbSet<Kebele> Kebeles { get; set; }

	public DbSet<UniversitySetting> UniversitySettings { get; set; }

	public DbSet<AcademicYear> AcademicYears { get; set; }

	public DbSet<Semester> Semesters { get; set; }

	public DbSet<AcademicGrade> AcademicGrades { get; set; }

	public DbSet<AcademicSection> AcademicSections { get; set; }

	public DbSet<TimetableEntry> TimetableEntries { get; set; }

	public DbSet<Period> Periods { get; set; }

	public DbSet<TimetableSetting> TimetableSettings { get; set; }

	public DbSet<Parent> Parents { get; set; }

	public DbSet<FeeStructure> FeeStructures { get; set; }

	public DbSet<Invoice> Invoices { get; set; }

	public DbSet<InvoiceLine> InvoiceLines { get; set; }

	public DbSet<Payment> Payments { get; set; }

	public DbSet<Waiver> Waivers { get; set; }

	public DbSet<LedgerEntry> LedgerEntries { get; set; }

	public DbSet<FinanceSetting> FinanceSettings { get; set; }

	public DbSet<RolePermission> RolePermissions { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base((DbContextOptions)options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.Entity<Instructor>().HasOne((Instructor t) => t.ApplicationUser).WithOne((ApplicationUser u) => u.Instructor)
			.HasForeignKey((Instructor t) => t.ApplicationUserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Student>().HasOne((Student s) => s.ApplicationUser).WithOne((ApplicationUser u) => u.Student)
			.HasForeignKey((Student s) => s.ApplicationUserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Instructor>().HasOne((Instructor t) => t.Department).WithMany((Department d) => d.Instructors)
			.HasForeignKey((Instructor t) => t.DepartmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Student>().HasOne((Student s) => s.Department).WithMany((Department d) => d.Students)
			.HasForeignKey((Student s) => s.DepartmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Course>().HasOne((Course c) => c.Department).WithMany((Department d) => d.Courses)
			.HasForeignKey((Course c) => c.DepartmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Course>().HasOne((Course c) => c.Instructor).WithMany((Instructor t) => t.Courses)
			.HasForeignKey((Course c) => c.InstructorId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Enrollment>().HasOne((Enrollment e) => e.Student).WithMany((Student s) => s.Enrollments)
			.HasForeignKey((Enrollment e) => e.StudentId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Enrollment>().HasOne((Enrollment e) => e.Course).WithMany((Course c) => c.Enrollments)
			.HasForeignKey((Enrollment e) => e.CourseId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<SemesterOfferedCourse>().HasOne((SemesterOfferedCourse o) => o.Semester).WithMany()
			.HasForeignKey((SemesterOfferedCourse o) => o.SemesterId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<SemesterOfferedCourse>().HasOne((SemesterOfferedCourse o) => o.Course).WithMany()
			.HasForeignKey((SemesterOfferedCourse o) => o.CourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<SemesterOfferedCourse>().HasOne((SemesterOfferedCourse o) => o.Department).WithMany()
			.HasForeignKey((SemesterOfferedCourse o) => o.DepartmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<OfferedCourseSchedule>().HasOne((OfferedCourseSchedule s) => s.SemesterOfferedCourse).WithMany((SemesterOfferedCourse o) => o.Schedules)
			.HasForeignKey((OfferedCourseSchedule s) => s.SemesterOfferedCourseId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Registration>().HasOne((Registration r) => r.Student).WithMany()
			.HasForeignKey((Registration r) => r.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Registration>().HasOne((Registration r) => r.Semester).WithMany()
			.HasForeignKey((Registration r) => r.SemesterId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Registration>().HasOne((Registration r) => r.Department).WithMany()
			.HasForeignKey((Registration r) => r.DepartmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<RegistrationItem>().HasOne((RegistrationItem i) => i.Registration).WithMany((Registration r) => r.Items)
			.HasForeignKey((RegistrationItem i) => i.RegistrationId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<RegistrationItem>().HasOne((RegistrationItem i) => i.SemesterOfferedCourse).WithMany()
			.HasForeignKey((RegistrationItem i) => i.SemesterOfferedCourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<RegistrationStatusHistory>().HasOne((RegistrationStatusHistory h) => h.Registration).WithMany((Registration r) => r.StatusHistory)
			.HasForeignKey((RegistrationStatusHistory h) => h.RegistrationId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Student>().HasIndex((Student s) => s.StudentId).IsUnique();
		builder.Entity<Course>().HasIndex((Course c) => c.Code).IsUnique();
		builder.Entity<ChatConversation>().HasOne((ChatConversation c) => c.User1).WithMany()
			.HasForeignKey((ChatConversation c) => c.User1Id)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<ChatConversation>().HasOne((ChatConversation c) => c.User2).WithMany()
			.HasForeignKey((ChatConversation c) => c.User2Id)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<ChatConversation>().Property((ChatConversation c) => c.LastMessage).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<ChatMessage>().Property((ChatMessage m) => m.AttachmentPath).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<ChatMessage>().Property((ChatMessage m) => m.AttachmentType).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<ChatMessage>().Property((ChatMessage m) => m.Content).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<ChatMessage>().HasOne((ChatMessage m) => m.Conversation).WithMany((ChatConversation c) => c.Messages)
			.HasForeignKey((ChatMessage m) => m.ConversationId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<ChatMessage>().HasOne((ChatMessage m) => m.Sender).WithMany()
			.HasForeignKey((ChatMessage m) => m.SenderId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Attendance>().Property((Attendance a) => a.Remarks).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Attendance>().HasOne((Attendance a) => a.Student).WithMany()
			.HasForeignKey((Attendance a) => a.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Attendance>().HasOne((Attendance a) => a.Course).WithMany()
			.HasForeignKey((Attendance a) => a.CourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Attendance>().HasOne((Attendance a) => a.Instructor).WithMany()
			.HasForeignKey((Attendance a) => a.InstructorId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Exam>().Property((Exam e) => e.AttachmentPath).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Exam>().Property((Exam e) => e.Instructions).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Exam>().HasOne((Exam e) => e.Course).WithMany()
			.HasForeignKey((Exam e) => e.CourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Exam>().HasOne((Exam e) => e.Instructor).WithMany()
			.HasForeignKey((Exam e) => e.InstructorId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Question>().HasOne((Question q) => q.Exam).WithMany((Exam e) => e.Questions)
			.HasForeignKey((Question q) => q.ExamId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<QuestionOption>().HasOne((QuestionOption o) => o.Question).WithMany((Question q) => q.Options)
			.HasForeignKey((QuestionOption o) => o.QuestionId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<StudentAnswer>().Property((StudentAnswer sa) => sa.AnswerText).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<StudentAnswer>().HasOne((StudentAnswer sa) => sa.Student).WithMany()
			.HasForeignKey((StudentAnswer sa) => sa.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<StudentAnswer>().HasOne((StudentAnswer sa) => sa.Question).WithMany()
			.HasForeignKey((StudentAnswer sa) => sa.QuestionId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<ExamResult>().Property((ExamResult er) => er.Grade).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<ExamResult>().Property((ExamResult er) => er.Remarks).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<ExamResult>().HasOne((ExamResult er) => er.Student).WithMany()
			.HasForeignKey((ExamResult er) => er.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<ExamResult>().HasOne((ExamResult er) => er.Exam).WithMany()
			.HasForeignKey((ExamResult er) => er.ExamId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Assignment>().Property((Assignment a) => a.AttachmentPath).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Assignment>().HasOne((Assignment a) => a.Course).WithMany()
			.HasForeignKey((Assignment a) => a.CourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Assignment>().HasOne((Assignment a) => a.Instructor).WithMany()
			.HasForeignKey((Assignment a) => a.InstructorId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AssignmentQuestion>().HasOne((AssignmentQuestion q) => q.Assignment).WithMany((Assignment a) => a.AssignmentQuestions)
			.HasForeignKey((AssignmentQuestion q) => q.AssignmentId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<AssignmentQuestionOption>().HasOne((AssignmentQuestionOption o) => o.AssignmentQuestion).WithMany((AssignmentQuestion q) => q.Options)
			.HasForeignKey((AssignmentQuestionOption o) => o.AssignmentQuestionId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<AssignmentAnswer>().Property((AssignmentAnswer aa) => aa.AnswerText).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<AssignmentAnswer>().HasOne((AssignmentAnswer aa) => aa.Student).WithMany()
			.HasForeignKey((AssignmentAnswer aa) => aa.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AssignmentAnswer>().HasOne((AssignmentAnswer aa) => aa.AssignmentQuestion).WithMany()
			.HasForeignKey((AssignmentAnswer aa) => aa.AssignmentQuestionId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AssignmentResult>().Property((AssignmentResult ar) => ar.Grade).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<AssignmentResult>().Property((AssignmentResult ar) => ar.Remarks).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<AssignmentResult>().HasOne((AssignmentResult ar) => ar.Student).WithMany()
			.HasForeignKey((AssignmentResult ar) => ar.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AssignmentResult>().HasOne((AssignmentResult ar) => ar.Assignment).WithMany()
			.HasForeignKey((AssignmentResult ar) => ar.AssignmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Submission>().Property((Submission s) => s.Feedback).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Submission>().Property((Submission s) => s.FilePath).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Submission>().HasOne((Submission s) => s.Assignment).WithMany((Assignment a) => a.Submissions)
			.HasForeignKey((Submission s) => s.AssignmentId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Submission>().HasOne((Submission s) => s.Student).WithMany()
			.HasForeignKey((Submission s) => s.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Grade>().Property((Grade g) => g.LetterGrade).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Grade>().Property((Grade g) => g.Remarks).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Grade>().HasOne((Grade g) => g.Student).WithMany()
			.HasForeignKey((Grade g) => g.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Grade>().HasOne((Grade g) => g.Course).WithMany()
			.HasForeignKey((Grade g) => g.CourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Notification>().Property((Notification n) => n.RelatedId).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Notification>().Property((Notification n) => n.LinkUrl).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Notification>().HasOne((Notification n) => n.User).WithMany()
			.HasForeignKey((Notification n) => n.UserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AuditLog>().Property((AuditLog al) => al.Details).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<AuditLog>().Property((AuditLog al) => al.IpAddress).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<AuditLog>().Property((AuditLog al) => al.DeviceInfo).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<AuditLog>().Property((AuditLog al) => al.UserId).HasMaxLength(450);
		builder.Entity<AuditLog>().HasIndex((AuditLog al) => al.UserId);
		builder.Entity<AuditLog>().HasIndex((AuditLog al) => al.Timestamp);
		builder.Entity<UniversityEvent>().Property((UniversityEvent e) => e.Description).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<UniversityEvent>().HasIndex((UniversityEvent e) => e.StartDateTime);
		builder.Entity<CourseSchedule>().HasOne((CourseSchedule s) => s.Course).WithMany()
			.HasForeignKey((CourseSchedule s) => s.CourseId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<CourseSchedule>().Property((CourseSchedule s) => s.Room).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<CourseSchedule>().HasIndex((CourseSchedule s) => s.CourseId);
		builder.Entity<CourseAssessment>().Property((CourseAssessment ca) => ca.Name).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<CourseAssessment>().Property((CourseAssessment ca) => ca.Weight).HasPrecision(18, 2);
		builder.Entity<CourseAssessment>().HasOne((CourseAssessment ca) => ca.Course).WithMany()
			.HasForeignKey((CourseAssessment ca) => ca.CourseId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<CourseAssessment>().HasIndex((CourseAssessment ca) => ca.CourseId);
		builder.Entity<AssessmentMark>().HasOne((AssessmentMark m) => m.CourseAssessment).WithMany((CourseAssessment ca) => ca.Marks)
			.HasForeignKey((AssessmentMark m) => m.CourseAssessmentId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<AssessmentMark>().HasOne((AssessmentMark m) => m.Student).WithMany()
			.HasForeignKey((AssessmentMark m) => m.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AssessmentMark>().HasIndex((AssessmentMark m) => new { m.CourseAssessmentId, m.StudentId }).IsUnique();
		builder.Entity<AssessmentMark>().Property((AssessmentMark m) => m.Score).HasPrecision(18, 2);
		builder.Entity<CourseResult>().HasOne((CourseResult r) => r.Course).WithMany()
			.HasForeignKey((CourseResult r) => r.CourseId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<CourseResult>().HasOne((CourseResult r) => r.Student).WithMany()
			.HasForeignKey((CourseResult r) => r.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<CourseResult>().HasIndex((CourseResult r) => new { r.CourseId, r.StudentId, r.Semester }).IsUnique();
		builder.Entity<CourseResult>().Property((CourseResult r) => r.TotalMark).HasPrecision(18, 2);
		builder.Entity<CourseResult>().Property((CourseResult r) => r.GradePoint).HasPrecision(18, 2);
		builder.Entity<CourseResult>().Property((CourseResult r) => r.LetterGrade).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<CourseResult>().Property((CourseResult r) => r.Remarks).HasDefaultValue(string.Empty)
			.IsRequired(required: false);
		builder.Entity<Zone>().HasOne((Zone z) => z.Region).WithMany((Region r) => r.Zones)
			.HasForeignKey((Zone z) => z.RegionId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Woreda>().HasOne((Woreda w) => w.Zone).WithMany((Zone z) => z.Woredas)
			.HasForeignKey((Woreda w) => w.ZoneId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Kebele>().HasOne((Kebele k) => k.Woreda).WithMany((Woreda w) => w.Kebeles)
			.HasForeignKey((Kebele k) => k.WoredaId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Semester>().HasOne((Semester s) => s.AcademicYear).WithMany()
			.HasForeignKey((Semester s) => s.AcademicYearId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<AcademicSection>().HasOne((AcademicSection s) => s.Grade).WithMany((AcademicGrade g) => g.Sections)
			.HasForeignKey((AcademicSection s) => s.GradeId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<TimetableEntry>().HasOne((TimetableEntry t) => t.AcademicYear).WithMany()
			.HasForeignKey((TimetableEntry t) => t.AcademicYearId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<TimetableEntry>().HasOne((TimetableEntry t) => t.Semester).WithMany()
			.HasForeignKey((TimetableEntry t) => t.SemesterId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<TimetableEntry>().HasOne((TimetableEntry t) => t.Grade).WithMany()
			.HasForeignKey((TimetableEntry t) => t.GradeId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<TimetableEntry>().HasOne((TimetableEntry t) => t.Section).WithMany()
			.HasForeignKey((TimetableEntry t) => t.SectionId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<FeeStructure>().HasOne((FeeStructure f) => f.AcademicYear).WithMany()
			.HasForeignKey((FeeStructure f) => f.AcademicYearId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<FeeStructure>().HasOne((FeeStructure f) => f.Department).WithMany()
			.HasForeignKey((FeeStructure f) => f.DepartmentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<FeeStructure>().Property((FeeStructure f) => f.Amount).HasPrecision(18, 2);
		builder.Entity<FeeStructure>().Property((FeeStructure f) => f.AmountForGovernmentSponsored).HasPrecision(18, 2);
		builder.Entity<Invoice>().HasOne((Invoice i) => i.Student).WithMany()
			.HasForeignKey((Invoice i) => i.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Invoice>().HasOne((Invoice i) => i.Semester).WithMany()
			.HasForeignKey((Invoice i) => i.SemesterId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Invoice>().Property((Invoice i) => i.TotalAmount).HasPrecision(18, 2);
		builder.Entity<Invoice>().HasIndex((Invoice i) => i.InvoiceNumber).IsUnique();
		builder.Entity<Invoice>().HasIndex((Invoice i) => new { i.StudentId, i.SemesterId }).IsUnique();
		builder.Entity<InvoiceLine>().HasOne((InvoiceLine l) => l.Invoice).WithMany((Invoice i) => i.Lines)
			.HasForeignKey((InvoiceLine l) => l.InvoiceId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<InvoiceLine>().HasOne((InvoiceLine l) => l.FeeStructure).WithMany()
			.HasForeignKey((InvoiceLine l) => l.FeeStructureId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<InvoiceLine>().Property((InvoiceLine l) => l.Amount).HasPrecision(18, 2);
		builder.Entity<Waiver>().HasOne((Waiver w) => w.Invoice).WithMany((Invoice i) => i.Waivers)
			.HasForeignKey((Waiver w) => w.InvoiceId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Waiver>().Property((Waiver w) => w.Amount).HasPrecision(18, 2);
		builder.Entity<Payment>().HasOne((Payment p) => p.Invoice).WithMany((Invoice i) => i.Payments)
			.HasForeignKey((Payment p) => p.InvoiceId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Payment>().HasOne((Payment p) => p.Student).WithMany()
			.HasForeignKey((Payment p) => p.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Payment>().Property((Payment p) => p.Amount).HasPrecision(18, 2);
		builder.Entity<Payment>().HasIndex((Payment p) => p.Reference).IsUnique();
		builder.Entity<LedgerEntry>().HasOne((LedgerEntry e) => e.Student).WithMany()
			.HasForeignKey((LedgerEntry e) => e.StudentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<LedgerEntry>().HasOne((LedgerEntry e) => e.Invoice).WithMany()
			.HasForeignKey((LedgerEntry e) => e.InvoiceId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<LedgerEntry>().HasOne((LedgerEntry e) => e.Payment).WithMany()
			.HasForeignKey((LedgerEntry e) => e.PaymentId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<LedgerEntry>().HasOne((LedgerEntry e) => e.Waiver).WithMany()
			.HasForeignKey((LedgerEntry e) => e.WaiverId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<LedgerEntry>().Property((LedgerEntry e) => e.Amount).HasPrecision(18, 2);
		builder.Entity<FinanceSetting>().Property((FinanceSetting s) => s.FirstInstallmentPercent).HasPrecision(5, 2);
		builder.Entity<FinanceSetting>().Property((FinanceSetting s) => s.ChapaBaseUrl).HasMaxLength(300);
		builder.Entity<TimetableEntry>().HasOne((TimetableEntry t) => t.Period).WithMany()
			.HasForeignKey((TimetableEntry t) => t.PeriodId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Parent>().HasOne((Parent p) => p.Student).WithMany((Student s) => s.Parents)
			.HasForeignKey((Parent p) => p.StudentId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<Parent>().HasOne((Parent p) => p.ApplicationUser).WithOne((ApplicationUser u) => u.Parent)
			.HasForeignKey((Parent p) => p.ApplicationUserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Instructor>().HasOne((Instructor i) => i.Region).WithMany()
			.HasForeignKey((Instructor i) => i.RegionId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Instructor>().HasOne((Instructor i) => i.Zone).WithMany()
			.HasForeignKey((Instructor i) => i.ZoneId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Instructor>().HasOne((Instructor i) => i.Woreda).WithMany()
			.HasForeignKey((Instructor i) => i.WoredaId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<Instructor>().HasOne((Instructor i) => i.Kebele).WithMany()
			.HasForeignKey((Instructor i) => i.KebeleId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<CourseMaterial>().HasOne((CourseMaterial m) => m.Course).WithMany()
			.HasForeignKey((CourseMaterial m) => m.CourseId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.Entity<CourseMaterial>().HasOne((CourseMaterial m) => m.UploadedBy).WithMany()
			.HasForeignKey((CourseMaterial m) => m.UploadedById)
			.OnDelete(DeleteBehavior.Restrict);
		builder.Entity<CourseMaterial>().Property((CourseMaterial m) => m.Title).HasMaxLength(200)
			.IsRequired();
		builder.Entity<CourseMaterial>().Property((CourseMaterial m) => m.Description).HasMaxLength(2000);
		builder.Entity<CourseMaterial>().Property((CourseMaterial m) => m.FileName).HasMaxLength(255)
			.IsRequired();
		builder.Entity<CourseMaterial>().Property((CourseMaterial m) => m.FilePath).HasMaxLength(500)
			.IsRequired();
		builder.Entity<CourseMaterial>().Property((CourseMaterial m) => m.FileType).HasMaxLength(20)
			.IsRequired();
		builder.Entity<CourseMaterial>().HasIndex((CourseMaterial m) => m.CourseId);
		builder.Entity<CourseMaterial>().HasIndex((CourseMaterial m) => m.UploadedById);
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		foreach (EntityEntry<BaseEntity> item in ChangeTracker.Entries<BaseEntity>())
		{
			if (item.State == EntityState.Added)
			{
				if (item.Entity.CreatedAt == default(DateTime))
				{
					item.Entity.CreatedAt = DateTime.UtcNow;
				}
			}
			else if (item.State == EntityState.Modified)
			{
				item.Entity.UpdatedAt = DateTime.UtcNow;
			}
		}
		return base.SaveChangesAsync(cancellationToken);
	}
}
