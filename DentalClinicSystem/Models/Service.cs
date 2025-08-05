using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Service
{
    [Key]
    public int ServiceId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "اسم الخدمة")]
    public string ServiceName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "وصف الخدمة")]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "السعر")]
    public decimal Price { get; set; }

    [StringLength(50)]
    [Display(Name = "الفئة")]
    public string? Category { get; set; }

    [Display(Name = "المدة المتوقعة (بالدقائق)")]
    public int? EstimatedDurationMinutes { get; set; }

    [Display(Name = "Duration (minutes)")]
    public int Duration { get; set; } = 60;

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "آخر تحديث")]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(500)]
    [Display(Name = "مسار الصورة")]
    public string? ImagePath { get; set; }

    [Display(Name = "شائع")]
    public bool IsPopular { get; set; } = false;

    [Display(Name = "ترتيب العرض")]
    public int DisplayOrder { get; set; } = 0;

    // Navigation Properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
