namespace Employee_Management_System.ViewModels.Employee
{
    public class EmployeeListItemViewModel
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfilePhoto { get; set; }
        public string Department { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string EmploymentStatus { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
