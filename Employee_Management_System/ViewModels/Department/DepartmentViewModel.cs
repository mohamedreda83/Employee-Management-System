using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.ViewModels.Department
{
    public class DepartmentListViewModel
    {
        public IEnumerable<DepartmentListItemViewModel> Departments { get; set; } = Array.Empty<DepartmentListItemViewModel>();
        public string? Search { get; set; }
    }

    public class DepartmentListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ManagerName { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class DepartmentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        public int? ManagerId { get; set; }

        // For manager dropdown
        public IEnumerable<ManagerSelectItem> Managers { get; set; } = Array.Empty<ManagerSelectItem>();
    }

    public class ManagerSelectItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
    }
}
