using DentalClinicSystem.Models;

namespace DentalClinicSystem.ViewModels
{
    public class DashboardReportViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalTreatments { get; set; }
        public int TotalInvoices { get; set; }
        public int TodayAppointments { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedTreatments { get; set; }
    }

    public class FinancialReportViewModel
    {
        public int TotalInvoices { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int PaidInvoices { get; set; }
        public int UnpaidInvoices { get; set; }
        public int PartiallyPaidInvoices { get; set; }
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }

    public class DentistPerformanceViewModel
    {
        public string DentistId { get; set; } = string.Empty;
        public string DentistName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public double CompletionRate => TotalAppointments > 0 ? (double)CompletedAppointments / TotalAppointments * 100 : 0;
    }
}
