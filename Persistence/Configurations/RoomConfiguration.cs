﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(e => e.RoomID);

            builder.Property(e => e.RoomID)
                .HasColumnType("int")
                .HasColumnName("RoomID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Capacity)
                .HasColumnType("int")
                .HasColumnName("Capacity")
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnType("varchar(50)")
                .HasColumnName("Status")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.NumberOfBeds)
                .HasColumnType("int")
                .HasColumnName("NumberOfBeds");        

            // one-to-many relationship between Room and Facility
            builder.HasOne(e => e.Facility)
                .WithMany(e => e.Rooms)
                .HasForeignKey(e => e.FacilityID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship between Room and ReservedRoom
            builder.HasMany(e => e.ReservedRooms)
                .WithOne(e => e.Room)
                .HasForeignKey(e => e.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many relationship between Room and RoomType
            builder.HasOne(e => e.RoomType)
                .WithMany(e => e.Rooms)
                .HasForeignKey(e => e.RoomTypeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
