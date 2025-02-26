using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Identity.Contexts
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<ReservationIdentityDbContext>
    {
        public ReservationIdentityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ReservationIdentityDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new ReservationIdentityDbContext(optionsBuilder.Options);
        }
    }
}
