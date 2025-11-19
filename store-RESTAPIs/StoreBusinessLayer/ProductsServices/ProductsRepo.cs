using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.Products.ProductInterfaces;
using static StoreBusinessLayer.Products.ProductsDtos;

namespace StoreBusinessLayer.Products
{
    public class ProductsRepo:IProduct
    {
        AppDbContext _Context;
        private readonly  IProductColor _colors;
        private readonly IProductSize _Sizes;
        private readonly IProductCategory _CategoriesBL;

        public ProductsRepo(AppDbContext context, IProductColor Colors, IProductSize Sizes, IProductCategory category)
        {
            _Context = context;
            _colors = Colors;
            _Sizes = Sizes;
            _CategoriesBL = category;
        }

        public async Task<int> AddProduct(ProductsDtos.AddProductReq Req)
        {
            byte CategoryId = await _CategoriesBL.GetCategoryIdByNameAsync(Req.CategoryName);
            try
            {
                var newProduct = new Product
                {
                    CategoryId = CategoryId,
                    ProductNameAr = Req.ProductNameAr,
                    ProductNameEn = Req.ProductNameEn,
                    ShortNameAr = Req.ShortNameAr,
                    ShortNameEn = Req.ShortNameEn,
                    DiscountPercentage = Req.DiscountPercentage,
                    ProductPrice = Req.ProductPrice,
                    MoreDetailsAr = Req.MoreDetailsAr,
                    MoreDetailsEn = Req.MoreDetailsEn,
                    IsFeatured = Req.IsFeatured

                };

                await _Context.Products.AddAsync(newProduct);
                await _Context.SaveChangesAsync();
                return newProduct.ProductId;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> AddProductDetails(ProductsDtos.AddProductDetails Req)
        {
            try
            {
               var Details = new ProductsDetails
                {
                    ProductId = Req.ProductId,
                    ProductImage = Req.ProductImage,
                    Quantity = Req.Quantity,
                    ColorId =Req.ColorId,
                    SizeId =Req.SizeId == 0 ? (byte?)null : Req.SizeId,
                };

                await _Context.AddAsync(Details);
                await _Context.SaveChangesAsync();
                return Details.ProductDetailsId;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ProductsDtos.GetProductsReq>> GetDiscountsProducts(short PageNumber,byte Limit)
        {
            try
            {
                var discountProducts = await _Context.Products
                    .Where(p => p.DiscountPercentage > 0)
                    .Include(p => p.ProductDetails)
                    .OrderBy(x => Guid.NewGuid())
                    .Paginate(PageNumber, Limit)
                    .ToListAsync();

                var result = discountProducts.Select(Product => new ProductsDtos.GetProductsReq
                {
                    ProductId = Product.ProductId,
                    ProductNameAr = Product.ProductNameAr,
                    ProductNameEn = Product.ProductNameEn,
                    ShortNameAr = Product.ShortNameAr,
                    ShortNameEn = Product.ShortNameEn,
                    ProductPrice = Product.ProductPrice,
                    PriceAfterDiscount = Product.ProductPrice - (Product.ProductPrice * (Product.DiscountPercentage / 100)),
                    DiscountPercentage = Product.DiscountPercentage,
                    ProductImage = Product.ProductDetails.FirstOrDefault()?.ProductImage,
                    MoreDetailsAr = Product.MoreDetailsAr,
                    MoreDetailsEn = Product.MoreDetailsEn,
                    CategoryId = Product.CategoryId,
                    IsFeatured = Product.IsFeatured

                }).ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ProductsDtos.GetProductsReq>> GetProductsWithLimit(short PageNumber, byte Limit)
        {
            try
            {
                var discountProducts = await _Context.Products
                    .Include(p => p.ProductDetails)
                    .OrderBy(x => Guid.NewGuid())
                    .Paginate(PageNumber, Limit)
                    .ToListAsync();

                var result = discountProducts.Select(Product => new ProductsDtos.GetProductsReq
                {
                    ProductId = Product.ProductId,
                    ProductNameAr = Product.ProductNameAr,
                    ProductNameEn = Product.ProductNameEn,
                    ShortNameAr = Product.ShortNameAr,
                    ShortNameEn = Product.ShortNameEn,
                    ProductPrice = Product.ProductPrice,
                    PriceAfterDiscount = Product.ProductPrice - (Product.ProductPrice * (Product.DiscountPercentage / 100)),
                    DiscountPercentage = Product.DiscountPercentage,
                    ProductImage = Product.ProductDetails.FirstOrDefault()?.ProductImage,
                    MoreDetailsAr = Product.MoreDetailsAr,
                    MoreDetailsEn = Product.MoreDetailsEn,
                    CategoryId = Product.CategoryId,
                    IsFeatured = Product.IsFeatured

                }).ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ProductsDtos.GetProductsReq>> GetFeaturedProducts(short PageNumber, byte Limit)
        {
            try
            {
                var featuredProducts = await _Context.Products
                    .Where(p => p.IsFeatured)
                    .Include(p => p.ProductDetails)
                    .OrderByDescending(p => p.ProductId)
                    .Paginate(PageNumber, Limit)
                    .ToListAsync();

                return featuredProducts.Select(Product => new ProductsDtos.GetProductsReq
                {
                    ProductId = Product.ProductId,
                    ProductNameAr = Product.ProductNameAr,
                    ProductNameEn = Product.ProductNameEn,
                    ShortNameAr = Product.ShortNameAr,
                    ShortNameEn = Product.ShortNameEn,
                    ProductPrice = Product.ProductPrice,
                    PriceAfterDiscount = Product.ProductPrice - (Product.ProductPrice * (Product.DiscountPercentage / 100)),
                    DiscountPercentage = Product.DiscountPercentage,
                    ProductImage = Product.ProductDetails.FirstOrDefault()?.ProductImage,
                    MoreDetailsAr = Product.MoreDetailsAr,
                    MoreDetailsEn = Product.MoreDetailsEn,
                    CategoryId = Product.CategoryId,
                    IsFeatured = Product.IsFeatured
                }).ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ProductsDtos.GetProductsReq>> GetProductsWhereInClothesCategory(short PageNumber, byte Limit)
        {
            try
            {
                var discountProducts = await _Context.Products
                    .Where(p => p.CategoryId ==4 )//1 is the id of the Accessories category
                    .Include(p => p.ProductDetails)
                    .OrderBy(x => Guid.NewGuid())
                    .Paginate(PageNumber, Limit)
                    .ToListAsync();

                var result = discountProducts.Select(Product => new ProductsDtos.GetProductsReq
                {
                    ProductId = Product.ProductId,
                    ProductNameAr = Product.ProductNameAr,
                    ProductNameEn = Product.ProductNameEn,
                    ShortNameAr = Product.ShortNameAr,
                    ShortNameEn = Product.ShortNameEn,
                    ProductPrice = Product.ProductPrice,
                    PriceAfterDiscount = Product.ProductPrice - (Product.ProductPrice * (Product.DiscountPercentage / 100)),
                    DiscountPercentage = Product.DiscountPercentage,
                    ProductImage = Product.ProductDetails.FirstOrDefault()?.ProductImage,
                    MoreDetailsAr = Product.MoreDetailsAr,
                    MoreDetailsEn = Product.MoreDetailsEn,
                    CategoryId = Product.CategoryId,
                    IsFeatured = Product.IsFeatured

                }).ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<ProductsDtos.GetProductsReq?> GetProductByName(string name)
        {
            try
            {
                var product = await _Context.Products
                    .Include(p => p.ProductDetails)
                    .Where(p => p.ProductNameAr == name || p.ProductNameEn == name)
                    .FirstOrDefaultAsync();

                if (product == null)
                    return null;

                return new ProductsDtos.GetProductsReq
                {
                    ProductId = product.ProductId,
                    ProductNameAr = product.ProductNameAr,
                    ProductNameEn = product.ProductNameEn,
                    ShortNameAr = product.ShortNameAr,
                    ShortNameEn = product.ShortNameEn,
                    ProductPrice = product.ProductPrice,
                    PriceAfterDiscount = product.ProductPrice - (product.ProductPrice * (product.DiscountPercentage / 100)),
                    DiscountPercentage = product.DiscountPercentage,
                    ProductImage = product.ProductDetails.FirstOrDefault()?.ProductImage,
                    MoreDetailsAr = product.MoreDetailsAr,
                    MoreDetailsEn = product.MoreDetailsEn,
                    IsFeatured = product.IsFeatured
                };
            }
            catch (Exception ex)
            {
                // ✅ سجل الخطأ بدلاً من رميه مباشرةً
                Console.WriteLine($"خطأ أثناء جلب المنتج: {ex.Message}");
                throw;
            }
        }

        public async Task<ProductsDtos.GetProductsReq> GetProductDetailsByProductId(int ProductId)
        {
            try
            {
                var ProductInfo = await _Context.Products
                    .Include(p => p.ProductDetails) // تحميل تفاصيل المنتج المرتبطة
                    .FirstOrDefaultAsync(p => p.ProductId == ProductId); // التصفية تتم هنا وليس في Include

                if (ProductInfo != null)
                {
                    return new ProductsDtos.GetProductsReq
                    {
                        ProductId = ProductInfo.ProductId,
                        ProductNameAr = ProductInfo.ProductNameAr,
                        ProductNameEn = ProductInfo.ProductNameEn,
                        ShortNameAr = ProductInfo.ShortNameAr,
                        ShortNameEn = ProductInfo.ShortNameEn,
                        ProductPrice = ProductInfo.ProductPrice,
                        PriceAfterDiscount = ProductInfo.ProductPrice - (ProductInfo.ProductPrice * (ProductInfo.DiscountPercentage / 100)),
                        DiscountPercentage = ProductInfo.DiscountPercentage,
                        ProductImage = ProductInfo.ProductDetails.FirstOrDefault()?.ProductImage, // تجنب NullReferenceException
                        MoreDetailsAr = ProductInfo.MoreDetailsAr,
                        MoreDetailsEn = ProductInfo.MoreDetailsEn,
                        CategoryId = ProductInfo.CategoryId,
                        IsFeatured = ProductInfo.IsFeatured,
                        TotalQuantity = ProductInfo.ProductDetails.Sum(pd => pd.Quantity)
                    };
                }

                return new ProductsDtos.GetProductsReq(); // إرجاع كائن فارغ عند عدم العثور على المنتج
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات المنتج.", ex);
            }
        }

        public async Task<List<ProductsDtos.GetProductsReq>> GetListProductsWithinName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentNullException(nameof(productName));
            }

            try
            {
                var lowerProductName = productName.ToLower();

                // جلب CategoryId في حالة كان الاسم مطابقًا لفئة معينة
                byte? categoryId = await _CategoriesBL.GetCategoryIdByNameAsync(productName);

                var products = await _Context.Products
                    .Where(p =>
                        p.ProductNameAr.ToLower().Contains(lowerProductName) ||
                        p.ProductNameEn.ToLower().Contains(lowerProductName) ||
                        p.ProductNameAr.ToLower() == (lowerProductName) ||
                        p.ProductNameEn.ToLower() == (lowerProductName) ||
                        (categoryId.HasValue && p.CategoryId == categoryId.Value) ||
                        p.MoreDetailsAr.ToLower().Contains(lowerProductName) ||
                        p.MoreDetailsEn.ToLower().Contains(lowerProductName))
                    .Include(p => p.ProductDetails)
                    .ToListAsync();

                // إذا لم يتم العثور على منتجات، يمكن إرجاع قائمة فارغة بدلاً من `null`
                if (!products.Any())
                {
                    return new List<ProductsDtos.GetProductsReq>();
                }

                var result = products.Select(product => new ProductsDtos.GetProductsReq
                {
                    ProductId = product.ProductId,
                    ProductNameAr = product.ProductNameAr,
                    ProductNameEn = product.ProductNameEn,
                    ShortNameAr = product.ShortNameAr,
                    ShortNameEn = product.ShortNameEn,
                    ProductPrice = product.ProductPrice,
                    PriceAfterDiscount = product.ProductPrice - (product.ProductPrice * (product.DiscountPercentage / 100)),
                    DiscountPercentage = product.DiscountPercentage,
                    ProductImage = product.ProductDetails.FirstOrDefault()?.ProductImage,
                    MoreDetailsAr = product.MoreDetailsAr,
                    MoreDetailsEn = product.MoreDetailsEn,
                    CategoryId = product.CategoryId,
                    IsFeatured = product.IsFeatured
                }).ToList();

                return result;
            }
            catch 
            {
                throw;
            }
        }
        public async Task<ProductsDtos.GetProductDetailsReq> GetDetailsByProductId(int ProductId)
        {
            if (ProductId <= 0)
            {
                throw new ArgumentException("Id must be greater than 0", nameof(ProductId));
            }
            try
            {
                var productDetails = await _Context.ProductDetails
                    .FirstOrDefaultAsync(details => details.ProductId == ProductId && details.Quantity != 0);

                if (productDetails == null)
                {
                     productDetails = await _Context.ProductDetails
                                       .FirstOrDefaultAsync(details => details.ProductId == ProductId);
                }

                var result = new ProductsDtos.GetProductDetailsReq
                {
                    ProductDetailsId = productDetails!.ProductDetailsId,
                    Color = await _colors.GetColorNameByIdAsync(productDetails!.ColorId),
                    Size = productDetails.SizeId.HasValue ? await _Sizes.GetSizeNameByIdAsync(productDetails.SizeId.Value) : null,
                    Quantity = productDetails.Quantity,
                };

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<string>> GetProductColorsByProductId(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("ProductId must be greater than 0", nameof(productId));
            }

            // جلب ColorId فقط وإغلاق الاتصال بقاعدة البيانات بسرعة
            var colorIds = await _Context.ProductDetails
                .AsNoTracking()
                .Where(product => product.ProductId == productId)
                .Select(product => product.ColorId).Distinct()
                .ToListAsync();

            if (colorIds.Count == 1)
            {
                string ColorName = await _colors.GetColorNameByIdAsync(colorIds[0]);
                return new List<string>() { ColorName };
            }
            var colorNames = new List<string>();
            foreach (var id in colorIds)
            {
                colorNames.Add(await _colors.GetColorNameByIdAsync(id));
            }

            return colorNames;
        }
        public async Task<List<string>> GetProductSizesByProductId(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("ProductId must be greater than 0", nameof(productId));
            }
            var sizesId = await _Context.ProductDetails
                .AsNoTracking()
                .Where(product => product.ProductId == productId)
                .Select(product => product.SizeId).Distinct()
                .ToListAsync();
            if(sizesId.Count==1)
            {
               string SizeName= await _Sizes.GetSizeNameByIdAsync(sizesId[0]);
                return  new List<string>() { SizeName };
            }
            var sizesNames = new List<string>();
            foreach (var id in sizesId)
            {
                sizesNames.Add(await _Sizes.GetSizeNameByIdAsync(id));
            }

            // تحقق إذا كانت القائمة غير فارغة ثم أرجعها
            return  sizesNames.Count > 1 ? sizesNames : null!;
        }
        public async Task<ProductsDtos.GetProductDetails> GetDetailsBy(int ProductId, string ColorName, string SizeName = "")
        {
            if (ProductId <= 0 && string.IsNullOrEmpty(ColorName))
            {
                throw new Exception("missing important data");
            }
            int ColorId = await _colors.GetColorIdByColorNameAsync(ColorName);
            int? SizeId = await _Sizes.GetSizeIdByNameAsync(SizeName);
            try
            {
                //this for search by color , size ,product Id
                if (SizeId != null)
                {
                    var ProductDetails = await _Context.ProductDetails
                        .Include(pd => pd.ProductDetailImages)
                        .FirstOrDefaultAsync(
                         Details => Details.ProductId == ProductId
                      && Details.SizeId == SizeId
                      && Details.ColorId == ColorId);
                    if(ProductDetails!=null)
                    {
                    var images = ProductDetails.ProductDetailImages?
                        .OrderBy(img => img.DisplayOrder)
                        .Select(img => img.ImageUrl)
                        .ToList() ?? new List<string>();

                    var result = new ProductsDtos.GetProductDetails
                    {
                        ProductDetailsId = ProductDetails.ProductDetailsId,
                        Image = ProductDetails!.ProductImage,
                        Images = images,
                        Quantity = ProductDetails.Quantity
                    };
                       return result;
                    }
                    return null!;
                }

                //this for search by color and productid only=>that is for products that not contain  sizes
                else
                {
                    var ProductDetails = await _Context.ProductDetails
                        .Include(pd => pd.ProductDetailImages)
                        .FirstOrDefaultAsync(
                    Details => Details.ProductId == ProductId
                    && Details.ColorId == ColorId);

                    var images = ProductDetails?.ProductDetailImages?
                        .OrderBy(img => img.DisplayOrder)
                        .Select(img => img.ImageUrl)
                        .ToList() ?? new List<string>();

                    var result = new ProductsDtos.GetProductDetails
                    {
                        ProductDetailsId = ProductDetails!.ProductDetailsId,
                        Image = ProductDetails!.ProductImage,
                        Images = images,
                        Quantity = ProductDetails.Quantity
                    };
                    return result;
                }

            }
            catch
            {
                throw;
            }

        }
        public async Task<List<string>> GetAllColorsBelongsToSizeName(int ProductId, string SizeName)
        {
            int? SizeId = await _Sizes.GetSizeIdByNameAsync(SizeName);

            if (SizeId == null)
            {
                return new List<string>();
            }

            var productDetails = await _Context.ProductDetails
                .Where(details => details.ProductId == ProductId && details.SizeId == SizeId).Distinct()
                .ToListAsync();

            List<string> colorNames = new List<string>();

            foreach (var details in productDetails)
            {
                string colorName = await _colors.GetColorNameByIdAsync(details.ColorId);
                colorNames.Add(colorName);
            }

            return colorNames;
        }
        public async Task<List<string>> GetAllSizesBelongsToColorName(int ProductId, string ColorName)
        {
            int? ColorId = await _colors.GetColorIdByColorNameAsync(ColorName);

            if (ColorId == null)
            {
                return new List<string>();
            }

            var productDetails = await _Context.ProductDetails
                .Where(details => details.ProductId == ProductId && details.ColorId == ColorId).Distinct()
                .ToListAsync();

            List<string> SizesNames = new List<string>();

            foreach (var details in productDetails)
            {
                string SizeName = await _Sizes.GetSizeNameByIdAsync(details.SizeId);
                SizesNames.Add(SizeName);
            }

            return SizesNames;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                           Admin section
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<List<ProductsDtos.CategorySummaryDto>>GetCategoriesName()
        {
            return await _CategoriesBL.GetCategoriesName();
        }
        public async Task<string?> GetProductImagePathAsync(int ProductDetailsId)
        {
            var productDetails = await _Context.ProductDetails
                .FirstOrDefaultAsync(d => d.ProductDetailsId == ProductDetailsId);
            return productDetails?.ProductImage;
        }

        public static async Task<bool> DeleteImageAsync(string? imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    Console.WriteLine("لا توجد صورة سابقة للحذف.");
                    return true; // لا توجد صورة، إذاً لا يوجد شيء لحذفه.
                }

                if (imagePath.StartsWith("/"))
                {
                    imagePath = imagePath.Substring(1);
                }

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"تم حذف الصورة: {fullPath}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"⚠️ الملف غير موجود: {fullPath}");
                    return true; // الصورة غير موجودة، لا داعي للقلق.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء حذف الصورة: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteLastProductImageAsync(int ProductDetailsId)
        {
            try
            {
                string? imagePath = await GetProductImagePathAsync(ProductDetailsId);
                return await DeleteImageAsync(imagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء حذف صورة المنتج: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductImageAsync(int ProductDetailsId, string fileUrl)
        {
            try
            {
                var productDetails = await _Context.ProductDetails
                    .FirstOrDefaultAsync(d => d.ProductDetailsId == ProductDetailsId);
                if (productDetails == null)
                {
                    return false;
                }
                productDetails.ProductImage = fileUrl;
                _Context.ProductDetails.Update(productDetails);
                return await _Context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UpdateProductDto> GetProductWithDetailsAsync(int productId)
        {
            // محاولة جلب المنتج مع التفاصيل المرتبطة به
            var product = await _Context.Products
                       .Include(p => p.ProductDetails)
                           .ThenInclude(d => d.Color)
                       .Include(p => p.ProductDetails)
                           .ThenInclude(d => d.Size).Include(P=>P.Category)
                       .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                throw new Exception("المنتج غير موجود");

            // التأكد من وجود التفاصيل المرتبطة بالمنتج
            if (product.ProductDetails == null || !product.ProductDetails.Any())
                throw new Exception("تفاصيل المنتج غير موجودة");

            var dto = new UpdateProductDto
            {
                ProductId = product.ProductId,
                ProductNameAr = product.ProductNameAr,
                ProductNameEn = product.ProductNameEn,
                ShortNameAr = product.ShortNameAr,
                ShortNameEn = product.ShortNameEn,
                ProductPrice = product.ProductPrice,
                CategoryName = product.Category.CategoryName, 
                MoreDetailsAr = product.MoreDetailsAr,
                MoreDetailsEn = product.MoreDetailsEn,
                DiscountPercentage = product.DiscountPercentage,
                IsFeatured = product.IsFeatured,
                ProductDetails = product.ProductDetails.Select(d => new UpdateProductDetailDto
                {
                    ProductDetailId = d.ProductDetailsId,
                    ColorName = d.Color?.ColorName ?? "غير محدد", 
                    SizeName = d.Size?.SizeName ?? "غير محدد",
                    Quantity = d.Quantity,
                    ProductImage = d.ProductImage
                }).ToList()
            };

            return dto;
        }
        public async Task<bool> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            // البحث عن المنتج في قاعدة البيانات مع تفاصيله
            var product = await _Context.Products
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductId == updateProductDto.ProductId);

            if (product == null)
                throw new Exception("المنتج غير موجود");

            // تحديث بيانات المنتج الأساسية
            product.ProductNameAr = updateProductDto.ProductNameAr;
            product.ProductNameEn = updateProductDto.ProductNameEn;
            product.ShortNameAr = updateProductDto.ShortNameAr;
            product.ShortNameEn = updateProductDto.ShortNameEn;
            product.ProductPrice = updateProductDto.ProductPrice;
            product.MoreDetailsAr = updateProductDto.MoreDetailsAr;
            product.MoreDetailsEn = updateProductDto.MoreDetailsEn;
            product.DiscountPercentage = updateProductDto.DiscountPercentage;
            product.IsFeatured = updateProductDto.IsFeatured;

            // تحديث التصنيف (إذا كنت تخزن التصنيف ككيان منفصل)
            var category = await _Context.Category.FirstOrDefaultAsync(c => c.CategoryName == updateProductDto.CategoryName);
            if (category == null)
                throw new Exception("التصنيف غير موجود");
            product.CategoryId =await _CategoriesBL.GetCategoryIdByNameAsync(updateProductDto.CategoryName);

            // تحديث تفاصيل المنتج
            var existingDetails = product.ProductDetails.ToList(); // قائمة بالتفاصيل الحالية في قاعدة البيانات

            foreach (var detailDto in updateProductDto.ProductDetails!)
            {
                var existingDetail = existingDetails.FirstOrDefault(d => d.ProductDetailsId == detailDto.ProductDetailId);
                byte ColorId=await _colors.GetColorIdByColorNameAsync(detailDto.ColorName);
                byte? sizeId = await _Sizes.GetSizeIdByNameAsync(detailDto.SizeName!);

            
                if (existingDetail != null)
                {
                    // تحديث التفاصيل الموجودة
                    existingDetail.ColorId = ColorId;
                    existingDetail.SizeId = sizeId;
                    existingDetail.Quantity = detailDto.Quantity;
                    existingDetail.ProductImage = detailDto.ProductImage;
                }
                else
                {
                    // إضافة تفاصيل جديدة
                    product.ProductDetails.Add(new ProductsDetails
                    {
                        ColorId = ColorId,
                        SizeId = sizeId,
                        Quantity = detailDto.Quantity,
                        
                        ProductImage = detailDto.ProductImage
                    });
                }
            }

            // حذف التفاصيل التي لم تعد موجودة في الطلب
            var detailIdsInRequest = updateProductDto.ProductDetails.Select(d => d.ProductDetailId).ToList();
            var detailsToRemove = existingDetails.Where(d => !detailIdsInRequest.Contains(d.ProductDetailsId)).ToList();
            _Context.ProductDetails.RemoveRange(detailsToRemove);

            // حفظ التغييرات في قاعدة البيانات
            await _Context.SaveChangesAsync();

            return true;
        }

        public async Task<List<int>>GetProductsIds()
        {
            try
            {
                var productsIds = await _Context.Products.Select(P => P.ProductId).ToListAsync();
                if(productsIds==null||productsIds.Count==0)
                {
                    throw new Exception("لا توجد منتجات في قاعده البيانات");
                }
                return productsIds;
            }catch(Exception ex)
            { throw new Exception(ex.Message.ToString()); }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _Context.Products
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return false;
            }

            // 1. حذف جميع الردود (ReviewReplies) المرتبطة بتعليقات هذا المنتج
            var reviews = await _Context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (reviews.Any())
            {
                var reviewIds = reviews.Select(r => r.Id).ToList();
                var replies = await _Context.ReviewReplies
                    .Where(rr => reviewIds.Contains(rr.ReviewId))
                    .ToListAsync();
                
                if (replies.Any())
                {
                    _Context.ReviewReplies.RemoveRange(replies);
                }
            }

            // 2. حذف جميع التعليقات (ProductReviews) المرتبطة بهذا المنتج
            if (reviews.Any())
            {
                _Context.ProductReviews.RemoveRange(reviews);
            }

            // 3. حذف المنتج من السلة (CartDetails) ومن تفاصيل الطلبات (OrderDetails)
            if (product.ProductDetails != null && product.ProductDetails.Count > 0)
            {
                var productDetailsIds = product.ProductDetails.Select(pd => pd.ProductDetailsId).ToList();

                // حذف من السلة
                var cartDetails = await _Context.CartDetails
                    .Where(cd => productDetailsIds.Contains(cd.ProductDetailsId))
                    .ToListAsync();
                
                if (cartDetails.Any())
                {
                    _Context.CartDetails.RemoveRange(cartDetails);
                }

                // حذف من تفاصيل الطلبات
                var orderDetails = await _Context.OrderDetails
                    .Where(od => productDetailsIds.Contains(od.ProductDetailsId))
                    .ToListAsync();
                
                if (orderDetails.Any())
                {
                    _Context.OrderDetails.RemoveRange(orderDetails);
                }

                // 4. حذف صور المنتج
                foreach (var details in product.ProductDetails)
                {
                    await DeleteImageAsync(details.ProductImage);
                }

                // 5. حذف تفاصيل المنتج
                _Context.ProductDetails.RemoveRange(product.ProductDetails);
            }

            // 6. حذف المنتج نفسه
            _Context.Products.Remove(product);
            await _Context.SaveChangesAsync();
            return true;
        }

        // Methods for managing ProductDetailImages
        public async Task<int> AddProductDetailImageAsync(ProductsDtos.AddProductDetailImageDto dto)
        {
            try
            {
                var image = new ProductDetailImages
                {
                    ProductDetailsId = dto.ProductDetailsId,
                    ImageUrl = dto.ImageUrl,
                    DisplayOrder = dto.DisplayOrder
                };

                _Context.ProductDetailImages.Add(image);
                await _Context.SaveChangesAsync();
                return image.ProductDetailImageId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product detail image: {ex.Message}");
            }
        }

        public async Task<List<ProductsDtos.ProductDetailImageDto>> GetProductDetailImagesAsync(int productDetailsId)
        {
            try
            {
                var images = await _Context.ProductDetailImages
                    .Where(img => img.ProductDetailsId == productDetailsId)
                    .OrderBy(img => img.DisplayOrder)
                    .Select(img => new ProductsDtos.ProductDetailImageDto
                    {
                        ProductDetailImageId = img.ProductDetailImageId,
                        ProductDetailsId = img.ProductDetailsId,
                        ImageUrl = img.ImageUrl,
                        DisplayOrder = img.DisplayOrder
                    })
                    .ToListAsync();

                return images;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting product detail images: {ex.Message}");
            }
        }

        public async Task<bool> DeleteProductDetailImageAsync(int productDetailImageId)
        {
            try
            {
                var image = await _Context.ProductDetailImages
                    .FirstOrDefaultAsync(img => img.ProductDetailImageId == productDetailImageId);

                if (image == null)
                    return false;

                // Delete the physical file
                await DeleteImageAsync(image.ImageUrl);

                _Context.ProductDetailImages.Remove(image);
                await _Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product detail image: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAllProductDetailImagesAsync(int productDetailsId)
        {
            try
            {
                var images = await _Context.ProductDetailImages
                    .Where(img => img.ProductDetailsId == productDetailsId)
                    .ToListAsync();

                if (!images.Any())
                    return true;

                // Delete physical files
                foreach (var image in images)
                {
                    await DeleteImageAsync(image.ImageUrl);
                }

                _Context.ProductDetailImages.RemoveRange(images);
                await _Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting all product detail images: {ex.Message}");
            }
        }
    }
}
