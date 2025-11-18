using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreDataAccessLayer.Entities;

namespace StoreBusinessLayer.Products
{
    public class ProductsDtos
    {
        public class AddProductReq
        {
            public int? ProductId { get; set; }
            [Required]
            public string ProductNameAr { get; set; } = null!;
            [Required]
            public string ProductNameEn { get; set; } = null!;
            [Required]
            public string ShortNameAr { get; set; } = null!;
            [Required]
            public string ShortNameEn { get; set; } = null!;
            [Required]
            public decimal ProductPrice { get; set; }
            [Required]
            public decimal DiscountPercentage { get; set; }
            [Required]
            public string CategoryName { get; set; } = null!;
            [Required]
            public string MoreDetailsAr { get; set; } = null!;
            [Required]
            public string MoreDetailsEn { get; set; } = null!;
            public bool IsFeatured { get; set; }

        }
        public class AddProductDetails
        {
            public int ProductDetailsId { get; set; }
            [Required]
            public int ProductId { get; set; }
            [Required]
            public byte ColorId { get; set; }
            public byte? SizeId { get; set; }
            [Required]
            public int Quantity { get; set; }
            [Required]
            public string ProductImage { get; set; } = null!;
        }
        public class GetProductsReq
        {
            public int? ProductId { get; set; }
            public string ProductNameAr { get; set; } = null!;
            public string ProductNameEn { get; set; } = null!;
            public string ShortNameAr { get; set; } = null!;
            public string ShortNameEn { get; set; } = null!;
            public decimal ProductPrice { get; set; }
            public decimal PriceAfterDiscount { get; set; }
            public decimal DiscountPercentage { get; set; }
            public string ProductImage { get; set; } = null!;
            public string MoreDetailsAr { get; set; } = null!;
            public string MoreDetailsEn { get; set; } = null!;
            public byte CategoryId { get; set; }
            public bool IsFeatured { get; set; }
            public int TotalQuantity { get; set; }

        }
        public class GetProductDetailsReq
        {
            //this uses when client try to open any product on website
            public string? Color { get; set; }
            public int ProductDetailsId { get; set; }
            public string? Size { get; set; }
            public int Quantity { get; set; }
        }
        public class GetProductDetails
        {
            //this uses when client change the color or size during buying Clothes in UI
            public int ProductDetailsId { get; set; }
            public int Quantity { get; set; }
            public string Image { get; set; } = null!;
        }

         public class UpdateProductDto
        {
            public int ProductId { get; set; }
            public string ProductNameAr { get; set; } = null!;
            public string ProductNameEn { get; set; } = null!;
            public string ShortNameAr { get; set; } = null!;
            public string ShortNameEn { get; set; } = null!;
            public decimal ProductPrice { get; set; }
            public string CategoryName { get; set; } = null!;
            public string MoreDetailsAr { get; set; } = null!;
            public string MoreDetailsEn { get; set; } = null!;
            public decimal DiscountPercentage { get; set; }
            public bool IsFeatured { get; set; }
            public List<UpdateProductDetailDto>? ProductDetails { get; set; }
        }

        public class UpdateProductDetailDto
        {
            public int ProductDetailId { get; set; }
            public string? ColorName { get; set; }
            public string? SizeName { get; set; }
            public int Quantity { get; set; }
            public string? ProductImage { get; set; }
        }

        public class CategorySummaryDto
        {
            public byte CategoryId { get; set; }
            public string CategoryNameAr { get; set; } = null!;
            public string CategoryNameEn { get; set; } = null!;
            public string ImagePath { get; set; } = null!;
        }

        public class UpsertCategoryDto
        {
            public byte? CategoryId { get; set; }
            [Required]
            public string CategoryNameAr { get; set; } = null!;
            [Required]
            public string CategoryNameEn { get; set; } = null!;
            [Required]
            public string ImagePath { get; set; } = null!;
        }

    }
}
