using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class CurrencyRateConfigurations : IEntityTypeConfiguration<CurrencyRate>
    {
        public void Configure(EntityTypeBuilder<CurrencyRate> builder)
        {
            builder.ToTable("CurrencyRates");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.HasIndex(c => c.CurrencyCode).IsUnique();
            builder.Property(c => c.RateToAED).HasColumnType("decimal(18,6)");
            builder.Property(c => c.IsActive).HasDefaultValue(true);
        }
    }
}


