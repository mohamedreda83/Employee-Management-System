using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.ViewModels.Employee
{
    public class EmployeeEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeNumber { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        public string? ProfilePhoto { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? NationalId { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public int? PositionId { get; set; }

        [Required]
        public decimal Salary { get; set; }

        public DateOnly? HireDate { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? Notes { get; set; }

        public IEnumerable<SelectItem> Departments { get; set; } = Array.Empty<SelectItem>();
        public IEnumerable<SelectItem> Positions { get; set; } = Array.Empty<SelectItem>();
    }
}
