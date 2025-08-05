using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Treatment
{
    [Key]
    public int TreatmentId { get; set; }

    [Required]
    [Display(Name = "تاريخ العلاج")]
    public DateTime TreatmentDate { get; set; } = DateTime.Now;

    [Required]
    [StringLength(100)]
    [Display(Name = "نوع العلاج")]
    public string TreatmentType { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "وصف العلاج")]
    public string? Description { get; set; }

    [StringLength(1000)]
    [Display(Name = "الملاحظات السريرية")]
    public string? ClinicalNotes { get; set; }

    [StringLength(500)]
    [Display(Name = "الأسنان المعالجة")]
    public string? TeethTreated { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "التكلفة")]
    public decimal Cost { get; set; }

    [StringLength(50)]
    [Display(Name = "حالة العلاج")]
    public string Status { get; set; } = "مكتمل"; // مكتمل، جاري، مؤجل

    [Display(Name = "مدة العلاج (بالدقائق)")]
    public int? DurationMinutes { get; set; }

    [StringLength(500)]
    [Display(Name = "التعليمات بعد العلاج")]
    public string? PostTreatmentInstructions { get; set; }

    [StringLength(1000)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "الموعد التالي المقترح")]
    public DateTime? NextAppointmentSuggested { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "آخر تحديث")]
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    [Required]
    [Display(Name = "المريض")]
    public int PatientId { get; set; }

    [Required]
    [Display(Name = "الطبيب")]
    public int DentistId { get; set; }

    [Display(Name = "الموعد")]
    public int? AppointmentId { get; set; }

    [Display(Name = "الخدمة")]
    public int? ServiceId { get; set; }

    // Navigation Properties
    [ForeignKey("PatientId")]
    public virtual Patient? Patient { get; set; }

    [ForeignKey("DentistId")]
    public virtual Dentist? Dentist { get; set; }

    [ForeignKey("AppointmentId")]
    public virtual Appointment? Appointment { get; set; }

    [ForeignKey("ServiceId")]
    public virtual Service? Service { get; set; }

    public virtual ICollection<TreatmentFile> TreatmentFiles { get; set; } = new List<TreatmentFile>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
