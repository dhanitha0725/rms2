using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");

            builder.HasKey(e => e.ImageID);

            builder.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(e => e.Facility)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
