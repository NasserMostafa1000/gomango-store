using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;
using System;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class LegalContentConfigurations : IEntityTypeConfiguration<LegalContent>
    {
        public void Configure(EntityTypeBuilder<LegalContent> builder)
        {
            builder.ToTable("LegalContent");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TermsAr)
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired();

            builder.Property(x => x.TermsEn)
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired();

            builder.Property(x => x.PrivacyAr)
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired();

            builder.Property(x => x.PrivacyEn)
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired();

            builder.Property(x => x.LastUpdated)
                   .HasColumnType("DATETIME2")
                   .IsRequired();

            builder.HasData(new LegalContent
            {
                Id = 1,
                TermsAr = "سيتم تحديث شروط الاستخدام قريباً.",
                TermsEn = "Terms of use will be updated soon.",
                PrivacyAr = "سيتم تحديث سياسة الخصوصية قريباً.",
                PrivacyEn = "Privacy policy will be updated soon.",
                LastUpdated = DateTime.UtcNow
            });
        }
    }
}

