using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.ToTable("RoomTypes");

            builder.HasKey(rt => rt.RoomTypeID);

            builder.Property(rt => rt.RoomTypeID)
                .HasColumnType("int")
                .HasColumnName("RoomTypeID")
                .ValueGeneratedOnAdd();

            // one-to-many relationship between RoomType and Room
            builder.HasMany(rt => rt.Rooms)
                .WithOne(r => r.RoomType)
                .HasForeignKey(r => r.RoomTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship between RoomType and RoomPricing
            builder.HasMany(rt => rt.RoomPricings)
                .WithOne(rp => rp.RoomType)
                .HasForeignKey(rp => rp.RoomTypeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
