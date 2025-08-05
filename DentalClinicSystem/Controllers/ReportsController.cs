using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public IActionResult Index()
        {
            return View();
        }

        // GET: Reports/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = new DashboardReportViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(p => p.IsActive),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TotalTreatments = await _context.Treatments.CountAsync(),
                TotalInvoices = await _context.Invoices.CountAsync(),
                
                TodayAppointments = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate.Date == DateTime.Today),
                
                // SQLite doesn't support Sum on decimal, so get all and sum on client
                ThisMonthRevenue = (await _context.Invoices
                    .Where(i => i.InvoiceDate.Month == DateTime.Now.Month &&
                               i.InvoiceDate.Year == DateTime.Now.Year)
                    .ToListAsync())
                    .Sum(i => i.PaidAmount),
                
                PendingAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == "مؤكد" && a.AppointmentDate >= DateTime.Today),

                CompletedTreatments = await _context.Treatments
                    .CountAsync(t => t.Status == "مكتمل")
            };

            return View(dashboardData);
        }

        // GET: Reports/Patients
        public async Task<IActionResult> Patients(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Patients.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(p => p.RegistrationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.RegistrationDate <= endDate.Value);
            }

            var patients = await query
                .Include(p => p.InsuranceCompany)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(patients);
        }

        // GET: Reports/Appointments
        public async Task<IActionResult> Appointments(DateTime? startDate, DateTime? endDate, string status)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Dentist)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Status = status;

            return View(appointments);
        }

        // GET: Reports/Treatments
        public async Task<IActionResult> Treatments(DateTime? startDate, DateTime? endDate, string treatmentType)
        {
            var query = _context.Treatments
                .Include(t => t.Patient)
                .Include(t => t.Dentist)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(t => t.TreatmentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.TreatmentDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(treatmentType))
            {
                query = query.Where(t => t.TreatmentType.Contains(treatmentType));
            }

            var treatments = await query
                .OrderByDescending(t => t.TreatmentDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.TreatmentType = treatmentType;

            return View(treatments);
        }

        // GET: Reports/Financial
        public async Task<IActionResult> Financial(DateTime? startDate, DateTime? endDate, string paymentStatus)
        {
            var query = _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Treatment)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(i => i.Status == paymentStatus);
            }

            var invoices = await query
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.PaymentStatus = paymentStatus;

            return View(invoices);
        }

        // GET: Reports/DentistPerformance
        public async Task<IActionResult> DentistPerformance(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Appointments
                .Include(a => a.Dentist)
                .Include(a => a.Patient)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate <= endDate.Value);
            }

            var appointments = await query.ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Appointments = appointments;

            return View();
        }
    }
}
