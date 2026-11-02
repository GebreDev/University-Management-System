using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Web.Models.ViewModels
{
    public class RoleCreateViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Permissions")]
        public List<string> SelectedPermissions { get; set; } = new List<string>();
    }
}