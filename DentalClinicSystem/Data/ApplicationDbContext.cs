using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Models;

namespace DentalClinicSystem.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Dentist> Dentists { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Treatment> Treatments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    public DbSet<DentistSchedule> DentistSchedules { get; set; }
    public DbSet<PatientFile> PatientFiles { get; set; }
    public DbSet<TreatmentFile> TreatmentFiles { get; set; }
    public DbSet<Insurance> Insurance { get; set; }

    // Portfolio DbSets
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<ClinicInfo> ClinicInfos { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships and constraints
        
        // Patient relationships
        builder.Entity<Patient>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.Patients)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Patient>()
            .HasOne(p => p.InsuranceCompany)
            .WithMany(ic => ic.Patients)
            .HasForeignKey(p => p.InsuranceCompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // Dentist relationships - removed User relationship as it's not in the current model

        // Appointment relationships
        builder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Appointment>()
            .HasOne(a => a.Dentist)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DentistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Appointment>()
            .HasOne(a => a.CreatedByUser)
            .WithMany(u => u.CreatedAppointments)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Treatment relationships
        builder.Entity<Treatment>()
            .HasOne(t => t.Patient)
            .WithMany(p => p.Treatments)
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Treatment>()
            .HasOne(t => t.Dentist)
            .WithMany(d => d.Treatments)
            .HasForeignKey(t => t.DentistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoice relationships
        builder.Entity<Invoice>()
            .HasOne(i => i.Patient)
            .WithMany(p => p.Invoices)
            .HasForeignKey(i => i.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Invoice>()
            .HasOne(i => i.Treatment)
            .WithMany(t => t.Invoices)
            .HasForeignKey(i => i.TreatmentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Prescription relationships
        builder.Entity<Prescription>()
            .HasOne(pr => pr.Patient)
            .WithMany(p => p.Prescriptions)
            .HasForeignKey(pr => pr.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // PatientFile relationships
        builder.Entity<PatientFile>()
            .HasOne(pf => pf.Patient)
            .WithMany(p => p.PatientFiles)
            .HasForeignKey(pf => pf.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure decimal precision
        builder.Entity<Service>()
            .Property(s => s.Price)
            .HasPrecision(10, 2);

        builder.Entity<Invoice>()
            .Property(i => i.TotalAmount)
            .HasPrecision(10, 2);

        builder.Entity<Invoice>()
            .Property(i => i.DiscountAmount)
            .HasPrecision(10, 2);

        builder.Entity<Invoice>()
            .Property(i => i.InsuranceAmount)
            .HasPrecision(10, 2);

        builder.Entity<Invoice>()
            .Property(i => i.PaidAmount)
            .HasPrecision(10, 2);

        builder.Entity<Invoice>()
            .Property(i => i.RemainingAmount)
            .HasPrecision(10, 2);

        builder.Entity<InvoiceItem>()
            .Property(ii => ii.UnitPrice)
            .HasPrecision(10, 2);

        builder.Entity<InvoiceItem>()
            .Property(ii => ii.TotalAmount)
            .HasPrecision(10, 2);

        builder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Entity<Treatment>()
            .Property(t => t.Cost)
            .HasPrecision(10, 2);

        builder.Entity<InsuranceCompany>()
            .Property(ic => ic.CoveragePercentage)
            .HasPrecision(5, 2);

        builder.Entity<InsuranceCompany>()
            .Property(ic => ic.MaxCoverageAmount)
            .HasPrecision(10, 2);

        // Configure unique constraints
        builder.Entity<Patient>()
            .HasIndex(p => p.NationalId)
            .IsUnique()
            .HasFilter("[NationalId] IS NOT NULL");

        builder.Entity<Patient>()
            .HasIndex(p => p.PassportNumber)
            .IsUnique()
            .HasFilter("[PassportNumber] IS NOT NULL");

        builder.Entity<Invoice>()
            .HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        // Configure computed columns
        builder.Entity<InvoiceItem>()
            .Property(ii => ii.TotalAmount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]");

        builder.Entity<Invoice>()
            .Property(i => i.RemainingAmount)
            .HasComputedColumnSql("[TotalAmount] - [DiscountAmount] - [InsuranceAmount] - [PaidAmount]");

        // Portfolio configurations
        builder.Entity<Portfolio>()
            .HasIndex(p => p.DisplayOrder);

        builder.Entity<Testimonial>()
            .HasIndex(t => t.DisplayOrder);

        builder.Entity<Service>()
            .HasIndex(s => s.DisplayOrder);

        // Ensure only one ClinicInfo record
        builder.Entity<ClinicInfo>()
            .HasData(new ClinicInfo
            {
                ClinicInfoId = 1,
                ClinicName = "Dental Excellence Clinic",
                Description = "Your trusted partner for comprehensive dental care",
                UpdatedAt = DateTime.Now
            });
    }
}
