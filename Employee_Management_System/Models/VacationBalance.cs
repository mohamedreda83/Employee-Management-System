using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Management_System.Models
{
    public class VacationBalance
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [ForeignKey(nameof(VacationType))]
        public int VacationTypeId { get; set; }
        public VacationType? VacationType { get; set; }

        public int Year { get; set; }
        public int EntitledDays { get; set; }
        public int UsedDays { get; set; }
    }
}
