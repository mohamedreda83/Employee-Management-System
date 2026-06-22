using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.ViewModels.UserManagement
{
    // Index list item — one row per employee
    public class UserManagementListViewModel
    {
        public IEnumerable<UserRowViewModel> Rows { get; set; } = Array.Empty<UserRowViewModel>();
        public string? Search { get; set; }
    }

    public class UserRowViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmploymentStatus { get; set; }

        // Account info (null if no account)
        public string? UserId { get; set; }
        public bool HasAccount => UserId != null;
        public IList<string> Roles { get; set; } = new List<string>();
        public bool IsLockedOut { get; set; }
    }

    // Create account form
    public class CreateUserViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string EmployeeNumber { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;

        public List<string> SelectedRoles { get; set; } = new();
        public IEnumerable<string> AvailableRoles { get; set; } = Array.Empty<string>();
    }

    // Edit roles / lock-out
    public class EditUserViewModel
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string EmployeeNumber { get; set; } = null!;
        public bool IsLockedOut { get; set; }

        public List<string> SelectedRoles { get; set; } = new();
        public IEnumerable<string> AvailableRoles { get; set; } = Array.Empty<string>();
    }

    // Reset password form
    public class ResetPasswordAdminViewModel
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
