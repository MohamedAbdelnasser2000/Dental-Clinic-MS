using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    [Required]
    [Display(Name = "تاريخ الموعد")]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Display(Name = "وقت البداية")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Display(Name = "وقت النهاية")]
    public TimeSpan EndTime { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "حالة الموعد")]
    public string Status { get; set; } = "مؤكد"; // مؤكد، ملغي، تم، لم يحضر

    [StringLength(500)]
    [Display(Name = "سبب الزيارة")]
    public string? ReasonForVisit { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "آخر تحديث")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "تم إرسال تذكير")]
    public bool ReminderSent { get; set; } = false;

    // Foreign Keys
    [Required]
    [Display(Name = "المريض")]
    public int PatientId { get; set; }

    [Required]
    [Display(Name = "الطبيب")]
    public int DentistId { get; set; }

    [Display(Name = "المستخدم المنشئ")]
    public string? CreatedByUserId { get; set; }

    [Display(Name = "الخدمة")]
    public int? ServiceId { get; set; }

    // Navigation Properties
    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("DentistId")]
    public virtual Dentist Dentist { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("ServiceId")]
    public virtual Service? Service { get; set; }

    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
