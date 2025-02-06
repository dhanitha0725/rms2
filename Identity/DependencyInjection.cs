using Microsoft.Extensions.Configuration;
using Application.Abstractions.Interfaces;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Identity.Contexts;
using Identity.Seeds;

namespace Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //configure identity db context
            services.AddDbContext<ReservationIdentityDbContext>(options =>
                options.UseNpgsql(configuration.
                    GetConnectionString("DefaultConnection")));

            //configure identity with ApplicationUser
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    //configure identity options
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<ReservationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication();

            // register authorization
            // define policies and roles
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                    policy => policy.RequireRole("Admin"));
            });

            services.AddSingleton<IEmailSender, NoOpEmailSender>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }


        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var services = scope.ServiceProvider;

            //seed roles
            await SeedRoles.SeedAsync(services);
        }
    }
}
