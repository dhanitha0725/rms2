using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(r => r.ReservationID);

            builder.Property(e => e.ReservationID)
                .HasColumnName("ReservationID")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.StartDate)
                .HasColumnName("StartDate")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(e => e.EndDate)
                .HasColumnName("EndDate")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(e => e.CreatedDate)
                .HasColumnName("CreatedDate")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("UpdatedDate")
                .HasColumnType("timestamp with time zone");

            builder.Property(r => r.Total)
                .HasColumnName("Total")
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            builder.Property(r => r.Status)
                .HasColumnType("varchar(50)")
                .HasColumnName("Status")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.UserType)
                .HasColumnType("varchar(50)")
                .HasColumnName("userType")
                .HasMaxLength(50);

            // one-to-many relationship with Payment
            builder.HasMany(r => r.Payments)
                .WithOne(p => p.Reservation)
                .HasForeignKey(p => p.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship with ReservedPackage
            builder.HasMany(r => r.ReservedPackages)
                .WithOne(rp => rp.Reservation)
                .HasForeignKey(rp => rp.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship with ReservedRoom
            builder.HasMany(r => r.ReservedRooms)
                .WithOne(rr => rr.Reservation)
                .HasForeignKey(rr => rr.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-one relationship with ReservationUserDetail
            builder.HasOne(r => r.ReservationUserDetail)
                .WithOne(rud => rud.Reservation)
                .HasForeignKey<ReservationUserDetail>(rud => rud.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
