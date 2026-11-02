using UniversityManagementSystem.Domain.Common;
using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Domain.Entities
{
    /// <summary>
    /// One selected offered course inside a Registration.
    /// </summary>
    public class RegistrationItem : BaseEntity
    {
        public int RegistrationId { get; set; }
        public Registration? Registration { get; set; }

        public int SemesterOfferedCourseId { get; set; }
        public SemesterOfferedCourse? SemesterOfferedCourse { get; set; }

        public int Credits { get; set; }
        public new RegistrationItemStatus Status { get; set; } = RegistrationItemStatus.Pending;
        public string? Note { get; set; }
    }
}
