using UniversityManagementSystem.Domain.Enums;
using System;

namespace UniversityManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceListDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public string Remarks { get; set; }
        public string InstructorName { get; set; }
    }
}