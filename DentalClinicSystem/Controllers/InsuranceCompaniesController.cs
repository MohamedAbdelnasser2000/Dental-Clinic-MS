using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InsuranceCompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsuranceCompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InsuranceCompanies
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            var companies = _context.InsuranceCompanies.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(c => c.CompanyName.Contains(searchString) ||
                                               c.Email.Contains(searchString) ||
                                               c.PhoneNumber.Contains(searchString));
            }

            var totalCount = await companies.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var companiesList = await companies
                .OrderBy(c => c.CompanyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(companiesList);
        }

        // GET: InsuranceCompanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.InsuranceCompanies
                .FirstOrDefaultAsync(m => m.InsuranceCompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            // Get patients using this insurance
            var patients = await _context.Patients
                .Where(p => p.InsuranceCompanyId == id)
                .Take(10)
                .ToListAsync();

            ViewBag.Patients = patients;

            return View(company);
        }

        // GET: InsuranceCompanies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InsuranceCompanies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyName,PhoneNumber,Email,Address,CoveragePercentage,MaxCoverageAmount,IsActive,Notes")] InsuranceCompany company)
        {
            if (ModelState.IsValid)
            {
                company.CreatedAt = DateTime.Now;
                _context.Add(company);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Insurance company created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: InsuranceCompanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.InsuranceCompanies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: InsuranceCompanies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InsuranceCompanyId,CompanyName,PhoneNumber,Email,Address,CoveragePercentage,MaxCoverageAmount,IsActive,Notes,CreatedAt")] InsuranceCompany company)
        {
            if (id != company.InsuranceCompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Insurance company updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceCompanyExists(company.InsuranceCompanyId))
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
            return View(company);
        }

        // GET: InsuranceCompanies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.InsuranceCompanies
                .FirstOrDefaultAsync(m => m.InsuranceCompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: InsuranceCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.InsuranceCompanies.FindAsync(id);
            if (company != null)
            {
                // Check if any patients are using this insurance
                var patientsCount = await _context.Patients.CountAsync(p => p.InsuranceCompanyId == id);
                if (patientsCount > 0)
                {
                    TempData["ErrorMessage"] = "Cannot delete insurance company. It is being used by patients.";
                    return RedirectToAction(nameof(Index));
                }

                _context.InsuranceCompanies.Remove(company);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Insurance company deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceCompanyExists(int id)
        {
            return _context.InsuranceCompanies.Any(e => e.InsuranceCompanyId == id);
        }
    }
}
