using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Seeds
{
    public class SeedRoles
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<IdentityRole>>();
            var userManager = serviceProvider
                .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();

            // add roles
            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //seed admin role
            var adminEmail = "admin123@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
