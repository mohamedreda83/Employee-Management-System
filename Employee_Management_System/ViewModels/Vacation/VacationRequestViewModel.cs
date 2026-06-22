namespace Employee_Management_System.ViewModels.Vacation
{
    public class VacationRequestViewModel
    {
        public IEnumerable<VacationBalanceCardViewModel> BalanceCards { get; set; } = Array.Empty<VacationBalanceCardViewModel>();
        public IEnumerable<VacationTypeSelectItem> VacationTypes { get; set; } = Array.Empty<VacationTypeSelectItem>();
        public IEnumerable<VacationRequestItemViewModel> Requests { get; set; } = Array.Empty<VacationRequestItemViewModel>();
        public VacationRequestFormModel NewRequest { get; set; } = new VacationRequestFormModel();
    }

    public class VacationBalanceCardViewModel
    {
        public string VacationType { get; set; } = null!;
        public int EntitledDays { get; set; }
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
    }

    public class VacationTypeSelectItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class VacationRequestItemViewModel
    {
        public int Id { get; set; }
        public string VacationType { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
    }

    public class VacationRequestFormModel
    {
        public int EmployeeId { get; set; }
        public int VacationTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string? Reason { get; set; }
    }
}
