namespace Employee_Management_System.ViewModels.Dashboard
{
    public class EmployeeDashboardViewModel
    {
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public int TotalExpectedDays { get; set; }
        public IEnumerable<EmployeeVacationDetail> VacationDetails { get; set; } = Array.Empty<EmployeeVacationDetail>();
    }

    public class EmployeeVacationDetail
    {
        public string VacationType { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Days { get; set; }
        public string Status { get; set; } = null!;
        public string? Reason { get; set; }
    }
}
