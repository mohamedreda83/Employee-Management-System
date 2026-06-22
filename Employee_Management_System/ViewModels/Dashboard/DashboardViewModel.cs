namespace Employee_Management_System.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int OnLeaveCount { get; set; }
        public IEnumerable<DepartmentChartItem> DepartmentChart { get; set; } = Array.Empty<DepartmentChartItem>();
        public IEnumerable<ActivityItem> RecentActivities { get; set; } = Array.Empty<ActivityItem>();
        public IEnumerable<NotificationItem> Notifications { get; set; } = Array.Empty<NotificationItem>();
        public IEnumerable<string> QuickActions { get; set; } = Array.Empty<string>();
    }

    public class DepartmentChartItem
    {
        public string DepartmentName { get; set; } = null!;
        public int EmployeeCount { get; set; }
    }

    public class ActivityItem
    {
        public string EmployeeName { get; set; } = null!;
        public string ActivityType { get; set; } = null!;
        public string ActivityTime { get; set; } = null!;
        public DateOnly Date { get; set; }
    }

    public class NotificationItem
    {
        public string Title { get; set; } = null!;
        public string? Body { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}