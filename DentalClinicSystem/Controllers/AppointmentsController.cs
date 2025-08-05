using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;

namespace DentalClinicSystem.Controllers;

[Authorize(Roles = "Admin,Doctor,Receptionist")]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public AppointmentsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Appointments
    public async Task<IActionResult> Index(string searchString, int? dentistId, int? patientId, DateTime? appointmentDate, string status, int page = 1, int pageSize = 10)
    {
        var appointments = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Dentist)
            .Include(a => a.Service)
            .Include(a => a.CreatedByUser)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            appointments = appointments.Where(a => a.Patient.FullName.Contains(searchString) ||
                                                 a.Dentist.FullName.Contains(searchString) ||
                                                 a.ReasonForVisit!.Contains(searchString));
        }

        if (dentistId.HasValue)
        {
            appointments = appointments.Where(a => a.DentistId == dentistId);
        }

        if (patientId.HasValue)
        {
            appointments = appointments.Where(a => a.PatientId == patientId);
        }

        if (appointmentDate.HasValue)
        {
            appointments = appointments.Where(a => a.AppointmentDate.Date == appointmentDate.Value.Date);
        }

        if (!string.IsNullOrEmpty(status))
        {
            appointments = appointments.Where(a => a.Status == status);
        }

        var totalCount = await appointments.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var appointmentsList = await appointments
            .OrderBy(a => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Sort by StartTime in memory since SQLite doesn't support TimeSpan in ORDER BY
        appointmentsList = appointmentsList.OrderBy(a => a.StartTime).ToList();

        ViewBag.SearchString = searchString;
        ViewBag.DentistId = dentistId;
        ViewBag.PatientId = patientId;
        ViewBag.AppointmentDate = appointmentDate;
        ViewBag.Status = status;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;

        ViewBag.Dentists = new SelectList(
            await _context.Dentists.Where(d => d.IsActive).ToListAsync(),
            "DentistId", "FullName");

        ViewBag.Patients = new SelectList(
            await _context.Patients.Where(p => p.IsActive).ToListAsync(),
            "PatientId", "FullName");

        ViewBag.StatusList = new SelectList(new[]
        {
            new { Value = "", Text = "جميع الحالات" },
            new { Value = "مؤكد", Text = "مؤكد" },
            new { Value = "ملغي", Text = "ملغي" },
            new { Value = "تم", Text = "تم" },
            new { Value = "لم يحضر", Text = "لم يحضر" }
        }, "Value", "Text");

        return View(appointmentsList);
    }

    // GET: Appointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Dentist)
            .Include(a => a.Service)
            .Include(a => a.CreatedByUser)
            .Include(a => a.Treatments)
            .FirstOrDefaultAsync(m => m.AppointmentId == id);

        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // GET: Appointments/Create
    public async Task<IActionResult> Create(int? patientId, int? dentistId)
    {
        ViewBag.Patients = new SelectList(
            await _context.Patients.Where(p => p.IsActive).ToListAsync(),
            "PatientId", "FullName", patientId);

        ViewBag.Dentists = new SelectList(
            await _context.Dentists.Where(d => d.IsActive).ToListAsync(),
            "DentistId", "FullName", dentistId);

        ViewBag.Services = new SelectList(
            await _context.Services.Where(s => s.IsActive).ToListAsync(),
            "ServiceId", "ServiceName");

        var appointment = new Appointment();
        if (patientId.HasValue)
            appointment.PatientId = patientId.Value;
        if (dentistId.HasValue)
            appointment.DentistId = dentistId.Value;

        return View(appointment);
    }

    // POST: Appointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        try
        {
            // Debug: Log the received data
            Console.WriteLine($"Received appointment data: PatientId={appointment.PatientId}, DentistId={appointment.DentistId}, Date={appointment.AppointmentDate}, StartTime={appointment.StartTime}, EndTime={appointment.EndTime}");

            // Remove navigation property validation errors
            ModelState.Remove("Patient");
            ModelState.Remove("Dentist");
            ModelState.Remove("Service");
            ModelState.Remove("CreatedByUser");

            // Check ModelState errors
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            if (ModelState.IsValid)
            {
                // Check for conflicts - Get all appointments for the same dentist and date first
                var existingAppointments = await _context.Appointments
                    .Where(a => a.DentistId == appointment.DentistId &&
                               a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                               a.Status != "ملغي")
                    .ToListAsync();

                // Check for time conflicts in memory
                var conflictingAppointment = existingAppointments
                    .FirstOrDefault(a =>
                        (a.StartTime <= appointment.StartTime && a.EndTime > appointment.StartTime) ||
                        (a.StartTime < appointment.EndTime && a.EndTime >= appointment.EndTime) ||
                        (a.StartTime >= appointment.StartTime && a.EndTime <= appointment.EndTime));

                if (conflictingAppointment != null)
                {
                    ModelState.AddModelError("", "يوجد تعارض في المواعيد مع هذا الطبيب في نفس الوقت");
                }
                else
                {
                    appointment.CreatedByUserId = _userManager.GetUserId(User);
                    appointment.CreatedAt = DateTime.Now;
                    appointment.Status = "مؤكد";

                    _context.Add(appointment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم حجز الموعد بنجاح";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating appointment: {ex.Message}");
            ModelState.AddModelError("", "حدث خطأ أثناء حفظ الموعد. يرجى المحاولة مرة أخرى.");
        }

        ViewBag.Patients = new SelectList(
            await _context.Patients.Where(p => p.IsActive).ToListAsync(),
            "PatientId", "FullName", appointment.PatientId);

        ViewBag.Dentists = new SelectList(
            await _context.Dentists.Where(d => d.IsActive).ToListAsync(),
            "DentistId", "FullName", appointment.DentistId);

        ViewBag.Services = new SelectList(
            await _context.Services.Where(s => s.IsActive).ToListAsync(),
            "ServiceId", "ServiceName", appointment.ServiceId);

        return View(appointment);
    }

    // GET: Appointments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        ViewBag.Patients = new SelectList(
            await _context.Patients.Where(p => p.IsActive).ToListAsync(),
            "PatientId", "FullName", appointment.PatientId);

        ViewBag.Dentists = new SelectList(
            await _context.Dentists.Where(d => d.IsActive).ToListAsync(),
            "DentistId", "FullName", appointment.DentistId);

        ViewBag.Services = new SelectList(
            await _context.Services.Where(s => s.IsActive).ToListAsync(),
            "ServiceId", "ServiceName", appointment.ServiceId);

        ViewBag.StatusList = new SelectList(new[]
        {
            new { Value = "مؤكد", Text = "مؤكد" },
            new { Value = "ملغي", Text = "ملغي" },
            new { Value = "تم", Text = "تم" },
            new { Value = "لم يحضر", Text = "لم يحضر" }
        }, "Value", "Text", appointment.Status);

        return View(appointment);
    }

    // POST: Appointments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,PatientId,DentistId,AppointmentDate,StartTime,EndTime,Status,ReasonForVisit,Notes,ServiceId,CreatedAt,ReminderSent")] Appointment appointment)
    {
        if (id != appointment.AppointmentId)
        {
            return NotFound();
        }

        // Remove navigation property validation errors
        ModelState.Remove("Patient");
        ModelState.Remove("Dentist");
        ModelState.Remove("Service");
        ModelState.Remove("CreatedByUser");

        if (ModelState.IsValid)
        {
            try
            {
                var existingAppointment = await _context.Appointments.FindAsync(id);
                if (existingAppointment == null)
                {
                    return NotFound();
                }

                existingAppointment.PatientId = appointment.PatientId;
                existingAppointment.DentistId = appointment.DentistId;
                existingAppointment.AppointmentDate = appointment.AppointmentDate;
                existingAppointment.StartTime = appointment.StartTime;
                existingAppointment.EndTime = appointment.EndTime;
                existingAppointment.Status = appointment.Status;
                existingAppointment.ReasonForVisit = appointment.ReasonForVisit;
                existingAppointment.Notes = appointment.Notes;
                existingAppointment.ServiceId = appointment.ServiceId;
                existingAppointment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم تحديث الموعد بنجاح";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointment.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Patients = new SelectList(
            await _context.Patients.Where(p => p.IsActive).ToListAsync(),
            "PatientId", "FullName", appointment.PatientId);

        ViewBag.Dentists = new SelectList(
            await _context.Dentists.Where(d => d.IsActive).ToListAsync(),
            "DentistId", "FullName", appointment.DentistId);

        ViewBag.Services = new SelectList(
            await _context.Services.Where(s => s.IsActive).ToListAsync(),
            "ServiceId", "ServiceName", appointment.ServiceId);

        ViewBag.StatusList = new SelectList(new[]
        {
            new { Value = "مؤكد", Text = "مؤكد" },
            new { Value = "ملغي", Text = "ملغي" },
            new { Value = "تم", Text = "تم" },
            new { Value = "لم يحضر", Text = "لم يحضر" }
        }, "Value", "Text", appointment.Status);

        return View(appointment);
    }

    // GET: Appointments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Dentist)
            .Include(a => a.Service)
            .Include(a => a.CreatedByUser)
            .FirstOrDefaultAsync(m => m.AppointmentId == id);

        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // POST: Appointments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                Console.WriteLine($"Deleting appointment: ID={appointment.AppointmentId}, Patient={appointment.PatientId}, Date={appointment.AppointmentDate}");

                // Hard delete - remove completely from database
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم حذف الموعد نهائياً من قاعدة البيانات!";
                Console.WriteLine("Appointment deleted successfully from database");
            }
            else
            {
                TempData["ErrorMessage"] = "الموعد غير موجود!";
                Console.WriteLine($"Appointment with ID {id} not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting appointment: {ex.Message}");
            TempData["ErrorMessage"] = "حدث خطأ أثناء حذف الموعد. يرجى المحاولة مرة أخرى.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Appointments/TestCreate
    public async Task<IActionResult> TestCreate()
    {
        ViewBag.Patients = new SelectList(
            await _context.Patients.Where(p => p.IsActive).ToListAsync(),
            "PatientId", "FullName");

        ViewBag.Dentists = new SelectList(
            await _context.Dentists.Where(d => d.IsActive).ToListAsync(),
            "DentistId", "FullName");

        ViewBag.Services = new SelectList(
            await _context.Services.Where(s => s.IsActive).ToListAsync(),
            "ServiceId", "ServiceName");

        return View(new Appointment());
    }

    private bool AppointmentExists(int id)
    {
        return _context.Appointments.Any(e => e.AppointmentId == id);
    }
}
