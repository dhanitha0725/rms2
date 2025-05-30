﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Packages");

            builder.HasKey(p => p.PackageID);

            builder.Property(p => p.PackageID)
                .HasColumnName("PackageID")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.PackageName)
                .HasColumnName("PackageName")
                .HasMaxLength(300)
                .HasColumnType("varchar(300)")
                .IsRequired();

            builder.Property(p => p.Duration)
                 .HasColumnName("Duration")
                 .HasColumnType("interval");

            builder.HasOne(p => p.Facility)
                .WithMany(f => f.Packages)
                .HasForeignKey(p => p.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
