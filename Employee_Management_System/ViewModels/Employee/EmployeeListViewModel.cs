namespace Employee_Management_System.ViewModels.Employee
{
    public class EmployeeListViewModel
    {
        public string? Search { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public string? Status { get; set; }
        public IEnumerable<EmployeeListItemViewModel> Employees { get; set; } = Array.Empty<EmployeeListItemViewModel>();
        public IEnumerable<SelectItem> Departments { get; set; } = Array.Empty<SelectItem>();
        public IEnumerable<SelectItem> Positions { get; set; } = Array.Empty<SelectItem>();
        public IEnumerable<string> Statuses { get; set; } = Array.Empty<string>();
    }
}
