using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Models;

namespace DentalClinicSystem.Data
{
    public static class SeedData
    {
        public static async Task SeedTestDataAsync(ApplicationDbContext context)
        {
            // Check if we already have test data
            if (await context.Patients.AnyAsync())
                return;

            // Add test patients
            var patients = new List<Patient>
            {
                new Patient
                {
                    FullName = "أحمد محمد علي",
                    Email = "ahmed@example.com",
                    Phone = "01234567890",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Gender = "Male",
                    Address = "القاهرة، مصر",
                    MedicalHistory = "لا توجد حساسية معروفة",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                },
                new Patient
                {
                    FullName = "فاطمة أحمد حسن",
                    Email = "fatima@example.com",
                    Phone = "01123456789",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    Gender = "Female",
                    Address = "الجيزة، مصر",
                    MedicalHistory = "حساسية من البنسلين",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                },
                new Patient
                {
                    FullName = "محمد عبد الله سالم",
                    Email = "mohamed@example.com",
                    Phone = "01012345678",
                    DateOfBirth = new DateTime(1978, 12, 10),
                    Gender = "Male",
                    Address = "الإسكندرية، مصر",
                    MedicalHistory = "مرض السكري",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                }
            };

            context.Patients.AddRange(patients);
            await context.SaveChangesAsync();

            // Add test dentist if doesn't exist
            if (!await context.Dentists.AnyAsync())
            {
                var dentist = new Dentist
                {
                    FirstName = "أحمد",
                    LastName = "محمد",
                    Email = "dentist@clinic.com",
                    Phone = "01234567890",
                    Specialization = "طب الأسنان العام",
                    LicenseNumber = "DEN001",
                    IsActive = true,
                    JoinDate = DateTime.Now
                };

                context.Dentists.Add(dentist);
                await context.SaveChangesAsync();
            }

            // Services are already seeded in DbInitializer, skip this section

            // Add today's appointments
            var today = DateTime.Today;

            // Get the first dentist and services from database
            var firstDentist = await context.Dentists.FirstOrDefaultAsync();
            var services = await context.Services.Take(3).ToListAsync();

            if (firstDentist != null && services.Any())
            {
                var appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        PatientId = patients[0].PatientId,
                        ServiceId = services[0].ServiceId,
                        DentistId = firstDentist.DentistId,
                        AppointmentDate = today,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(9, 30, 0),
                        Status = "مؤكد",
                        Notes = "فحص دوري",
                        CreatedAt = DateTime.Now
                    },
                    new Appointment
                    {
                        PatientId = patients[1].PatientId,
                        ServiceId = services.Count > 1 ? services[1].ServiceId : services[0].ServiceId,
                        DentistId = firstDentist.DentistId,
                        AppointmentDate = today,
                        StartTime = new TimeSpan(10, 30, 0),
                        EndTime = new TimeSpan(11, 15, 0),
                        Status = "مؤكد",
                        Notes = "تنظيف الأسنان",
                        CreatedAt = DateTime.Now
                    },
                    new Appointment
                    {
                        PatientId = patients[2].PatientId,
                        ServiceId = services.Count > 2 ? services[2].ServiceId : services[0].ServiceId,
                        DentistId = firstDentist.DentistId,
                        AppointmentDate = today,
                        StartTime = new TimeSpan(14, 0, 0),
                        EndTime = new TimeSpan(15, 0, 0),
                        Status = "مؤكد",
                        Notes = "حشو ضرس",
                        CreatedAt = DateTime.Now
                    }
                };

                context.Appointments.AddRange(appointments);
                await context.SaveChangesAsync();
            }

            // Add some treatments for revenue calculation
            if (services.Any())
            {
                var treatments = new List<Treatment>
                {
                    new Treatment
                    {
                        PatientId = patients[0].PatientId,
                        ServiceId = services[0].ServiceId,
                        TreatmentDate = DateTime.Now.AddDays(-5),
                        Cost = 200,
                        Notes = "فحص عام مكتمل",
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new Treatment
                    {
                        PatientId = patients[1].PatientId,
                        ServiceId = services.Count > 1 ? services[1].ServiceId : services[0].ServiceId,
                        TreatmentDate = DateTime.Now.AddDays(-3),
                        Cost = 300,
                        Notes = "تنظيف أسنان مكتمل",
                        CreatedAt = DateTime.Now.AddDays(-3)
                    }
                };

                context.Treatments.AddRange(treatments);
                await context.SaveChangesAsync();
            }
        }
    }
}
