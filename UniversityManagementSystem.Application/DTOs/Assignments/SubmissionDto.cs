using System;

namespace UniversityManagementSystem.Application.DTOs.Assignments
{
    public class SubmissionDto
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string FilePath { get; set; }
        public int? Grade { get; set; }
        public string Feedback { get; set; }
        public bool IsGraded { get; set; }
    }
}