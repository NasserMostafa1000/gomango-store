using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class DriverConfigurations : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.DriverId);
            builder.Property(d => d.DriverId).ValueGeneratedOnAdd();

            builder.Property(d => d.DriverName)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.UserId)
                .IsRequired();

            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.ToTable("Drivers");
        }
    }
}

