using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.InvoiceID);

            builder.Property(i => i.InvoiceID)
                .HasColumnName("InvoiceID")
                .HasColumnType("int")
                .HasMaxLength(15)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(i => i.AmountPaid)
                .HasColumnName("AmountPaid")
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            builder.Property(i => i.AmountDue)
                .HasColumnName("AmountDue")
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            builder.Property(i => i.IssuedDate)
                .HasColumnName("IssuedDate")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // one to many relationship between Invoice and Reservation
            builder.HasOne(i => i.Reservation)
                .WithMany(r => r.Invoices)
                .HasForeignKey(i => i.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
