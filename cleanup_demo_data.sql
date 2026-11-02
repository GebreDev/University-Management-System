-- ============================================================
-- UMS demo-data cleanup (single atomic transaction)
-- KEEPS: admin@ums.com, AspNetRoles, Periods, TimetableSettings,
--        FinanceSettings, empty address tables, __EFMigrationsHistory
-- DELETES: students, instructors (and their logins except admin),
--          courses, departments, years, semesters, and all demo
--          academic/finance/chat/audit data
-- ============================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRY
BEGIN TRANSACTION;

    -- 1) Grading (children that reference exams first)
    DELETE FROM Grades;
    DELETE FROM AssessmentMarks;
    DELETE FROM CourseAssessments;
    DELETE FROM CourseResults;
    DELETE FROM CourseGrades;
    DELETE FROM AcademicSections;
    DELETE FROM AcademicGrades;

    -- 2) Answers / options / questions
    DELETE FROM StudentAnswers;
    DELETE FROM QuestionOptions;
    DELETE FROM Questions;
    DELETE FROM AssignmentAnswers;
    DELETE FROM AssignmentQuestionOptions;
    DELETE FROM AssignmentQuestions;
    DELETE FROM AssignmentResults;
    DELETE FROM ExamResults;
    DELETE FROM Submissions;
    DELETE FROM Exams;
    DELETE FROM Assignments;

    -- 3) Attendance & materials & schedules
    DELETE FROM Attendances;
    DELETE FROM CourseMaterials;
    DELETE FROM CourseSchedules;

    -- 4) Enrollment
    DELETE FROM Enrollments;

    -- 5) Fees (ledger is append-only; clearing demo finance data entirely)
    DELETE FROM LedgerEntries;
    DELETE FROM Payments;
    DELETE FROM Waivers;
    DELETE FROM InvoiceLines;
    DELETE FROM Invoices;
    DELETE FROM FeeStructures;

    -- 6) Communication & events & logs
    DELETE FROM ChatMessages;
    DELETE FROM ChatConversations;
    DELETE FROM Notifications;
    DELETE FROM UniversityEvents;
    DELETE FROM AuditLogs;

    -- 7) Academic structure (courses before people: Courses.InstructorId → Instructors)
    DELETE FROM TimetableEntries;
    DELETE FROM Courses;

    -- 8) People (entities before identity users)
    DELETE FROM Students;
    DELETE FROM Instructors;
    DELETE FROM Parents;

    -- 9) Departments & calendar (after courses/people are gone)
    DELETE FROM Departments;
    DELETE FROM Semesters;
    DELETE FROM AcademicYears;

    -- 9) Identity users (ALL EXCEPT THE ADMIN) - role links first
    DELETE ur
    FROM AspNetUserRoles ur
    JOIN AspNetUsers u ON u.Id = ur.UserId
    WHERE u.UserName <> 'admin@ums.com';

    DELETE uc FROM AspNetUserClaims uc
    JOIN AspNetUsers u ON u.Id = uc.UserId WHERE u.UserName <> 'admin@ums.com';
    DELETE ul FROM AspNetUserLogins ul
    JOIN AspNetUsers u ON u.Id = ul.UserId WHERE u.UserName <> 'admin@ums.com';
    DELETE ut FROM AspNetUserTokens ut
    JOIN AspNetUsers u ON u.Id = ut.UserId WHERE u.UserName <> 'admin@ums.com';

    DELETE FROM AspNetUsers WHERE UserName <> 'admin@ums.com';

COMMIT TRANSACTION;

    -- Post-check summary
    SELECT 'REMAINING' AS Phase, t.name AS TableName, SUM(p.rows) AS RowsLeft
    FROM sys.tables t
    JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
    WHERE t.name IN ('AspNetUsers','AspNetRoles','Students','Instructors','Courses',
                     'Departments','AcademicYears','Semesters','Enrollments','Grades',
                     'Attendances','Invoices','FeeStructures','AuditLogs','Periods',
                     'TimetableSettings','FinanceSettings')
    GROUP BY t.name
    ORDER BY t.name;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'ERROR - TRANSACTION ROLLED BACK, NOTHING DELETED';
    PRINT ERROR_NUMBER();
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
