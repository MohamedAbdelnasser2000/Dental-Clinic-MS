using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicSystem.Models
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImagePath { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty; // Before/After, Procedures, Equipment, etc.

        public string? BeforeImagePath { get; set; }
        public string? AfterImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        [StringLength(100)]
        public string? PatientAge { get; set; }

        [StringLength(200)]
        public string? TreatmentType { get; set; }

        public string? TechnicalDetails { get; set; }
    }

    public class ClinicInfo
    {
        public int ClinicInfoId { get; set; }

        [Required]
        [StringLength(200)]
        public string ClinicName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Mission { get; set; }
        public string? Vision { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public string? WorkingHours { get; set; }

        [StringLength(500)]
        public string? LogoPath { get; set; }

        [StringLength(500)]
        public string? HeroImagePath { get; set; }

        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedInUrl { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string? UpdatedByUserId { get; set; }
    }

    public class Testimonial
    {
        public int TestimonialId { get; set; }

        [Required]
        [StringLength(100)]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [StringLength(200)]
        public string? TreatmentType { get; set; }

        [StringLength(500)]
        public string? PatientImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }


}
