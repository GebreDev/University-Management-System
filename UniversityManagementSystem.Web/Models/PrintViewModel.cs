namespace UniversityManagementSystem.Web.Models
{
    public class PrintViewModel
    {
        public string Title { get; set; } = "";
        public string[] HideHeaders { get; set; } = new string[] { "Actions", "Edit", "Delete" };
    }
}
