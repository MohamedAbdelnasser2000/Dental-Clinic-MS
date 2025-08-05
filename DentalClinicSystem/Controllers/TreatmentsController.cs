using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize]
    public class TreatmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TreatmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Treatments
        public async Task<IActionResult> Index(string searchString, int? patientId, int? dentistId, string status, int page = 1, int pageSize = 10)
        {
            var treatments = _context.Treatments
                .Include(t => t.Patient)
                .Include(t => t.Dentist)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                treatments = treatments.Where(t => t.TreatmentType.Contains(searchString) ||
                                                 t.Patient.FullName.Contains(searchString) ||
                                                 t.Dentist.FullName.Contains(searchString));
            }

            if (patientId.HasValue)
            {
                treatments = treatments.Where(t => t.PatientId == patientId.Value);
            }

            if (dentistId.HasValue)
            {
                treatments = treatments.Where(t => t.DentistId == dentistId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                treatments = treatments.Where(t => t.Status == status);
            }

            var totalCount = await treatments.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var treatmentsList = await treatments
                .OrderByDescending(t => t.TreatmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.PatientId = patientId;
            ViewBag.DentistId = dentistId;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName");
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName");

            return View(treatmentsList);
        }

        // GET: Treatments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .Include(t => t.Patient)
                .Include(t => t.Dentist)
                .FirstOrDefaultAsync(m => m.TreatmentId == id);

            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // GET: Treatments/Create
        public async Task<IActionResult> Create(int? patientId)
        {
            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", patientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName");
            
            var treatment = new Treatment();
            if (patientId.HasValue)
            {
                treatment.PatientId = patientId.Value;
            }
            
            return View(treatment);
        }

        // POST: Treatments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DentistId,TreatmentType,TreatmentDate,Cost,Status,Notes")] Treatment treatment)
        {
            try
            {
                // Debug: Log the received data
                Console.WriteLine($"Received treatment data: PatientId={treatment.PatientId}, DentistId={treatment.DentistId}, TreatmentType={treatment.TreatmentType}, Date={treatment.TreatmentDate}, Cost={treatment.Cost}");

                // Remove validation errors for navigation properties
                ModelState.Remove("Patient");
                ModelState.Remove("Dentist");

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
                    treatment.CreatedAt = DateTime.Now;
                    _context.Add(treatment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Treatment created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating treatment: {ex.Message}");
                ModelState.AddModelError("", "حدث خطأ أثناء حفظ العلاج. يرجى المحاولة مرة أخرى.");
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", treatment.PatientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName", treatment.DentistId);
            return View(treatment);
        }

        // GET: Treatments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", treatment.PatientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName", treatment.DentistId);
            return View(treatment);
        }

        // POST: Treatments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentId,PatientId,DentistId,TreatmentType,TreatmentDate,Cost,Status,Notes")] Treatment treatment)
        {
            if (id != treatment.TreatmentId)
            {
                return NotFound();
            }

            // Remove validation errors for navigation properties
            ModelState.Remove("Patient");
            ModelState.Remove("Dentist");
            ModelState.Remove("Appointment");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                try
                {
                    treatment.UpdatedAt = DateTime.Now;
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "تم تحديث العلاج بنجاح!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.TreatmentId))
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

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", treatment.PatientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName", treatment.DentistId);
            return View(treatment);
        }

        // GET: Treatments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .Include(t => t.Patient)
                .Include(t => t.Dentist)
                .FirstOrDefaultAsync(m => m.TreatmentId == id);

            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var treatment = await _context.Treatments.FindAsync(id);
                if (treatment != null)
                {
                    Console.WriteLine($"Deleting treatment: ID={treatment.TreatmentId}, Type={treatment.TreatmentType}, Patient={treatment.PatientId}");

                    // Hard delete - remove completely from database
                    _context.Treatments.Remove(treatment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم حذف العلاج نهائياً من قاعدة البيانات!";
                    Console.WriteLine("Treatment deleted successfully from database");
                }
                else
                {
                    TempData["ErrorMessage"] = "العلاج غير موجود!";
                    Console.WriteLine($"Treatment with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting treatment: {ex.Message}");
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف العلاج. يرجى المحاولة مرة أخرى.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(int id)
        {
            return _context.Treatments.Any(e => e.TreatmentId == id);
        }
    }
}
