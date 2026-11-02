using UniversityManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public string Remarks { get; set; }
    }
}