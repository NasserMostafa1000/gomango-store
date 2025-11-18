using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Products;

namespace StoreServices.Products.ProductInterfaces
{
    public interface IProductCategory
    {
        Task<byte> GetCategoryIdByNameAsync(string CategoryName);
        Task<List<ProductsDtos.CategorySummaryDto>> GetCategoriesName();
        Task<byte> CreateCategoryAsync(ProductsDtos.UpsertCategoryDto dto);
        Task<bool> UpdateCategoryAsync(ProductsDtos.UpsertCategoryDto dto);
        Task<bool> DeleteCategoryAsync(byte categoryId);
    }
}
