using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public partial class ProductsConfigurations
    {
        public class CategoryConfigurations : IEntityTypeConfiguration<Category>
        {
            public void Configure(EntityTypeBuilder<Category> builder)
            {
                builder.HasKey(Category => Category.CategoryId);
                builder.Property(Category => Category.CategoryId).ValueGeneratedOnAdd();

                builder.Property(Category => Category.CategoryName)
                       .HasColumnType("NVARCHAR")
                       .HasMaxLength(150)
                       .IsRequired();

                builder.Property(Category => Category.CategoryNameEn)
                       .HasColumnType("NVARCHAR")
                       .HasMaxLength(150);

                builder.Property(Category => Category.ImagePath)
                       .HasColumnType("NVARCHAR")
                       .HasMaxLength(500);

                // تعريف الأصناف
                builder.HasData(
               new Category { CategoryId = 1, CategoryName = "رجالي", CategoryNameEn = "Men", ImagePath = "/ProjectImages/ForMens.webp" },
               new Category { CategoryId = 2, CategoryName = "نسائي", CategoryNameEn = "Women", ImagePath = "/ProjectImages/ForWomens.png" },
               new Category { CategoryId = 3, CategoryName = "شنط نسائية", CategoryNameEn = "Women Bags", ImagePath = "/ProjectImages/WomenBags.jpg" },
               new Category { CategoryId = 4, CategoryName = "إاكسسوارات", CategoryNameEn = "Accessories", ImagePath = "/ProjectImages/Accessories.webp" }


           );


                builder.ToTable("Categories");
            }
        }
    }
}
