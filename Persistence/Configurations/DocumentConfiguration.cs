using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.DocumentID);

            builder.Property(d => d.DocumentID)
                .HasColumnType("int")
                .HasColumnName("DocumentID")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.DocumentType)
                .HasColumnType("varchar(30)")
                .HasColumnName("DocumentType")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(d => d.Url)
                .IsRequired();

            // One-to-many relationship between Document and Reservation
            builder.HasOne(d => d.Reservation)
                .WithMany(r => r.Documents)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_Documents_Reservations")
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between Document and Payment
            builder.HasOne(d => d.Payment)
                .WithMany(p => p.Documents)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Documents_Payments")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
