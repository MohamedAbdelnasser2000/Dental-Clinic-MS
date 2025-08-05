using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class DentistSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "يوم الأسبوع")]
    public string DayOfWeek { get; set; } = string.Empty; // الأحد، الاثنين، الثلاثاء...

    [Required]
    [Display(Name = "وقت البداية")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Display(Name = "وقت النهاية")]
    public TimeSpan EndTime { get; set; }

    [Display(Name = "مدة الموعد (بالدقائق)")]
    public int AppointmentDurationMinutes { get; set; } = 30;

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    // Foreign Key
    [Required]
    [Display(Name = "الطبيب")]
    public int DentistId { get; set; }

    // Navigation Property
    [ForeignKey("DentistId")]
    public virtual Dentist Dentist { get; set; } = null!;
}
