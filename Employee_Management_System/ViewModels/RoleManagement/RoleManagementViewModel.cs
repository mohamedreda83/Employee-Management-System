using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.ViewModels.RoleManagement
{
    public class RoleListViewModel
    {
        public IEnumerable<RoleItemViewModel> Roles { get; set; } = new List<RoleItemViewModel>();
    }

    public class RoleItemViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int UsersCount { get; set; }
    }

    public class RoleFormViewModel
    {
        public string? Id { get; set; }
        
        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; } = null!;

        public List<string> SelectedPermissions { get; set; } = new();

        // For rendering grouped checkboxes in the view
        public Dictionary<string, List<string>> AvailablePermissions { get; set; } = new();
    }
}
