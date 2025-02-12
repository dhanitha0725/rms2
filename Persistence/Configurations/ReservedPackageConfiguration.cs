using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ReservedPackageConfiguration : IEntityTypeConfiguration<ReservedPackage>
    {
        public void Configure(EntityTypeBuilder<ReservedPackage> builder)
        {
            builder.ToTable("ReservedPackages");

            builder.HasKey(rp => rp.ReservedPackageID);

            builder.Property(rp => rp.ReservedPackageID)
                .HasColumnType("int")
                .HasColumnName("ReservedPackageID")
                .ValueGeneratedOnAdd();

            // one-to-many relationship between ReservedPackage and Package
            builder.HasOne(rp => rp.Package)
                .WithMany(p => p.ReservedPackages)
                .HasForeignKey(rp => rp.PackageID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship between ReservedPackage and Reservation
            builder.HasOne(rp => rp.Reservation)
                .WithMany(r => r.ReservedPackages)
                .HasForeignKey(rp => rp.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
