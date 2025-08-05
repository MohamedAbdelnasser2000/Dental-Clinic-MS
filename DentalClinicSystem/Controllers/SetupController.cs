using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Data;
using DentalClinicSystem.Models;

namespace DentalClinicSystem.Controllers
{
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SetupController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> CreateUsers()
        {
            try
            {
                // Create roles
                var roles = new[] { "Admin", "Doctor", "Receptionist", "Patient" };
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Create admin user
                var adminEmail = "admin@dentalclinic.com";
                var adminUser = await _userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "مدير النظام",
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Create doctor user
                var doctorEmail = "doctor@dentalclinic.com";
                var doctorUser = await _userManager.FindByEmailAsync(doctorEmail);
                if (doctorUser == null)
                {
                    doctorUser = new User
                    {
                        UserName = doctorEmail,
                        Email = doctorEmail,
                        FullName = "د. أحمد محمد",
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(doctorUser, "Doctor123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(doctorUser, "Doctor");
                    }
                }

                // Create receptionist user
                var receptionistEmail = "receptionist@dentalclinic.com";
                var receptionistUser = await _userManager.FindByEmailAsync(receptionistEmail);
                if (receptionistUser == null)
                {
                    receptionistUser = new User
                    {
                        UserName = receptionistEmail,
                        Email = receptionistEmail,
                        FullName = "موظف الاستقبال",
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(receptionistUser, "Reception123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(receptionistUser, "Receptionist");
                    }
                }

                return Json(new { success = true, message = "Users created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> CreateSampleData()
        {
            try
            {
                // Add sample patients
                if (!_context.Patients.Any())
                {
                    var patients = new[]
                    {
                        new Patient
                        {
                            FullName = "أحمد محمد علي",
                            FirstName = "أحمد",
                            LastName = "محمد علي",
                            Email = "ahmed.mohamed@email.com",
                            PhoneNumber = "01234567890",
                            Address = "شارع النيل، القاهرة",
                            DateOfBirth = new DateTime(1985, 5, 15),
                            Gender = "ذكر",
                            MedicalHistory = "لا يوجد تاريخ مرضي مهم",
                            RegistrationDate = DateTime.Now,
                            IsActive = true
                        },
                        new Patient
                        {
                            FullName = "فاطمة أحمد حسن",
                            FirstName = "فاطمة",
                            LastName = "أحمد حسن",
                            Email = "fatma.ahmed@email.com",
                            PhoneNumber = "01123456789",
                            Address = "شارع الجمهورية، الإسكندرية",
                            DateOfBirth = new DateTime(1990, 8, 22),
                            Gender = "أنثى",
                            MedicalHistory = "حساسية من البنسلين",
                            RegistrationDate = DateTime.Now,
                            IsActive = true
                        },
                        new Patient
                        {
                            FullName = "محمد سالم أحمد",
                            FirstName = "محمد",
                            LastName = "سالم أحمد",
                            Email = "mohamed.salem@email.com",
                            PhoneNumber = "01098765432",
                            Address = "شارع الهرم، الجيزة",
                            DateOfBirth = new DateTime(1988, 12, 10),
                            Gender = "ذكر",
                            MedicalHistory = "مرض السكري",
                            RegistrationDate = DateTime.Now,
                            IsActive = true
                        }
                    };

                    _context.Patients.AddRange(patients);
                    await _context.SaveChangesAsync();
                }

                // Add sample services
                if (!_context.Services.Any())
                {
                    var services = new[]
                    {
                        new Service
                        {
                            ServiceName = "فحص عام",
                            Description = "فحص شامل للأسنان واللثة",
                            Price = 200,
                            Duration = 30,
                            Category = "فحص",
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            DisplayOrder = 1
                        },
                        new Service
                        {
                            ServiceName = "تنظيف الأسنان",
                            Description = "إزالة الجير وتنظيف الأسنان",
                            Price = 300,
                            Duration = 45,
                            Category = "تنظيف",
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            DisplayOrder = 2
                        },
                        new Service
                        {
                            ServiceName = "حشو الأسنان",
                            Description = "حشو الأسنان المسوسة",
                            Price = 500,
                            Duration = 60,
                            Category = "علاج",
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            DisplayOrder = 3
                        },
                        new Service
                        {
                            ServiceName = "تبييض الأسنان",
                            Description = "تبييض الأسنان بالليزر",
                            Price = 1500,
                            Duration = 90,
                            Category = "تجميل",
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            DisplayOrder = 4
                        },
                        new Service
                        {
                            ServiceName = "خلع الأسنان",
                            Description = "خلع الأسنان التالفة",
                            Price = 400,
                            Duration = 30,
                            Category = "جراحة",
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            DisplayOrder = 5
                        }
                    };

                    _context.Services.AddRange(services);
                    await _context.SaveChangesAsync();
                }

                // Add sample insurance companies
                if (!_context.InsuranceCompanies.Any())
                {
                    var insuranceCompanies = new[]
                    {
                        new InsuranceCompany
                        {
                            CompanyName = "شركة التأمين الطبي المصرية",
                            PhoneNumber = "0227654321",
                            Email = "info@egyptianmedical.com",
                            Address = "القاهرة، مصر",
                            CoveragePercentage = 80,
                            MaxCoverageAmount = 10000,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        },
                        new InsuranceCompany
                        {
                            CompanyName = "شركة الرعاية الصحية الشاملة",
                            PhoneNumber = "0223456789",
                            Email = "contact@comprehensive.com",
                            Address = "الإسكندرية، مصر",
                            CoveragePercentage = 70,
                            MaxCoverageAmount = 8000,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        }
                    };

                    _context.InsuranceCompanies.AddRange(insuranceCompanies);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Sample data created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Status()
        {
            try
            {
                var status = new
                {
                    DatabaseConnected = await _context.Database.CanConnectAsync(),
                    UsersCount = await _userManager.Users.CountAsync(),
                    RolesCount = await _roleManager.Roles.CountAsync(),
                    PatientsCount = await _context.Patients.CountAsync(),
                    ServicesCount = await _context.Services.CountAsync(),
                    InsuranceCompaniesCount = await _context.InsuranceCompanies.CountAsync(),
                    AdminExists = await _userManager.FindByEmailAsync("admin@dentalclinic.com") != null
                };

                return Json(new { success = true, status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
