namespace Employee_Management_System.ViewModels.Employee
{
    public class EmployeeDetailsViewModel
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfilePhoto { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
        public DateOnly? HireDate { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? Notes { get; set; }
        public IEnumerable<AttendanceHistoryItem> AttendanceHistory { get; set; } = Array.Empty<AttendanceHistoryItem>();
        public IEnumerable<VacationHistoryItem> VacationHistory { get; set; } = Array.Empty<VacationHistoryItem>();
    }

    public class AttendanceHistoryItem
    {
        public DateOnly Date { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Status { get; set; }
    }

    public class VacationHistoryItem
    {
        public string? VacationType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string? Status { get; set; }
    }
}
