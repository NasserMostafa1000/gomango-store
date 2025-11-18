using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class PendingOrderConfigurations : IEntityTypeConfiguration<PendingOrder>
    {
        public void Configure(EntityTypeBuilder<PendingOrder> builder)
        {
            builder.ToTable("PendingOrders");

            builder.HasKey(p => p.PendingOrderId);

            builder.Property(p => p.PendingOrderId)
                .HasColumnName("PendingOrderId")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.ClientId)
                .HasColumnName("ClientId")
                .IsRequired();

            builder.Property(p => p.Address)
                .HasColumnName("Address")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(p => p.TotalPrice)
                .HasColumnName("TotalPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.ShippingCoast)
                .HasColumnName("ShippingCoast")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.PaymentMethodId)
                .HasColumnName("PaymentMethodId")
                .IsRequired();

            builder.Property(p => p.ProductsJson)
                .HasColumnName("ProductsJson")
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(p => p.StripeSessionId)
                .HasColumnName("StripeSessionId")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(10)
                .HasDefaultValue("aed");

            builder.Property(p => p.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.IsCompleted)
                .HasColumnName("IsCompleted")
                .HasDefaultValue(false);

            builder.Property(p => p.OrderId)
                .HasColumnName("OrderId")
                .IsRequired(false);

            builder.Property(p => p.FromCart)
                .HasColumnName("FromCart")
                .HasDefaultValue(false);

            builder.Property(p => p.PaymentIntentId)
                .HasColumnName("PaymentIntentId")
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(p => p.DiscountCode)
                .HasColumnName("DiscountCode")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.HasIndex(p => p.StripeSessionId)
                .IsUnique();

            builder.HasIndex(p => p.ClientId);
        }
    }
}

