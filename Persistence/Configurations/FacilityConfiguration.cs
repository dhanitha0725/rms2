using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> builder)
        {
            builder.ToTable("Facilities");

            builder.HasKey(e => e.FacilityID);

            builder.Property(e => e.FacilityID)
                .ValueGeneratedOnAdd()
                .HasColumnName("FacilityID")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(e => e.FacilityName)
                .HasColumnName("FacilityName")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.FacilityType)
                .HasColumnName("FacilityType")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.Attributes)
                .HasColumnName("Attributes")
                .HasColumnType("jsonb");

            builder.Property(e => e.Description)
                .HasColumnName("Description")
                .HasColumnType("varchar(500)");
        }
    }
}
