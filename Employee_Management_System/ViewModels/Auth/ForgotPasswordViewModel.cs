using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
