using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Identity.Seeds
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //var logger = serviceProvider.GetRequiredService<ILogger<SeedRoles>>();
            var logger = Log.ForContext(typeof(SeedRoles));

            // add roles
            string[] roleNames =
            {
                    "Admin",
                    "Employee",
                    "Accountant",
                    "Hostel",
                    "Customer",
                    "Guest"
                };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    logger.Information($"Role {roleName} created");
                }
                else
                {
                    logger.Information($"Role {roleName} already exists");
                }
            }

            //seed admin role
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(user, "Admin@123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    logger.Information($"User {adminEmail} created and added to Admin role");
                }
                else
                {
                    logger.Error($"Error creating user {adminEmail}");
                }
            }
            else
            {
                logger.Information($"User {adminEmail} already exists");
            }
        }
    }
}
