using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models
{
    public class Insurance
    {
        [Key]
        public int InsuranceId { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Policy Number")]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Coverage Percentage")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal CoveragePercentage { get; set; }

        [Display(Name = "Maximum Coverage Amount")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxCoverageAmount { get; set; }

        [StringLength(20)]
        [Display(Name = "Provider Phone")]
        public string? ProviderPhone { get; set; }

        [StringLength(100)]
        [Display(Name = "Provider Email")]
        [EmailAddress]
        public string? ProviderEmail { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual Patient Patient { get; set; } = null!;
    }
}
