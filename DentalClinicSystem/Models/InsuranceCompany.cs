using System.ComponentModel.DataAnnotations;

namespace DentalClinicSystem.Models;

public class InsuranceCompany
{
    [Key]
    public int InsuranceCompanyId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "اسم شركة التأمين")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "رقم الهاتف")]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    [Display(Name = "البريد الإلكتروني")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [Display(Name = "نسبة التغطية")]
    public decimal CoveragePercentage { get; set; } = 0;

    [Display(Name = "الحد الأقصى للتغطية")]
    public decimal? MaxCoverageAmount { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
