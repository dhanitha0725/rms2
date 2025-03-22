using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class RoomPricingConfiguration : IEntityTypeConfiguration<RoomPricing>
    {
        public void Configure(EntityTypeBuilder<RoomPricing> builder)
        {
            builder.ToTable("RoomPricing");

            builder.HasKey(rp => rp.RoomPricingID);

            builder.Property(rp => rp.RoomPricingID)
                .HasColumnType("int")
                .HasColumnName("RoomPricingID")
                .ValueGeneratedOnAdd();

            builder.Property(rp => rp.Price)
                .HasColumnType("decimal(10,2)")
                .HasColumnName("Price")
                .IsRequired();

            builder.Property(rp => rp.Sector)
                .HasColumnType("varchar(30)")
                .HasColumnName("Sector")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(rp => rp.RoomType)
                .HasColumnType("varchar(30)")
                .HasColumnName("RoomType")
                .IsRequired()
                .HasMaxLength(30);

            // one-to-many relationship between Facility and RoomPricing
            builder.HasOne(rp => rp.Facility)
                .WithMany(f => f.RoomPricings)
                .HasForeignKey(rp => rp.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
