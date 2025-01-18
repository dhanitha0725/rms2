using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class InvoicePaymentConfiguration : IEntityTypeConfiguration<InvoicePayment>
    {
        public void Configure(EntityTypeBuilder<InvoicePayment> builder)
        {
            builder.ToTable("InvoicePayments");

            builder.HasKey(ip => new { ip.PaymentID, ip.InvoiceID });

            builder.HasOne(ip => ip.Payment)
                .WithMany(p => p.InvoicePayments)
                .HasForeignKey(ip => ip.PaymentID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ip => ip.Invoice)
                .WithMany(i => i.InvoicePayments)
                .HasForeignKey(ip => ip.InvoiceID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
