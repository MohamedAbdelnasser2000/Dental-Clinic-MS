using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models;

public class Patient
{
    [Key]
    public int PatientId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "الرقم القومي")]
    public string? NationalId { get; set; }

    [StringLength(20)]
    [Display(Name = "رقم جواز السفر")]
    public string? PassportNumber { get; set; }

    [Display(Name = "تاريخ الميلاد")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(10)]
    [Display(Name = "النوع")]
    public string? Gender { get; set; }

    [StringLength(20)]
    [Display(Name = "رقم الهاتف")]
    public string? PhoneNumber { get; set; }

    [StringLength(20)]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [Display(Name = "البريد الإلكتروني")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [StringLength(1000)]
    [Display(Name = "التاريخ المرضي")]
    public string? MedicalHistory { get; set; }

    [StringLength(500)]
    [Display(Name = "الحساسية من الأدوية")]
    public string? Allergies { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ التسجيل")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    [Display(Name = "آخر زيارة")]
    public DateTime? LastVisit { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    // Foreign Keys
    [Display(Name = "المستخدم المنشئ")]
    public string? CreatedByUserId { get; set; }

    [Display(Name = "شركة التأمين")]
    public int? InsuranceCompanyId { get; set; }

    // Navigation Properties
    [ForeignKey("CreatedByUserId")]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey("InsuranceCompanyId")]
    public virtual InsuranceCompany? InsuranceCompany { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<PatientFile> PatientFiles { get; set; } = new List<PatientFile>();
}
