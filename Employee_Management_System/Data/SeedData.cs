using Employee_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Employee_Management_System.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            const string adminRoleName = "Admin";
            const string adminEmail = "admin@company.com";
            const string adminPassword = "Admin123";
            const string adminFullName = "Administrator";
            const string adminEmployeeNumber = "EMP0001";

            IdentityRole? adminRole = await roleManager.FindByNameAsync(adminRoleName);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(adminRoleName);
                await roleManager.CreateAsync(adminRole);
            }

            var existingClaims = await roleManager.GetClaimsAsync(adminRole);
            foreach (var permission in Permissions.All)
            {
                if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim("Permission", permission));
                }
            }

            var department = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Administration");
            if (department == null)
            {
                department = new Department
                {
                    Name = "Administration"
                };
                context.Departments.Add(department);
                await context.SaveChangesAsync();
            }

            var vacationTypes = new[]
            {
                new VacationType { Name = "Annual Leave", DefaultAnnualDays = 14 },
                new VacationType { Name = "Sick Leave", DefaultAnnualDays = 10 },
                new VacationType { Name = "Maternity Leave", DefaultAnnualDays = 90 },
                new VacationType { Name = "Paternity Leave", DefaultAnnualDays = 14 },
                new VacationType { Name = "Unpaid Leave", DefaultAnnualDays = 0 }
            };

            foreach (var vacationType in vacationTypes)
            {
                var existingVacationType = await context.VacationTypes.FirstOrDefaultAsync(vt => vt.Name == vacationType.Name);
                if (existingVacationType == null)
                {
                    context.VacationTypes.Add(vacationType);
                }
            }

            await context.SaveChangesAsync();

            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Email == adminEmail);
            if (employee == null)
            {
                employee = new Employee
                {
                    EmployeeNumber = adminEmployeeNumber,
                    FullName = adminFullName,
                    Email = adminEmail,
                    DepartmentId = department.Id,
                    Salary = 0m,
                    EmploymentStatus = "Active"
                };
                context.Employees.Add(employee);
                await context.SaveChangesAsync();
            }

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    EmployeeId = employee.Id,
                    Roles = adminRoleName
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create admin user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, adminRoleName))
            {
                await userManager.AddToRoleAsync(user, adminRoleName);
            }
        }
    }
}
