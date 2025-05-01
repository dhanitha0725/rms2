using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ReservationUserDetailConfiguration : IEntityTypeConfiguration<ReservationUserDetail>
    {
        public void Configure(EntityTypeBuilder<ReservationUserDetail> builder)
        {
            builder.ToTable("ReservationUserDetails");

            builder.HasKey(r => r.ReservationUserDetailID);

            builder.Property(e => e.ReservationUserDetailID)
                .HasColumnName("ReservationUserDetailID")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasColumnName("LastName")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.Email)
                .HasColumnName("Email")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasColumnName("PhoneNumber")
                .HasColumnType("varchar(15)");

            builder.Property(e => e.OrganizationName)
                .HasColumnName("OrganizationName")
                .HasColumnType("varchar(100)");

            // One-to-one relationship with Reservation
            builder.HasOne(rud => rud.Reservation)
                .WithOne(r => r.ReservationUserDetail)
                .HasForeignKey<ReservationUserDetail>(rud => rud.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship with Payment
            builder.HasMany(rud => rud.Payments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.ReservationUserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
