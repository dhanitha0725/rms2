using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ReservedRoomConfiguration : IEntityTypeConfiguration<ReservedRoom>
    {
        public void Configure(EntityTypeBuilder<ReservedRoom> builder)
        {
            builder.ToTable("ReservedRooms");

            builder.HasKey(rr => rr.ReservedRoomID);

            builder.Property(rr => rr.ReservedRoomID)
                .HasColumnType("int")
                .HasColumnName("ReservedRoomID")
                .ValueGeneratedOnAdd();

            builder.Property(rr => rr.StartDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("StartDate")
                .IsRequired();

            builder.Property(rr => rr.EndDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("EndDate")
                .IsRequired();

            // one-to-many relationship between ReservedRoom and Reservation
            builder.HasOne(rr => rr.Reservation)
                .WithMany(r => r.ReservedRooms)
                .HasForeignKey(rr => rr.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship between ReservedRoom and Room
            builder.HasOne(rr => rr.Room)
                .WithMany(r => r.ReservedRooms)
                .HasForeignKey(rr => rr.RoomID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
