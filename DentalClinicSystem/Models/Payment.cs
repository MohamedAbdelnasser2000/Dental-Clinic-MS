using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Required]
    [Display(Name = "تاريخ الدفع")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "المبلغ المدفوع")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "طريقة الدفع")]
    public string PaymentMethod { get; set; } = string.Empty; // نقدي، بطاقة، تحويل بنكي

    [StringLength(100)]
    [Display(Name = "رقم المرجع")]
    public string? ReferenceNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    [Required]
    [Display(Name = "الفاتورة")]
    public int InvoiceId { get; set; }

    [Display(Name = "المستخدم المنشئ")]
    public string? CreatedByUserId { get; set; }

    // Navigation Properties
    [ForeignKey("InvoiceId")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual User? CreatedByUser { get; set; }
}
