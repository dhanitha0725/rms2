using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class FacilityTypeConfiguration : IEntityTypeConfiguration<FacilityType>
    {
        public void Configure(EntityTypeBuilder<FacilityType> builder)
        {
            builder.ToTable("FacilityType");

            builder.HasKey(e => e.FacilityTypeId);

            builder.Property(e => e.FacilityTypeId)
                .HasColumnName("FacilityTypeID")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");

            builder.HasMany(e => e.Facilities)
                .WithOne(e => e.FacilityType);

            builder.HasData(
                new FacilityType { FacilityTypeId = 1, TypeName = "Auditorium" },
                new FacilityType { FacilityTypeId = 2, TypeName = "Bungalow" },
                new FacilityType { FacilityTypeId = 3, TypeName = "Hall" },
                new FacilityType { FacilityTypeId = 4, TypeName = "Hostel" }
            );
        }
    }
}
