using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Models
{
    public class VacationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public int DefaultAnnualDays { get; set; }

        public ICollection<Vacation> Vacations { get; set; } = new HashSet<Vacation>();
        public ICollection<VacationBalance> VacationBalances { get; set; } = new HashSet<VacationBalance>();
    }
}
