using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.ViewModels.Position
{
    public class PositionListViewModel
    {
        public IEnumerable<PositionListItemViewModel> Positions { get; set; } = Array.Empty<PositionListItemViewModel>();
        public string? Search { get; set; }
        public int? FilterDepartmentId { get; set; }
        public IEnumerable<DepartmentSelectItem> Departments { get; set; } = Array.Empty<DepartmentSelectItem>();
    }

    public class PositionListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class PositionFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Position title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = null!;

        public int? DepartmentId { get; set; }

        // For department dropdown
        public IEnumerable<DepartmentSelectItem> Departments { get; set; } = Array.Empty<DepartmentSelectItem>();
    }

    public class DepartmentSelectItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
