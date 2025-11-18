using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class AnnouncementBarConfigurations : IEntityTypeConfiguration<AnnouncementBar>
    {
        public void Configure(EntityTypeBuilder<AnnouncementBar> builder)
        {
            builder.ToTable("AnnouncementBars");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.TextAr).IsRequired().HasMaxLength(500);
            builder.Property(a => a.TextEn).IsRequired().HasMaxLength(500);
            builder.Property(a => a.LinkUrl).HasMaxLength(500);
            builder.Property(a => a.IsActive).HasDefaultValue(true);
        }
    }
}

