using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicSystem.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index(string searchString, int? patientId, string status, int page = 1, int pageSize = 10)
        {
            var invoices = _context.Invoices
                .Include(i => i.Patient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                invoices = invoices.Where(i => i.InvoiceNumber.Contains(searchString) ||
                                             i.Patient.FullName.Contains(searchString));
            }

            if (patientId.HasValue)
            {
                invoices = invoices.Where(i => i.PatientId == patientId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                invoices = invoices.Where(i => i.Status == status);
            }

            var totalCount = await invoices.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var invoicesList = await invoices
                .OrderByDescending(i => i.InvoiceDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.PatientId = patientId;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName");

            return View(invoicesList);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create(int? patientId)
        {
            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", patientId);
            ViewBag.Treatments = new SelectList(await _context.Treatments.Include(t => t.Patient).ToListAsync(), "TreatmentId", "TreatmentType");

            var invoice = new Invoice
            {
                InvoiceDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30), // Default due date 30 days from now
                InvoiceNumber = GenerateInvoiceNumber()
            };

            if (patientId.HasValue)
            {
                invoice.PatientId = patientId.Value;
            }

            return View(invoice);
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,TreatmentId,InvoiceNumber,InvoiceDate,DueDate,TotalAmount,DiscountAmount,InsuranceAmount,PaidAmount,Status,Notes")] Invoice invoice)
        {
            // Remove navigation properties from ModelState
            ModelState.Remove("Patient");
            ModelState.Remove("Treatment");
            ModelState.Remove("CreatedByUser");

            // Calculate remaining amount
            invoice.RemainingAmount = invoice.TotalAmount - invoice.DiscountAmount - invoice.InsuranceAmount - invoice.PaidAmount;
            invoice.CreatedAt = DateTime.Now;

            // Debug ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                TempData["ErrorMessage"] = $"Validation errors: {string.Join(", ", errors.SelectMany(e => e.Errors))}";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(invoice);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Invoice created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error saving invoice: {ex.Message}";
                }
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", invoice.PatientId);
            ViewBag.Treatments = new SelectList(await _context.Treatments.Include(t => t.Patient).ToListAsync(), "TreatmentId", "TreatmentType", invoice.TreatmentId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", invoice.PatientId);
            ViewBag.Treatments = new SelectList(await _context.Treatments.Include(t => t.Patient).ToListAsync(), "TreatmentId", "TreatmentType", invoice.TreatmentId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceId,PatientId,TreatmentId,InvoiceNumber,InvoiceDate,DueDate,TotalAmount,DiscountAmount,InsuranceAmount,PaidAmount,Status,Notes,CreatedAt,CreatedByUserId")] Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            // Remove navigation properties from ModelState
            ModelState.Remove("Patient");
            ModelState.Remove("Treatment");
            ModelState.Remove("CreatedByUser");

            // Calculate remaining amount
            invoice.RemainingAmount = invoice.TotalAmount - invoice.DiscountAmount - invoice.InsuranceAmount - invoice.PaidAmount;
            invoice.UpdatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Invoice updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceId))
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

            ViewBag.Patients = new SelectList(await _context.Patients.Where(p => p.IsActive).ToListAsync(), "PatientId", "FullName", invoice.PatientId);
            ViewBag.Treatments = new SelectList(await _context.Treatments.Include(t => t.Patient).ToListAsync(), "TreatmentId", "TreatmentType", invoice.TreatmentId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Invoice deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Invoices/Print/5
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Treatment)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }

        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _context.Invoices
                .OrderByDescending(i => i.InvoiceId)
                .FirstOrDefault();

            var nextNumber = (lastInvoice?.InvoiceId ?? 0) + 1;
            return $"INV-{DateTime.Now.Year}-{nextNumber:D4}";
        }
    }
}
