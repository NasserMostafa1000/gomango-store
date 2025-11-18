using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public partial class CartConfigurations : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(Cart => Cart.CartId);
            builder.Property(Cart => Cart.CartId).ValueGeneratedOnAdd();
            builder.Property(Cart => Cart.SessionId).HasMaxLength(100);

            // علاقة اختيارية مع Client (للسماح بالسلة المؤقتة)
            builder.HasOne(Cart => Cart.Client)
                .WithOne(Client => Client.Cart)
                .HasForeignKey<Cart>(cart => cart.ClientId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.ToTable("Carts");
        }
    }
}