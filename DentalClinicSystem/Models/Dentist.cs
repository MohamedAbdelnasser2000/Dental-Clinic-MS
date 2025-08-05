using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Dentist
{
    [Key]
    public int DentistId { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}".Trim();

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Specialization")]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "License Number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [Display(Name = "Years of Experience")]
    public int? YearsOfExperience { get; set; }

    [StringLength(500)]
    [Display(Name = "Qualifications")]
    public string? Qualifications { get; set; }

    [Display(Name = "Photo Path")]
    public string? PhotoPath { get; set; }

    [Display(Name = "Join Date")]
    public DateTime JoinDate { get; set; } = DateTime.Now;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
