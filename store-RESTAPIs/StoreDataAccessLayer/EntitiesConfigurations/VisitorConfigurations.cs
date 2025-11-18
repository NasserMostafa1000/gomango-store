using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class VisitorConfigurations : IEntityTypeConfiguration<Visitor>
    {
        public void Configure(EntityTypeBuilder<Visitor> builder)
        {
            builder.ToTable("Visitors");
            builder.HasKey(v => v.VisitorId);
            builder.Property(v => v.VisitorId).ValueGeneratedOnAdd();
            builder.Property(v => v.IpAddress).HasMaxLength(45);
            builder.Property(v => v.Country).HasMaxLength(100);
            builder.Property(v => v.CountryCode).HasMaxLength(5);
            builder.Property(v => v.City).HasMaxLength(100);
            builder.Property(v => v.Region).HasMaxLength(100);
            builder.Property(v => v.UserAgent).HasMaxLength(500);
            builder.Property(v => v.Referrer).HasMaxLength(500);
            builder.Property(v => v.VisitTime).IsRequired();
            builder.Property(v => v.IsActive).HasDefaultValue(true);

            // علاقة اختيارية مع Client
            builder.HasOne(v => v.Client)
                .WithMany()
                .HasForeignKey(v => v.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

