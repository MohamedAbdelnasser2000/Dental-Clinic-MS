using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index(string searchString, int? patientId, int? dentistId, int page = 1, int pageSize = 10)
        {
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Dentist)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                prescriptions = prescriptions.Where(p => p.Medication.Contains(searchString) ||
                                                       p.Patient.FullName.Contains(searchString) ||
                                                       p.Dentist.FullName.Contains(searchString));
            }

            if (patientId.HasValue)
            {
                prescriptions = prescriptions.Where(p => p.PatientId == patientId.Value);
            }

            if (dentistId.HasValue)
            {
                prescriptions = prescriptions.Where(p => p.DentistId == dentistId.Value);
            }

            var totalCount = await prescriptions.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var prescriptionsList = await prescriptions
                .OrderByDescending(p => p.PrescriptionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.PatientId = patientId;
            ViewBag.DentistId = dentistId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName");
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName");

            return View(prescriptionsList);
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Dentist)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // GET: Prescriptions/Create
        public async Task<IActionResult> Create(int? patientId)
        {
            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", patientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName");

            var prescription = new Prescription
            {
                PrescriptionDate = DateTime.Now
            };

            if (patientId.HasValue)
            {
                prescription.PatientId = patientId.Value;
            }

            return View(prescription);
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DentistId,PrescriptionDate,Medication,Dosage,Instructions,Notes")] Prescription prescription)
        {
            // Remove navigation properties from ModelState
            ModelState.Remove("Patient");
            ModelState.Remove("Dentist");
            ModelState.Remove("Treatment");

            if (ModelState.IsValid)
            {
                prescription.CreatedAt = DateTime.Now;
                _context.Add(prescription);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Prescription created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", prescription.PatientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName", prescription.DentistId);
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", prescription.PatientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName", prescription.DentistId);
            ViewBag.Treatments = new SelectList(await _context.Treatments.ToListAsync(), "TreatmentId", "Description", prescription.TreatmentId);
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrescriptionId,PatientId,DentistId,PrescriptionDate,Medication,Dosage,Instructions,Notes,CreatedAt")] Prescription prescription)
        {
            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }

            // Remove navigation properties from ModelState
            ModelState.Remove("Patient");
            ModelState.Remove("Dentist");
            ModelState.Remove("Treatment");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Prescription updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.PrescriptionId))
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

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", prescription.PatientId);
            ViewBag.Dentists = new SelectList(await _context.Dentists.Where(d => d.IsActive).ToListAsync(), "DentistId", "FullName", prescription.DentistId);
            return View(prescription);
        }

        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Dentist)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                _context.Prescriptions.Remove(prescription);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Prescription deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.PrescriptionId == id);
        }
    }
}
