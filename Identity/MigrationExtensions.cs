using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Identity
{
    public class MigrationExtensions
    {
        public static void ApplyMigrations(IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using IdentityDbContext context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            context.Database.Migrate();
        }

    }
}
