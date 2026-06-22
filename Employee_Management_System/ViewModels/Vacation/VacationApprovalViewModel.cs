namespace Employee_Management_System.ViewModels.Vacation
{
    public class VacationApprovalViewModel
    {
        public string CurrentTab { get; set; } = "Pending";
        public IEnumerable<string> Tabs { get; set; } = new[] { "Pending", "Approved", "Rejected" };
        public IEnumerable<VacationApprovalItemViewModel> Requests { get; set; } = Array.Empty<VacationApprovalItemViewModel>();
    }

    public class VacationApprovalItemViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string VacationType { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
        public string? ApprovedBy { get; set; }
        public string? DecisionComment { get; set; }
    }
}
