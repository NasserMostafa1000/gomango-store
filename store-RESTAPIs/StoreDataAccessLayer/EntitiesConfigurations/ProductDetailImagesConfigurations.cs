using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class ProductDetailImagesConfigurations : IEntityTypeConfiguration<ProductDetailImages>
    {
        public void Configure(EntityTypeBuilder<ProductDetailImages> builder)
        {
            builder.HasKey(img => img.ProductDetailImageId);
            builder.Property(img => img.ProductDetailImageId).ValueGeneratedOnAdd();

            builder.Property(img => img.ImageUrl)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(img => img.DisplayOrder)
                .HasColumnType("INT")
                .HasDefaultValue(0);

            builder.HasOne(img => img.ProductDetails)
                .WithMany(pd => pd.ProductDetailImages)
                .HasForeignKey(img => img.ProductDetailsId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasIndex(img => img.ProductDetailsId)
                .HasDatabaseName("IX_ProductDetailImages_ProductDetailsId");

            builder.ToTable("ProductDetailImages");
        }
    }
}

