using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class ProductReviewConfigurations : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("ProductReviews");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(2000);
            builder.HasOne(r => r.Product)
                   .WithMany()
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}


