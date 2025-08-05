using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class InvoiceItem
{
    [Key]
    public int InvoiceItemId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "وصف العنصر")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "الكمية")]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "سعر الوحدة")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "المبلغ الإجمالي")]
    public decimal TotalAmount { get; set; }

    // Foreign Keys
    [Required]
    [Display(Name = "الفاتورة")]
    public int InvoiceId { get; set; }

    [Display(Name = "الخدمة")]
    public int? ServiceId { get; set; }

    // Navigation Properties
    [ForeignKey("InvoiceId")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("ServiceId")]
    public virtual Service? Service { get; set; }
}
