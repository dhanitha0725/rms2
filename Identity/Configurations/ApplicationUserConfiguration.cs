using Domain.Entities;
using Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // set the UserId as the primary key for both User and AspNetUsers tables
            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<User>(u => u.UserId);
        }
    }
}
