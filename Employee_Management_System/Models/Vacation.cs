using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Management_System.Models
{
    public class Vacation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [ForeignKey(nameof(VacationType))]
        public int VacationTypeId { get; set; }
        public VacationType? VacationType { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysRequested { get; set; }

        public string? Reason { get; set; }
        public string? Status { get; set; }

        [ForeignKey(nameof(ApprovedBy))]
        public int? ApprovedById { get; set; }
        public Employee? ApprovedBy { get; set; }

        public string? DecisionComment { get; set; }
    }
}
