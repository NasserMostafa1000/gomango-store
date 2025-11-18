using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public partial class AdminInfoConfigurations : IEntityTypeConfiguration<AdminInfo>
    {
        public void Configure(EntityTypeBuilder<AdminInfo> builder)
        {
            builder.HasKey(WebsiteInfo => WebsiteInfo.Id);
            builder.Property(WebsiteInfo => WebsiteInfo.Id).ValueGeneratedOnAdd();



            builder.Property(WebsiteInfo => WebsiteInfo.Id).HasColumnType("TINYINT").IsRequired();



            builder.Property(WebsiteInfo => WebsiteInfo.TransactionNumber)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(20)
                   .IsRequired();


            builder.Property(WebsiteInfo => WebsiteInfo.WhatsAppNumber)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(WebsiteInfo => WebsiteInfo.PhoneNumber)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(WebsiteInfo => WebsiteInfo.Email)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(WebsiteInfo => WebsiteInfo.AboutUsAr)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(4000);

            builder.Property(WebsiteInfo => WebsiteInfo.AboutUsEn)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(4000);

            builder.Property(WebsiteInfo => WebsiteInfo.FacebookUrl)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(500);

            builder.Property(WebsiteInfo => WebsiteInfo.InstagramUrl)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(500);

            builder.Property(WebsiteInfo => WebsiteInfo.TikTokUrl)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(500);
            builder.HasData(
                new AdminInfo
                {
                    Id = 1,
                    TransactionNumber = "1234567890",
                    WhatsAppNumber = "+201234567890",
                    PhoneNumber = "+201098765432",
                    Email = "info@website.com",
                    AboutUsAr = "نص تعريفي افتراضي عن المتجر يمكن تعديله من لوحة التحكم.",
                    AboutUsEn = "Default about-us content that can be managed from the admin panel.",
                    FacebookUrl = "https://www.facebook.com/your-page",
                    InstagramUrl = "https://www.instagram.com/your-page",
                    TikTokUrl = "https://www.tiktok.com/@your-page"
                }
            );

            builder.ToTable("AdminInfo");
        }
    }
}
