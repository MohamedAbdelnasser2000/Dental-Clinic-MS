using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize]
    public class InsuranceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsuranceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Insurance
        public async Task<IActionResult> Index(string searchString, string providerName, string status, int page = 1, int pageSize = 10)
        {
            var insurance = _context.Insurance
                .Include(i => i.Patient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                insurance = insurance.Where(i => i.PolicyNumber.Contains(searchString) ||
                                               i.Patient.FullName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(providerName))
            {
                insurance = insurance.Where(i => i.ProviderName == providerName);
            }

            if (!string.IsNullOrEmpty(status))
            {
                var now = DateTime.Now;
                switch (status)
                {
                    case "Active":
                        insurance = insurance.Where(i => i.IsActive && i.ExpiryDate > now);
                        break;
                    case "Expired":
                        insurance = insurance.Where(i => i.ExpiryDate < now);
                        break;
                    case "Expiring":
                        insurance = insurance.Where(i => i.ExpiryDate < now.AddDays(30) && i.ExpiryDate > now);
                        break;
                }
            }

            var totalCount = await insurance.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var insuranceList = await insurance
                .OrderByDescending(i => i.ExpiryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.ProviderName = providerName;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(insuranceList);
        }

        // GET: Insurance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurance
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(m => m.InsuranceId == id);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // GET: Insurance/Create
        public async Task<IActionResult> Create(int? patientId)
        {
            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", patientId);
            
            var insurance = new Insurance
            {
                StartDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                IsActive = true
            };
            
            if (patientId.HasValue)
            {
                insurance.PatientId = patientId.Value;
            }
            
            return View(insurance);
        }

        // POST: Insurance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,ProviderName,PolicyNumber,StartDate,ExpiryDate,CoveragePercentage,MaxCoverageAmount,ProviderPhone,ProviderEmail,Notes,IsActive")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insurance);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Insurance policy created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", insurance.PatientId);
            return View(insurance);
        }

        // GET: Insurance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurance.FindAsync(id);
            if (insurance == null)
            {
                return NotFound();
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", insurance.PatientId);
            return View(insurance);
        }

        // POST: Insurance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InsuranceId,PatientId,ProviderName,PolicyNumber,StartDate,ExpiryDate,CoveragePercentage,MaxCoverageAmount,ProviderPhone,ProviderEmail,Notes,IsActive")] Insurance insurance)
        {
            if (id != insurance.InsuranceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insurance);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Insurance policy updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceExists(insurance.InsuranceId))
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

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", insurance.PatientId);
            return View(insurance);
        }

        // GET: Insurance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurance
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(m => m.InsuranceId == id);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // POST: Insurance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurance = await _context.Insurance.FindAsync(id);
            if (insurance != null)
            {
                _context.Insurance.Remove(insurance);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Insurance policy deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceExists(int id)
        {
            return _context.Insurance.Any(e => e.InsuranceId == id);
        }
    }
}
