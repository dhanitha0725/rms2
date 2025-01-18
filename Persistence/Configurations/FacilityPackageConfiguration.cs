using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class FacilityPackageConfiguration : IEntityTypeConfiguration<FacilityPackage>
    {
        public void Configure(EntityTypeBuilder<FacilityPackage> builder)
        {
            builder.ToTable("FacilityPackages");

            builder.HasKey(fp => new { fp.FacilityID, fp.PackageID });

            builder.HasOne(fp => fp.Facility)
                .WithMany(f => f.FacilityPackages)
                .HasForeignKey(fp => fp.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fp => fp.Package)
                .WithMany(p => p.FacilityPackages)
                .HasForeignKey(fp => fp.PackageID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
