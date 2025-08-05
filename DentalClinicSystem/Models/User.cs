using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DentalClinicSystem.Models;

public class User : IdentityUser
{
    [Required]
    [StringLength(100)]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "تاريخ الميلاد")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(10)]
    [Display(Name = "النوع")]
    public string? Gender { get; set; }

    [StringLength(500)]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "آخر تحديث")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public virtual ICollection<Dentist> Dentists { get; set; } = new List<Dentist>();
    public virtual ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
}
