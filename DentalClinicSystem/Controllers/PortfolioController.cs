using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PortfolioController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Portfolio
        public async Task<IActionResult> Index(string searchString = "", string category = "", int page = 1, int pageSize = 10)
        {
            var query = _context.Portfolios.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Title.Contains(searchString) || p.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var totalItems = await query.CountAsync();
            var portfolios = await query
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.Category = category;
            ViewBag.Categories = await _context.Portfolios
                .Where(p => !string.IsNullOrEmpty(p.Category))
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.PageSize = pageSize;

            return View(portfolios);
        }

        // GET: Portfolio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(m => m.PortfolioId == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // GET: Portfolio/Create
        public IActionResult Create()
        {
            ViewBag.Categories = GetCategories();
            return View();
        }

        // POST: Portfolio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,PatientAge,TreatmentType,TechnicalDetails,IsActive,IsFeatured,DisplayOrder")] Portfolio portfolio, IFormFile? imageFile, IFormFile? beforeImageFile, IFormFile? afterImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image uploads
                if (imageFile != null)
                {
                    portfolio.ImagePath = await SaveImageAsync(imageFile, "portfolio");
                }

                if (beforeImageFile != null)
                {
                    portfolio.BeforeImagePath = await SaveImageAsync(beforeImageFile, "portfolio/before");
                }

                if (afterImageFile != null)
                {
                    portfolio.AfterImagePath = await SaveImageAsync(afterImageFile, "portfolio/after");
                }

                portfolio.CreatedAt = DateTime.Now;
                _context.Add(portfolio);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Portfolio item created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = GetCategories();
            return View(portfolio);
        }

        // GET: Portfolio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            ViewBag.Categories = GetCategories();
            return View(portfolio);
        }

        // POST: Portfolio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PortfolioId,Title,Description,Category,PatientAge,TreatmentType,TechnicalDetails,IsActive,IsFeatured,DisplayOrder,ImagePath,BeforeImagePath,AfterImagePath,CreatedAt")] Portfolio portfolio, IFormFile? imageFile, IFormFile? beforeImageFile, IFormFile? afterImageFile)
        {
            if (id != portfolio.PortfolioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image uploads
                    if (imageFile != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(portfolio.ImagePath))
                        {
                            DeleteImage(portfolio.ImagePath);
                        }
                        portfolio.ImagePath = await SaveImageAsync(imageFile, "portfolio");
                    }

                    if (beforeImageFile != null)
                    {
                        if (!string.IsNullOrEmpty(portfolio.BeforeImagePath))
                        {
                            DeleteImage(portfolio.BeforeImagePath);
                        }
                        portfolio.BeforeImagePath = await SaveImageAsync(beforeImageFile, "portfolio/before");
                    }

                    if (afterImageFile != null)
                    {
                        if (!string.IsNullOrEmpty(portfolio.AfterImagePath))
                        {
                            DeleteImage(portfolio.AfterImagePath);
                        }
                        portfolio.AfterImagePath = await SaveImageAsync(afterImageFile, "portfolio/after");
                    }

                    portfolio.UpdatedAt = DateTime.Now;
                    _context.Update(portfolio);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Portfolio item updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioExists(portfolio.PortfolioId))
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

            ViewBag.Categories = GetCategories();
            return View(portfolio);
        }

        // GET: Portfolio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(m => m.PortfolioId == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // POST: Portfolio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio != null)
            {
                // Delete associated images
                if (!string.IsNullOrEmpty(portfolio.ImagePath))
                {
                    DeleteImage(portfolio.ImagePath);
                }
                if (!string.IsNullOrEmpty(portfolio.BeforeImagePath))
                {
                    DeleteImage(portfolio.BeforeImagePath);
                }
                if (!string.IsNullOrEmpty(portfolio.AfterImagePath))
                {
                    DeleteImage(portfolio.AfterImagePath);
                }

                _context.Portfolios.Remove(portfolio);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Portfolio item deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolios.Any(e => e.PortfolioId == id);
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

        private List<string> GetCategories()
        {
            return new List<string>
            {
                "Before/After",
                "Dental Procedures",
                "Equipment",
                "Clinic Interior",
                "Team",
                "Certificates",
                "Awards"
            };
        }
    }
}
