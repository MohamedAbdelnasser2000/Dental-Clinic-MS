namespace DentalClinicSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalServices { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<Appointment> TodaysAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> RecentAppointments { get; set; } = new List<Appointment>();
        public List<Patient> RecentPatients { get; set; } = new List<Patient>();
        public int TodaysAppointmentsCount { get; set; }
        public int PendingAppointmentsCount { get; set; }
        public decimal TodaysRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
    }
}
