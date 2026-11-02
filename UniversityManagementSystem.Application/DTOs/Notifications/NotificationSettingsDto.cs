namespace UniversityManagementSystem.Application.DTOs.Notifications
{
    public class NotificationSettingsDto
    {
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool AttendanceAlerts { get; set; }
        public bool AssignmentDeadlines { get; set; }
        public bool ExamReminders { get; set; }
    }
}