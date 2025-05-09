using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.PaymentID);

            builder.Property(p => p.PaymentID)
                .HasColumnName("PaymentID")
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(p => p.Method)
                .HasColumnName("Method")
                .HasColumnType("varchar(30)")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(p => p.AmountPaid)
                .HasColumnName("AmountPaid")
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            builder.Property(p => p.CreatedDate)
                .HasColumnName("CreatedDate")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(p => p.Status)
                .HasColumnType("varchar(30)")
                .HasColumnName("Status")
                .HasMaxLength(30)
                .IsRequired();

            // One-to-many relationship between Payment and Reservation
            builder.HasOne(p => p.Reservation)
                .WithMany(r => r.Payments)
                .HasForeignKey(p => p.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship with ReservationUserDetail
            builder.HasOne(p => p.User)
                .WithMany(rud => rud.Payments)
                .HasForeignKey(p => p.ReservationUserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
