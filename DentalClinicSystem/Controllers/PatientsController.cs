using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using System.Linq;

namespace DentalClinicSystem.Controllers;

[Authorize(Roles = "Admin,Doctor,Receptionist")]
public class PatientsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public PatientsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Patients
    public async Task<IActionResult> Index(string searchString, int? insuranceCompanyId, int page = 1, int pageSize = 10)
    {
        var patients = _context.Patients
            .Include(p => p.InsuranceCompany)
            .Include(p => p.CreatedByUser)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            patients = patients.Where(p => p.FullName.Contains(searchString) ||
                                         p.PhoneNumber!.Contains(searchString) ||
                                         p.NationalId!.Contains(searchString) ||
                                         p.Email!.Contains(searchString));
        }

        if (insuranceCompanyId.HasValue)
        {
            patients = patients.Where(p => p.InsuranceCompanyId == insuranceCompanyId);
        }

        var totalCount = await patients.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var patientsList = await patients
            .OrderByDescending(p => p.RegistrationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.SearchString = searchString;
        ViewBag.InsuranceCompanyId = insuranceCompanyId;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;

        ViewBag.InsuranceCompanies = new SelectList(
            await _context.InsuranceCompanies.Where(ic => ic.IsActive).ToListAsync(),
            "InsuranceCompanyId", "CompanyName");

        return View(patientsList);
    }

    // GET: Patients/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .Include(p => p.InsuranceCompany)
            .Include(p => p.CreatedByUser)
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Dentist)
            .Include(p => p.Treatments)
                .ThenInclude(t => t.Dentist)
            .Include(p => p.Invoices)
            .FirstOrDefaultAsync(m => m.PatientId == id);

        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }

    // GET: Patients/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.InsuranceCompanies = new SelectList(
            await _context.InsuranceCompanies.Where(ic => ic.IsActive).ToListAsync(),
            "InsuranceCompanyId", "CompanyName");

        return View();
    }

    // POST: Patients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FullName,NationalId,PassportNumber,DateOfBirth,Gender,PhoneNumber,Email,Address,MedicalHistory,Allergies,Notes,InsuranceCompanyId")] Patient patient)
    {
        // Split FullName into FirstName and LastName if not provided
        if (!string.IsNullOrEmpty(patient.FullName))
        {
            var nameParts = patient.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length > 0)
            {
                patient.FirstName = nameParts[0];
                patient.LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
            }
        }

        if (ModelState.IsValid)
        {
            patient.CreatedByUserId = _userManager.GetUserId(User);
            patient.RegistrationDate = DateTime.Now;
            patient.IsActive = true;

            try
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم إضافة المريض بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حفظ البيانات: " + ex.Message;
            }
        }
        else
        {
            // Log validation errors for debugging
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                TempData["ErrorMessage"] = "خطأ في التحقق: " + error.ErrorMessage;
                break; // Show only first error
            }
        }

        ViewBag.InsuranceCompanies = new SelectList(
            await _context.InsuranceCompanies.Where(ic => ic.IsActive).ToListAsync(),
            "InsuranceCompanyId", "CompanyName", patient.InsuranceCompanyId);

        return View(patient);
    }

    // GET: Patients/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        ViewBag.InsuranceCompanies = new SelectList(
            await _context.InsuranceCompanies.Where(ic => ic.IsActive).ToListAsync(),
            "InsuranceCompanyId", "CompanyName", patient.InsuranceCompanyId);

        return View(patient);
    }

    // POST: Patients/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PatientId,FullName,NationalId,PassportNumber,DateOfBirth,Gender,PhoneNumber,Email,Address,MedicalHistory,Allergies,Notes,InsuranceCompanyId,RegistrationDate,IsActive")] Patient patient)
    {
        if (id != patient.PatientId)
        {
            return NotFound();
        }

        // Split FullName into FirstName and LastName if not provided
        if (!string.IsNullOrEmpty(patient.FullName))
        {
            var nameParts = patient.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length > 0)
            {
                patient.FirstName = nameParts[0];
                patient.LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingPatient = await _context.Patients.FindAsync(id);
                if (existingPatient == null)
                {
                    return NotFound();
                }

                existingPatient.FullName = patient.FullName;
                existingPatient.FirstName = patient.FirstName;
                existingPatient.LastName = patient.LastName;
                existingPatient.NationalId = patient.NationalId;
                existingPatient.PassportNumber = patient.PassportNumber;
                existingPatient.DateOfBirth = patient.DateOfBirth;
                existingPatient.Gender = patient.Gender;
                existingPatient.PhoneNumber = patient.PhoneNumber;
                existingPatient.Email = patient.Email;
                existingPatient.Address = patient.Address;
                existingPatient.MedicalHistory = patient.MedicalHistory;
                existingPatient.Allergies = patient.Allergies;
                existingPatient.Notes = patient.Notes;
                existingPatient.InsuranceCompanyId = patient.InsuranceCompanyId;
                existingPatient.IsActive = patient.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم تحديث بيانات المريض بنجاح";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(patient.PatientId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حفظ البيانات: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Log validation errors for debugging
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                TempData["ErrorMessage"] = "خطأ في التحقق: " + error.ErrorMessage;
                break; // Show only first error
            }
        }

        ViewBag.InsuranceCompanies = new SelectList(
            await _context.InsuranceCompanies.Where(ic => ic.IsActive).ToListAsync(),
            "InsuranceCompanyId", "CompanyName", patient.InsuranceCompanyId);

        return View(patient);
    }

    // GET: Patients/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _context.Patients
            .Include(p => p.InsuranceCompany)
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(m => m.PatientId == id);

        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }

    // POST: Patients/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient != null)
        {
            try
            {
                // Hard delete - حذف نهائي
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم حذف المريض نهائياً من النظام";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "لا يمكن حذف المريض لأنه مرتبط ببيانات أخرى في النظام";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PatientExists(int id)
    {
        return _context.Patients.Any(e => e.PatientId == id);
    }

    // Test Delete Modal
    [AllowAnonymous]
    public IActionResult TestDelete()
    {
        return View();
    }
}
