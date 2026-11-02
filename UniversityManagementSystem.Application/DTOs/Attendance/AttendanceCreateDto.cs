using UniversityManagementSystem.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceCreateDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public string Remarks { get; set; }
    }
}