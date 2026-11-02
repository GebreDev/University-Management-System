using System;
using UniversityManagementSystem.Domain.Enums;

namespace UniversityManagementSystem.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public EntityStatus Status { get; set; } = EntityStatus.Active;
    }
}