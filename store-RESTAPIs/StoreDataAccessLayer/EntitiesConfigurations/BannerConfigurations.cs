using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class BannerConfigurations : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable("Banners");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.TitleAr).IsRequired().HasMaxLength(200);
            builder.Property(b => b.TitleEn).IsRequired().HasMaxLength(200);
            builder.Property(b => b.SubTitleAr).HasMaxLength(300);
            builder.Property(b => b.SubTitleEn).HasMaxLength(300);
            builder.Property(b => b.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(b => b.LinkUrl).HasMaxLength(500);
            builder.Property(b => b.DisplayOrder).HasDefaultValue(0);
            builder.Property(b => b.IsActive).HasDefaultValue(true);
        }
    }
}


