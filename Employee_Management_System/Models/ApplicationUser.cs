using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Employee_Management_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public string? Roles { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
