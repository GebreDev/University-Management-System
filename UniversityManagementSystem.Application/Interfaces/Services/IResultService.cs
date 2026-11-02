using UniversityManagementSystem.Application.DTOs.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversityManagementSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Course results workflow: instructor enters assessment marks on a dynamic
    /// grid, saves drafts and finally "sends" the results so students can see them.
    /// </summary>
    public interface IResultService
    {
        /// <summary>Courses taught by the instructor with their result status (Grades &amp; Results table).</summary>
        Task<IEnumerable<InstructorCourseResultDto>> GetInstructorCourseResultsAsync(int instructorId);

        /// <summary>Full assessment board model (enrolled students + assessment columns + marks) for a course.</summary>
        Task<CourseAssessmentBoardDto> GetCourseAssessmentBoardAsync(int courseId);

        /// <summary>Saves the assessment columns and marks as a draft (students cannot see anything yet).</summary>
        Task SaveDraftAsync(int courseId, SaveCourseAssessmentDto dto);

        /// <summary>Saves the board and publishes final results to all graded students (sends notifications).</summary>
        Task<int> PublishResultsAsync(int courseId, SaveCourseAssessmentDto dto);

        /// <summary>Student transcript with SGP / CGP / SGPA / CGPA (published results only).</summary>
        Task<StudentTranscriptDto> GetStudentTranscriptAsync(int studentId, int? semester = null);

        /// <summary>Breakdown of one published course result for a student (assessment modal).</summary>
        Task<StudentCourseResultDetailDto?> GetStudentCourseResultDetailAsync(int studentId, int courseId);
    }
}
