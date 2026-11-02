using UniversityManagementSystem.Domain.Common;
using System;

namespace UniversityManagementSystem.Domain.Entities
{
    public class UniversityEvent : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string EventType { get; set; } = "General"; // Ceremony, Academic, Administrative, Social
        public string Color { get; set; } = "primary";     // Bootstrap-ish accent used by the dashboard
        public bool IsPublic { get; set; } = true;
    }
}
