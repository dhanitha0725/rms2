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
