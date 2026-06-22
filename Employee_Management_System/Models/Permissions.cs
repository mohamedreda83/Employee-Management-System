namespace Employee_Management_System.Models
{
    public static class Permissions
    {
        public static List<string> All => new()
        {
            Dashboard.View, Dashboard.ViewEmployee,
            Employees.View, Employees.Create, Employees.Edit, Employees.Delete,
            Departments.View, Departments.Create, Departments.Edit, Departments.Delete,
            Positions.View, Positions.Create, Positions.Edit, Positions.Delete,
            Attendance.ViewMy, Attendance.Review,
            Vacations.Request, Vacations.Approve,
            UserManagement.View, UserManagement.Manage,
            RoleManagement.View, RoleManagement.Manage
        };

        public static class Dashboard
        {
            public const string View = "Dashboard.View";
            public const string ViewEmployee = "Dashboard.ViewEmployee";
        }

        public static class Employees
        {
            public const string View = "Employees.View";
            public const string Create = "Employees.Create";
            public const string Edit = "Employees.Edit";
            public const string Delete = "Employees.Delete";
        }

        public static class Departments
        {
            public const string View = "Departments.View";
            public const string Create = "Departments.Create";
            public const string Edit = "Departments.Edit";
            public const string Delete = "Departments.Delete";
        }

        public static class Positions
        {
            public const string View = "Positions.View";
            public const string Create = "Positions.Create";
            public const string Edit = "Positions.Edit";
            public const string Delete = "Positions.Delete";
        }

        public static class Attendance
        {
            public const string ViewMy = "Attendance.ViewMy";
            public const string Review = "Attendance.Review";
        }

        public static class Vacations
        {
            public const string Request = "Vacations.Request";
            public const string Approve = "Vacations.Approve";
        }

        public static class UserManagement
        {
            public const string View = "UserManagement.View";
            public const string Manage = "UserManagement.Manage";
        }

        public static class RoleManagement
        {
            public const string View = "RoleManagement.View";
            public const string Manage = "RoleManagement.Manage";
        }
    }
}
