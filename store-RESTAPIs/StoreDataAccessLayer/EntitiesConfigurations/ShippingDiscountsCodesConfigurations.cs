using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataLayer.Entities;

namespace StoreDataLayer.EntitiesConfigurations
{
    public class ShippingDiscountsCodesConfigurations : IEntityTypeConfiguration<ShippingDiscountCodes>
    {
        public void Configure(EntityTypeBuilder<ShippingDiscountCodes> builder)
        {
            builder.HasKey(Sd => Sd.promoCodeNumber);
            builder.Property(Sd => Sd.promoCodeNumber).HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(Sd => Sd.IsActive).HasColumnType("BIT").IsRequired();
            var discounts = new List<ShippingDiscountCodes>();
            for (int i = 1; i <= 1000; i++)
            {
                
                    discounts.Add(new ShippingDiscountCodes
                    {
                        promoCodeNumber = $"PROMO{i:D4}", // PROMO0001, PROMO0002, ..., PROMO1000
                        IsActive = true
                    });
                

            }

            builder.HasData(discounts);
            builder.ToTable("ShippingDiscountCodes");

        }
    }
}
