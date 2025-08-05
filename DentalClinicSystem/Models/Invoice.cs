using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "رقم الفاتورة")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "تاريخ الفاتورة")]
    public DateTime InvoiceDate { get; set; } = DateTime.Now;

    [Required]
    [Display(Name = "تاريخ الاستحقاق")]
    public DateTime DueDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "المبلغ الإجمالي")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "مبلغ الخصم")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "مبلغ التأمين")]
    public decimal InsuranceAmount { get; set; } = 0;

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "المبلغ المدفوع")]
    public decimal PaidAmount { get; set; } = 0;

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "المبلغ المتبقي")]
    public decimal RemainingAmount { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "حالة الفاتورة")]
    public string Status { get; set; } = "غير مدفوعة"; // مدفوعة، غير مدفوعة، مدفوعة جزئياً

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "آخر تحديث")]
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    [Required]
    [Display(Name = "المريض")]
    public int PatientId { get; set; }

    [Display(Name = "العلاج")]
    public int? TreatmentId { get; set; }

    [Display(Name = "المستخدم المنشئ")]
    public string? CreatedByUserId { get; set; }

    // Navigation Properties
    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("TreatmentId")]
    public virtual Treatment? Treatment { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual User? CreatedByUser { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
