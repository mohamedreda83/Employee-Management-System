using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Management_System.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeNumber { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        public string? ProfilePhoto { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? NationalId { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [ForeignKey(nameof(Position))]
        public int? PositionId { get; set; }
        public Position? Position { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public DateOnly? HireDate { get; set; }

        [StringLength(50)]
        public string? EmploymentStatus { get; set; }

        public string? Notes { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
        public ICollection<Vacation> Vacations { get; set; } = new HashSet<Vacation>();
        public ICollection<VacationBalance> VacationBalances { get; set; } = new HashSet<VacationBalance>();
    }
}
