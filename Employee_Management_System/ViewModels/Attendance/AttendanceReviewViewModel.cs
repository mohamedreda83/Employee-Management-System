namespace Employee_Management_System.ViewModels.Attendance
{
    public class AttendanceReviewViewModel
    {
        public DateOnly Date { get; set; }
        public string StatusFilter { get; set; } = "All";
        public IEnumerable<AttendanceReviewRecordViewModel> AttendanceRecords { get; set; } = Array.Empty<AttendanceReviewRecordViewModel>();
        public IEnumerable<string> StatusOptions { get; set; } = new[] { "All", "Present", "Absent", "Late", "On Leave" };
        public IEnumerable<AttendanceEmployeeItemViewModel> Employees { get; set; } = Array.Empty<AttendanceEmployeeItemViewModel>();
    }

    public class AttendanceReviewRecordViewModel
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Department { get; set; }
        public string? Position { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Status { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        public string? ManualReason { get; set; }
    }

    public class AttendanceEmployeeItemViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Department { get; set; }
        public string? Position { get; set; }
    }
}
