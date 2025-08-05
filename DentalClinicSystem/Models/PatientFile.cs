using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class PatientFile
{
    [Key]
    public int FileId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "اسم الملف")]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "مسار الملف")]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "نوع الملف")]
    public string? FileType { get; set; }

    [Display(Name = "حجم الملف")]
    public long FileSize { get; set; }

    [StringLength(100)]
    [Display(Name = "فئة الملف")]
    public string? Category { get; set; } // أشعة، تقارير، صور...

    [StringLength(500)]
    [Display(Name = "وصف")]
    public string? Description { get; set; }

    [Display(Name = "تاريخ الرفع")]
    public DateTime UploadDate { get; set; } = DateTime.Now;

    // Foreign Keys
    [Required]
    [Display(Name = "المريض")]
    public int PatientId { get; set; }

    [Display(Name = "المستخدم الرافع")]
    public string? UploadedByUserId { get; set; }

    // Navigation Properties
    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("UploadedByUserId")]
    public virtual User? UploadedByUser { get; set; }
}

public class TreatmentFile
{
    [Key]
    public int FileId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "اسم الملف")]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "مسار الملف")]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "نوع الملف")]
    public string? FileType { get; set; }

    [Display(Name = "حجم الملف")]
    public long FileSize { get; set; }

    [StringLength(500)]
    [Display(Name = "وصف")]
    public string? Description { get; set; }

    [Display(Name = "تاريخ الرفع")]
    public DateTime UploadDate { get; set; } = DateTime.Now;

    // Foreign Keys
    [Required]
    [Display(Name = "العلاج")]
    public int TreatmentId { get; set; }

    [Display(Name = "المستخدم الرافع")]
    public string? UploadedByUserId { get; set; }

    // Navigation Properties
    [ForeignKey("TreatmentId")]
    public virtual Treatment Treatment { get; set; } = null!;

    [ForeignKey("UploadedByUserId")]
    public virtual User? UploadedByUser { get; set; }
}
