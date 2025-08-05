using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Models;
using DentalClinicSystem.Data;

namespace DentalClinicSystem.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        // Get today's appointments
        var todaysAppointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Service)
            .Include(a => a.Dentist)
            .Where(a => a.AppointmentDate.Date == today)
            .ToListAsync();

        // Sort by StartTime on client side (SQLite doesn't support TimeSpan in ORDER BY)
        todaysAppointments = todaysAppointments
            .OrderBy(a => a.StartTime)
            .Take(5)
            .ToList();

        // Get statistics
        var totalPatients = await _context.Patients.CountAsync();
        var totalAppointments = await _context.Appointments.CountAsync();
        var totalServices = await _context.Services.CountAsync();
        // Calculate monthly revenue (SQLite doesn't support Sum on decimal, so get all and sum on client)
        var monthlyTreatments = await _context.Treatments
            .Where(t => t.TreatmentDate.Month == DateTime.Now.Month)
            .ToListAsync();

        var totalRevenue = monthlyTreatments.Sum(t => t.Cost);

        var dashboardData = new DashboardViewModel
        {
            TotalPatients = totalPatients,
            TotalAppointments = totalAppointments,
            TotalServices = totalServices,
            MonthlyRevenue = totalRevenue,
            TodaysAppointments = todaysAppointments
        };

        return View(dashboardData);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
