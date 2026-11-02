using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Domain.Entities
{
    public class RolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public string Permission { get; set; } = string.Empty;
    }
}
