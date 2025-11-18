using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class ShippingCoastsConfigurations : IEntityTypeConfiguration<ShippingCoasts>
    {
        public void Configure(EntityTypeBuilder<ShippingCoasts> builder)
        {
            builder.HasKey(sp => sp.Id);
            builder.Property(sp => sp.Id)
                   .HasColumnType("TINYINT");

            builder.Property(sp => sp.Price)
                   .HasColumnType("DECIMAL")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(sp => sp.GovernorateName)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(55);

            builder.HasData(LoadData());
        }

        private ShippingCoasts[] LoadData()
        {
            // الإمارات السبع (بالدرهم الإماراتي AED)
            return new ShippingCoasts[]
            {
                new ShippingCoasts { Id = 1, GovernorateName = "أبوظبي",        Price = 20.00m },
                new ShippingCoasts { Id = 2, GovernorateName = "دبي",           Price = 18.00m },
                new ShippingCoasts { Id = 3, GovernorateName = "الشارقة",       Price = 18.00m },
                new ShippingCoasts { Id = 4, GovernorateName = "عجمان",         Price = 17.00m },
                new ShippingCoasts { Id = 5, GovernorateName = "أم القيوين",    Price = 22.00m },
                new ShippingCoasts { Id = 6, GovernorateName = "رأس الخيمة",    Price = 25.00m },
                new ShippingCoasts { Id = 7, GovernorateName = "الفجيرة",       Price = 24.00m },
            };
        }
    }
}
