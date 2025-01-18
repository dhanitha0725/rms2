using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .HasColumnType("int")
                .HasColumnName("UserId")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.FirstName)
                .HasColumnType("varchar(50)")
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .HasColumnType("varchar(50)")
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .HasColumnType("varchar(50)")
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.PhoneNumber)
                .HasColumnType("varchar(15)")
                .HasColumnName("PhoneNumber")
                .HasMaxLength(15);

            builder.Property(u => u.Role)
                .HasColumnType("varchar(30)")
                .HasColumnName("Role")
                .IsRequired();




        }
    }
}
