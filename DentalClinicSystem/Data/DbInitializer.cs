using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DentalClinicSystem.Models;

namespace DentalClinicSystem.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Apply migrations to create/update database
        await context.Database.MigrateAsync();

        // Create roles
        await CreateRolesAsync(roleManager);

        // Create default admin user
        await CreateDefaultUsersAsync(userManager);

        // Seed initial data
        await SeedInitialDataAsync(context);

        // Seed test data for dashboard
        await SeedData.SeedTestDataAsync(context);
    }

    private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task CreateDefaultUsersAsync(UserManager<User> userManager)
    {
        // Create default admin user
        var adminEmail = "admin@dentalclinic.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

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

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create default doctor user
        var doctorEmail = "doctor@dentalclinic.com";
        var doctorUser = await userManager.FindByEmailAsync(doctorEmail);

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

            var result = await userManager.CreateAsync(doctorUser, "Doctor123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(doctorUser, "Doctor");
            }
        }

        // Create default receptionist user
        var receptionistEmail = "receptionist@dentalclinic.com";
        var receptionistUser = await userManager.FindByEmailAsync(receptionistEmail);

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

            var result = await userManager.CreateAsync(receptionistUser, "Reception123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(receptionistUser, "Receptionist");
            }
        }
    }

    private static async Task SeedInitialDataAsync(ApplicationDbContext context)
    {
        // Seed Insurance Companies
        if (!context.InsuranceCompanies.Any())
        {
            var insuranceCompanies = new List<InsuranceCompany>
            {
                new InsuranceCompany
                {
                    CompanyName = "شركة التأمين الطبي الأولى",
                    PhoneNumber = "01234567890",
                    Email = "info@insurance1.com",
                    CoveragePercentage = 80,
                    MaxCoverageAmount = 10000,
                    IsActive = true
                },
                new InsuranceCompany
                {
                    CompanyName = "شركة التأمين الصحي المتقدم",
                    PhoneNumber = "01234567891",
                    Email = "info@insurance2.com",
                    CoveragePercentage = 70,
                    MaxCoverageAmount = 15000,
                    IsActive = true
                }
            };

            context.InsuranceCompanies.AddRange(insuranceCompanies);
            await context.SaveChangesAsync();
        }

        // Seed Services
        if (!context.Services.Any())
        {
            var services = new List<Service>
            {
                new Service
                {
                    ServiceName = "فحص عام",
                    Description = "فحص شامل للأسنان واللثة",
                    Price = 100,
                    Category = "فحص",
                    EstimatedDurationMinutes = 30,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "حشو عادي",
                    Description = "حشو الأسنان بالحشو العادي",
                    Price = 200,
                    Category = "علاج",
                    EstimatedDurationMinutes = 45,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "حشو تجميلي",
                    Description = "حشو الأسنان بالحشو التجميلي",
                    Price = 350,
                    Category = "علاج",
                    EstimatedDurationMinutes = 60,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "خلع سن",
                    Description = "خلع الأسنان التالفة",
                    Price = 150,
                    Category = "جراحة",
                    EstimatedDurationMinutes = 30,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "تنظيف الأسنان",
                    Description = "تنظيف الأسنان وإزالة الجير",
                    Price = 120,
                    Category = "تنظيف",
                    EstimatedDurationMinutes = 45,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "تبييض الأسنان",
                    Description = "تبييض الأسنان بالليزر",
                    Price = 800,
                    Category = "تجميل",
                    EstimatedDurationMinutes = 90,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "تركيب تقويم",
                    Description = "تركيب تقويم الأسنان",
                    Price = 5000,
                    Category = "تقويم",
                    EstimatedDurationMinutes = 120,
                    IsActive = true
                },
                new Service
                {
                    ServiceName = "علاج عصب",
                    Description = "علاج عصب الأسنان",
                    Price = 400,
                    Category = "علاج",
                    EstimatedDurationMinutes = 90,
                    IsActive = true
                }
            };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();
        }
    }
}
