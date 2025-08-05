using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;

namespace DentalClinicSystem.Controllers
{
    public class PublicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Public/Portfolio
        public async Task<IActionResult> Portfolio()
        {
            var clinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();
            var featuredPortfolios = await _context.Portfolios
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderBy(p => p.DisplayOrder)
                .Take(6)
                .ToListAsync();

            var portfoliosByCategory = await _context.Portfolios
                .Where(p => p.IsActive)
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, Items = g.OrderBy(p => p.DisplayOrder).ToList() })
                .ToListAsync();

            var testimonials = await _context.Testimonials
                .Where(t => t.IsActive && t.IsApproved)
                .OrderBy(t => t.DisplayOrder)
                .Take(6)
                .ToListAsync();

            var services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            ViewBag.ClinicInfo = clinicInfo ?? new ClinicInfo { ClinicName = "Dental Excellence Clinic" };
            ViewBag.FeaturedPortfolios = featuredPortfolios;
            ViewBag.PortfoliosByCategory = portfoliosByCategory;
            ViewBag.Testimonials = testimonials;
            ViewBag.Services = services;

            return View();
        }

        // GET: Public/Gallery
        public async Task<IActionResult> Gallery(string category = "")
        {
            var query = _context.Portfolios.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var portfolios = await query
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            var categories = await _context.Portfolios
                .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Category))
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.ClinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();

            return View(portfolios);
        }

        // GET: Public/Services
        public async Task<IActionResult> Services()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            var clinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();

            ViewBag.ClinicInfo = clinicInfo;
            return View(services);
        }

        // GET: Public/About
        public async Task<IActionResult> About()
        {
            var clinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();
            var dentists = await _context.Dentists
                .Where(d => d.IsActive)
                .ToListAsync();

            ViewBag.ClinicInfo = clinicInfo;
            ViewBag.Dentists = dentists;

            return View();
        }

        // GET: Public/Contact
        public async Task<IActionResult> Contact()
        {
            var clinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();
            ViewBag.ClinicInfo = clinicInfo;

            return View();
        }

        // POST: Public/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string phone, string message)
        {
            if (ModelState.IsValid)
            {
                // Here you can implement email sending logic
                // For now, we'll just show a success message
                TempData["SuccessMessage"] = "Thank you for your message! We will get back to you soon.";
                return RedirectToAction(nameof(Contact));
            }

            var clinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();
            ViewBag.ClinicInfo = clinicInfo;
            return View();
        }

        // GET: Public/PortfolioDetails/5
        public async Task<IActionResult> PortfolioDetails(int id)
        {
            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(p => p.PortfolioId == id && p.IsActive);

            if (portfolio == null)
            {
                return NotFound();
            }

            // Get related portfolios from the same category
            var relatedPortfolios = await _context.Portfolios
                .Where(p => p.IsActive && p.Category == portfolio.Category && p.PortfolioId != id)
                .OrderBy(p => p.DisplayOrder)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedPortfolios = relatedPortfolios;
            ViewBag.ClinicInfo = await _context.ClinicInfos.FirstOrDefaultAsync();

            return View(portfolio);
        }
    }
}
