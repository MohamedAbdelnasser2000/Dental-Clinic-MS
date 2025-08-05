using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize]
    public class DentistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DentistsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Dentists
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalDentists = await _context.Dentists.Where(d => d.IsActive).CountAsync();
            var dentists = await _context.Dentists
                .Where(d => d.IsActive)
                .OrderBy(d => d.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalDentists / pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.TotalDentists = totalDentists;

            return View(dentists);
        }

        // GET: Dentists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dentist = await _context.Dentists
                .FirstOrDefaultAsync(m => m.DentistId == id);
            if (dentist == null)
            {
                return NotFound();
            }

            return View(dentist);
        }

        // GET: Dentists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dentists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Phone,Specialization,LicenseNumber,Qualifications,YearsOfExperience,Address,DateOfBirth,Gender")] Dentist dentist, IFormFile? PhotoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle photo upload
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    dentist.PhotoPath = await SaveImageAsync(PhotoFile, "dentists");
                }

                dentist.JoinDate = DateTime.Now;
                dentist.IsActive = true;
                _context.Add(dentist);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم إضافة الطبيب بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(dentist);
        }

        // GET: Dentists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dentist = await _context.Dentists.FindAsync(id);
            if (dentist == null)
            {
                return NotFound();
            }
            return View(dentist);
        }

        // POST: Dentists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DentistId,FirstName,LastName,Email,Phone,Specialization,LicenseNumber,Qualifications,YearsOfExperience,Address,DateOfBirth,Gender,JoinDate,IsActive,PhotoPath")] Dentist dentist, IFormFile? PhotoFile)
        {
            if (id != dentist.DentistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle photo upload
                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        // Delete old photo if exists
                        if (!string.IsNullOrEmpty(dentist.PhotoPath))
                        {
                            DeleteImage(dentist.PhotoPath);
                        }
                        dentist.PhotoPath = await SaveImageAsync(PhotoFile, "dentists");
                    }

                    _context.Update(dentist);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "تم تحديث بيانات الطبيب بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DentistExists(dentist.DentistId))
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
            return View(dentist);
        }

        // GET: Dentists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dentist = await _context.Dentists
                .FirstOrDefaultAsync(m => m.DentistId == id);
            if (dentist == null)
            {
                return NotFound();
            }

            return View(dentist);
        }

        // POST: Dentists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dentist = await _context.Dentists.FindAsync(id);
            if (dentist != null)
            {
                dentist.IsActive = false; // Soft delete
                _context.Update(dentist);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم حذف الطبيب بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DentistExists(int id)
        {
            return _context.Dentists.Any(e => e.DentistId == id);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile, string folder)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/images/{folder}/{uniqueFileName}";
        }

        private void DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
    }
}
