namespace Employee_Management_System.ViewModels.Attendance
{
    public class EmployeeAttendanceViewModel
    {
        public DateOnly CurrentDate { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public AttendanceMonthRecordViewModel? TodayRecord { get; set; }
        public IEnumerable<AttendanceMonthRecordViewModel> MonthlyRecords { get; set; } = Array.Empty<AttendanceMonthRecordViewModel>();
    }

    public class AttendanceMonthRecordViewModel
    {
        public DateOnly Date { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Status { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
    }
}
