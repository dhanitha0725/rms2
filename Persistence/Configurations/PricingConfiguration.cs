using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class PricingConfiguration : IEntityTypeConfiguration<Pricing>
    {
        public void Configure(EntityTypeBuilder<Pricing> builder)
        {
            builder.ToTable("Pricing");

            builder.HasKey(e => e.PriceID);

            builder.Property(e => e.PriceID)
                .HasColumnType("int")
                .HasColumnName("PriceID")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(e => e.Price)
                .HasColumnType("decimal(12,2)")
                .HasColumnName("Price")
                .IsRequired();

            builder.Property(e => e.Sector)
                .HasColumnType("varchar(50)")
                .HasColumnName("Sector")
                .HasMaxLength(30)
                .IsRequired();

            // one-to-many relationship between Pricing and Package
            builder.HasOne(e => e.Package)
                .WithMany(e => e.Pricings)
                .HasForeignKey(e => e.PackageID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
