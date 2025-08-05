using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Prescription
{
    [Key]
    public int PrescriptionId { get; set; }

    [Required]
    [Display(Name = "تاريخ الوصفة")]
    public DateTime PrescriptionDate { get; set; } = DateTime.Now;

    [Required]
    [StringLength(100)]
    [Display(Name = "Medication")]
    public string Medication { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Dosage")]
    public string Dosage { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Instructions")]
    public string? Instructions { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    [Required]
    [Display(Name = "المريض")]
    public int PatientId { get; set; }

    [Required]
    [Display(Name = "الطبيب")]
    public int DentistId { get; set; }

    [Display(Name = "العلاج")]
    public int? TreatmentId { get; set; }

    // Navigation Properties
    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("DentistId")]
    public virtual Dentist Dentist { get; set; } = null!;

    [ForeignKey("TreatmentId")]
    public virtual Treatment? Treatment { get; set; }

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}

public class PrescriptionItem
{
    [Key]
    public int PrescriptionItemId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "اسم الدواء")]
    public string MedicineName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "الجرعة")]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "التكرار")]
    public string Frequency { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "المدة")]
    public string Duration { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "التعليمات")]
    public string? Instructions { get; set; }

    // Foreign Key
    [Required]
    [Display(Name = "الوصفة")]
    public int PrescriptionId { get; set; }

    // Navigation Property
    [ForeignKey("PrescriptionId")]
    public virtual Prescription Prescription { get; set; } = null!;
}
