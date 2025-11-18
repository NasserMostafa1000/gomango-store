using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.Products.ProductInterfaces;
using static StoreBusinessLayer.Products.ProductsDtos;

namespace StoreBusinessLayer.Products
{
   public class CategoriesRepo:IProductCategory
    {
        AppDbContext _Context;
        public CategoriesRepo(AppDbContext context)
        {
            _Context = context;
        }
        public async Task<byte>GetCategoryIdByNameAsync(string CategoryName)
        {
            var normalized = CategoryName.Trim();
            var CategoryItem =await _Context.Category
                .FirstOrDefaultAsync(Categories => Categories.CategoryName == normalized
                    || Categories.CategoryNameEn == normalized);
            if(CategoryItem==null)
            {
                return 0;
            }
            return CategoryItem.CategoryId;
        }
        public async Task<List<CategorySummaryDto>>GetCategoriesName()
        {
            var categories = await _Context.Category
                .OrderBy(c => c.CategoryId)
                .Select(Category => new CategorySummaryDto
                {
                    CategoryId = Category.CategoryId,
                    CategoryNameAr = Category.CategoryName,
                    CategoryNameEn = Category.CategoryNameEn ?? Category.CategoryName,
                    ImagePath = Category.ImagePath ?? string.Empty
                })
                .ToListAsync();
            return categories;

        }

        public async Task<byte> CreateCategoryAsync(UpsertCategoryDto dto)
        {
            var entity = new Category
            {
                CategoryName = dto.CategoryNameAr.Trim(),
                CategoryNameEn = dto.CategoryNameEn.Trim(),
                ImagePath = dto.ImagePath
            };
            await _Context.Category.AddAsync(entity);
            await _Context.SaveChangesAsync();
            return entity.CategoryId;
        }

        public async Task<bool> UpdateCategoryAsync(UpsertCategoryDto dto)
        {
            if (!dto.CategoryId.HasValue)
            {
                throw new ArgumentException("CategoryId is required for update");
            }

            var category = await _Context.Category.FirstOrDefaultAsync(c => c.CategoryId == dto.CategoryId);
            if (category == null)
            {
                return false;
            }

            category.CategoryName = dto.CategoryNameAr.Trim();
            category.CategoryNameEn = dto.CategoryNameEn.Trim();
            category.ImagePath = dto.ImagePath;
            _Context.Category.Update(category);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(byte categoryId)
        {
            var category = await _Context.Category.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return false;
            }

            if (category.Products != null && category.Products.Any())
            {
                throw new InvalidOperationException("لا يمكن حذف تصنيف مرتبط بمنتجات نشطة.");
            }

            _Context.Category.Remove(category);
            return await _Context.SaveChangesAsync() > 0;

        }
    }
}
