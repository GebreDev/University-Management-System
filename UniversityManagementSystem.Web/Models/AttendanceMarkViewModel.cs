using UniversityManagementSystem.Application.DTOs.Attendance;
using UniversityManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Web.Models
{
    public class AttendanceMarkViewModel
    {
        [Required(ErrorMessage = "Course is required.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "At least one student is required.")]
        public List<AttendanceStudentViewModel> Students { get; set; } = new();
    }

    public class AttendanceStudentViewModel
    {
        [Required]
        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public AttendanceStatus Status { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
}
