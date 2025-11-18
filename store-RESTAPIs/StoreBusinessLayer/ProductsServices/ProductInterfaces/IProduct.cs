using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Products;
using static StoreBusinessLayer.Products.ProductsDtos;

namespace StoreServices.Products.ProductInterfaces
{
    public interface IProduct
    {
         Task<int> AddProduct(ProductsDtos.AddProductReq Req);
        Task<int> AddProductDetails(ProductsDtos.AddProductDetails Req);
        Task<List<ProductsDtos.GetProductsReq>> GetDiscountsProducts(short PageNumber, byte Limit);
        Task<List<ProductsDtos.GetProductsReq>> GetProductsWithLimit(short PageNumber, byte Limit);
        Task<List<ProductsDtos.GetProductsReq>> GetFeaturedProducts(short PageNumber, byte Limit);
        Task<List<ProductsDtos.GetProductsReq>> GetProductsWhereInClothesCategory(short PageNumber, byte Limit);
        Task<ProductsDtos.GetProductsReq?> GetProductByName(string name);
        Task<ProductsDtos.GetProductsReq> GetProductDetailsByProductId(int ProductId);
        Task<List<ProductsDtos.GetProductsReq>> GetListProductsWithinName(string productName);
        Task<ProductsDtos.GetProductDetailsReq> GetDetailsByProductId(int ProductId);
        Task<List<string>> GetProductColorsByProductId(int productId);
        Task<List<string>> GetProductSizesByProductId(int productId);
        Task<ProductsDtos.GetProductDetails> GetDetailsBy(int ProductId, string ColorName, string SizeName = "");
        Task<List<string>> GetAllColorsBelongsToSizeName(int ProductId, string SizeName);
        Task<List<string>> GetAllSizesBelongsToColorName(int ProductId, string ColorName);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                           Admin section
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Task<List<ProductsDtos.CategorySummaryDto>> GetCategoriesName();
        Task<string?> GetProductImagePathAsync(int ProductDetailsId);
        Task<bool> DeleteLastProductImageAsync(int ProductDetailsId);
        Task<bool> UpdateProductImageAsync(int ProductDetailsId, string fileUrl);
        Task<UpdateProductDto> GetProductWithDetailsAsync(int productId);
        Task<bool> UpdateProductAsync(UpdateProductDto updateProductDto);
        Task<List<int>> GetProductsIds();
        Task<bool> DeleteProductAsync(int productId);

    }
}
