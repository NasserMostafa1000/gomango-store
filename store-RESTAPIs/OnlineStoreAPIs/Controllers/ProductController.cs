using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.Products;
using StoreServices.Products.ProductInterfaces;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
       private readonly IProduct _ProductsRepo;
        public ProductController(IProduct ProductsBl)
        {
            _ProductsRepo = ProductsBl;
        }
        [HttpGet("GetDiscountProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetTodayDiscountProducts(short Page,byte Limit, [FromQuery] string? lang = "ar")
        {

            try
            {
                // Log the received lang parameter
                System.Diagnostics.Debug.WriteLine($"GetTodayDiscountProducts - Received lang: {lang}");
                var result = await _ProductsRepo.GetDiscountsProducts(Page, Limit);
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameEn,
                        ProductName = p.ProductNameEn,
                        MoreDetails = p.MoreDetailsEn,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
                else
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameAr,
                        ProductName = p.ProductNameAr,
                        MoreDetails = p.MoreDetailsAr,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("GetAllProductsWithLimit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetAllProductsWithLimit(short Page, byte Limit, [FromQuery] string? lang = "ar")
        {

            try
            {
                // Log the received lang parameter
                System.Diagnostics.Debug.WriteLine($"GetAllProductsWithLimit - Received lang: {lang}");
                var result = await _ProductsRepo.GetProductsWithLimit(Page, Limit);
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameEn,
                        ProductName = p.ProductNameEn,
                        MoreDetails = p.MoreDetailsEn,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
                else
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameAr,
                        ProductName = p.ProductNameAr,
                        MoreDetails = p.MoreDetailsAr,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("GetFeaturedProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetFeaturedProducts(short Page, byte Limit, [FromQuery] string? lang = "ar")
        {
            try
            {
                var result = await _ProductsRepo.GetFeaturedProducts(Page, Limit);
                if (lang?.ToLower() == "en")
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameEn,
                        ProductName = p.ProductNameEn,
                        MoreDetails = p.MoreDetailsEn,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId,
                        p.IsFeatured
                    });
                    return Ok(mapped);
                }
                else
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameAr,
                        ProductName = p.ProductNameAr,
                        MoreDetails = p.MoreDetailsAr,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId,
                        p.IsFeatured
                    });
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("GetProductsWhereInClothesCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetProductWhereInClothesCategory(short Page, byte Limit, [FromQuery] string? lang = "ar")
        {

            try
            {
                var result = await _ProductsRepo.GetProductsWhereInClothesCategory(Page, Limit);
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameEn,
                        ProductName = p.ProductNameEn,
                        MoreDetails = p.MoreDetailsEn,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
                else
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameAr,
                        ProductName = p.ProductNameAr,
                        MoreDetails = p.MoreDetailsAr,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetProductsByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetProductsByName(string Name, [FromQuery] string? lang = "ar")
        {
            if (string.IsNullOrEmpty(Name))
                return BadRequest(new { message = "يرجى وضع اسم ما تبحث عنه قبل البحث" });

            try
            {
                var result = await _ProductsRepo.GetListProductsWithinName(Name);
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameEn,
                        ProductName = p.ProductNameEn,
                        MoreDetails = p.MoreDetailsEn,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
                else
                {
                    var mapped = result.Select(p => new
                    {
                        p.ProductId,
                        ShortName = p.ShortNameAr,
                        ProductName = p.ProductNameAr,
                        MoreDetails = p.MoreDetailsAr,
                        p.ProductPrice,
                        p.PriceAfterDiscount,
                        p.DiscountPercentage,
                        p.ProductImage,
                        p.CategoryId
                    });
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetProductsById(int ID, [FromQuery] string? lang = "ar")
        {
            if (ID<=0)
                return BadRequest(new { message = "هذا العنصر غير متوفر" });

            try
            {
                // Log the received lang parameter
                System.Diagnostics.Debug.WriteLine($"GetProductsById - Received lang: {lang}");
                var result = await _ProductsRepo.GetProductDetailsByProductId(ID);
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var mapped = new
                    {
                        result.ProductId,
                        ShortName = result.ShortNameEn,
                        ProductName = result.ProductNameEn,
                        MoreDetails = result.MoreDetailsEn,
                        result.ProductPrice,
                        result.PriceAfterDiscount,
                        result.DiscountPercentage,
                        result.ProductImage,
                        result.CategoryId
                    };
                    return Ok(mapped);
                }
                else
                {
                    var mapped = new
                    {
                        result.ProductId,
                        ShortName = result.ShortNameAr,
                        ProductName = result.ProductNameAr,
                        MoreDetails = result.MoreDetailsAr,
                        result.ProductPrice,
                        result.PriceAfterDiscount,
                        result.DiscountPercentage,
                        result.ProductImage,
                        result.CategoryId
                    };
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ShareProduct/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> ShareProduct(int id, [FromQuery] string? lang = "ar")
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "معرف المنتج غير صالح." });
            }

            try
            {
                var result = await _ProductsRepo.GetProductDetailsByProductId(id);
                if (result?.ProductId is null or 0)
                {
                    return NotFound(new { message = "المنتج غير موجود." });
                }

                var isEnglish = string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase);
                var productName = isEnglish ? result.ProductNameEn : result.ProductNameAr;
                var description = isEnglish ? result.MoreDetailsEn : result.MoreDetailsAr;
                var price = result.PriceAfterDiscount > 0 ? result.PriceAfterDiscount : result.ProductPrice;
                var quantity = result.TotalQuantity;
                var imagePath = result.ProductImage ?? string.Empty;

                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
                var imageUrl = imagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? imagePath
                    : $"{baseUrl}{(imagePath.StartsWith("/") ? string.Empty : "/")}{imagePath}";
                var redirectLang = isEnglish ? "en" : "ar";
                var redirectUrl = $"http://localhost:5173/productDetails/{id}?lang={redirectLang}";

                var encodedDescription = System.Net.WebUtility.HtmlEncode(description ?? string.Empty);
                var html = $@"<!DOCTYPE html>
<html lang=""{(isEnglish ? "en" : "ar")}"">
<head>
    <meta charset=""utf-8"" />
    <title>{productName}</title>
    <meta name=""description"" content=""{encodedDescription}"" />
    <meta property=""og:title"" content=""{productName}"" />
    <meta property=""og:description"" content=""{encodedDescription}"" />
    <meta property=""og:image"" content=""{imageUrl}"" />
    <meta property=""og:type"" content=""product"" />
    <meta property=""product:price:amount"" content=""{price:F2}"" />
    <meta property=""product:availability"" content=""{(quantity > 0 ? "in stock" : "out of stock")}"" />
    <meta http-equiv=""refresh"" content=""0; url={redirectUrl}"" />
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, sans-serif; background:#f9f6ef; color:#0a2540; padding:40px; text-align:center; }}
        .card {{ max-width:700px; margin:0 auto; background:#fff; border-radius:24px; padding:32px; box-shadow:0 20px 40px rgba(10,37,64,.1); }}
        img {{ max-width:100%; border-radius:16px; margin:24px auto; }}
        a {{ color:#0a2540; font-weight:bold; text-decoration:none; }}
    </style>
</head>
<body>
    <div class=""card"">
        <h1>{productName}</h1>
        <p>{encodedDescription}</p>
        <p><strong>{(isEnglish ? "Price" : "السعر")}:</strong> {price:F2}</p>
        <p><strong>{(isEnglish ? "Available" : "المتوفر")}:</strong> {Math.Max(quantity, 0)}</p>
        <img src=""{imageUrl}"" alt=""{productName}"" loading=""lazy"" />
        <p><a href=""{redirectUrl}"">{(isEnglish ? "View full details" : "عرض التفاصيل الكاملة")}</a></p>
    </div>
    <script>window.location.href = '{redirectUrl}';</script>
</body>
</html>";

                return Content(html, "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetProductDetailsById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> GetProductDetailsById(int Id)
        {
            try
            {
                return Ok(await _ProductsRepo.GetDetailsByProductId(Id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetSizesByProductId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> GetSizesNameByProductId(int productId)
        {
            try
            {
                var sizes = await _ProductsRepo.GetProductSizesByProductId(productId);

                // إذا كانت القائمة فارغة أو null، يتم إرجاع رسالة واضحة
                if (sizes == null || sizes.Count == 0)
                {
                    return Ok(new { Sizes = "No Sizes" });
                }

                return Ok(sizes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetColorsByProductId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> GetColorsNameByProductId(int productId)
        {
            try
            {
                return Ok(await _ProductsRepo.GetProductColorsByProductId(productId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetDetailsBy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> GetSpecificDetails(int ProductId, string ColorName, string SizeName)
        {
            try
            {
                if (!string.IsNullOrEmpty(SizeName))
                {
                    var result01 = await _ProductsRepo.GetDetailsBy(ProductId, ColorName, SizeName);
                    return Ok(result01);

                }
                var result02 = await _ProductsRepo.GetDetailsBy(ProductId, ColorName);
                return Ok(result02);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpGet("GetColorsBelongsToSpecificSize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult>GetColorsBelongsToSpecificSize(int ProductId,string SizeName)
        {
            if(string .IsNullOrEmpty(SizeName)&&ProductId<=0)
            {
                return BadRequest(new { Message = "missing important data" });
            }
            try
            {
                return  Ok(await _ProductsRepo.GetAllColorsBelongsToSizeName(ProductId, SizeName));
            }catch(Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString() });

            }
        }
        [HttpGet("GetSizesBelongsToSpecificColor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult> GetSizesBelongsToSpecificColor(int ProductId, string ColorName)
        {
            if (string.IsNullOrEmpty(ColorName) && ProductId <= 0)
            {
                return BadRequest(new { Message = "missing important data" });
            }
            try
            {
                return Ok(await _ProductsRepo.GetAllSizesBelongsToColorName(ProductId, ColorName));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message.ToString() });

            }
        }

        [HttpGet("GetProductWithName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductsDtos.GetProductsReq>>> GetProductWithProductName(string name, [FromQuery] string? lang = "ar")
        {

            try
            {
                var result = await _ProductsRepo.GetProductByName(name);
                if (result == null) return NotFound();
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var mapped = new
                    {
                        result.ProductId,
                        ShortName = result.ShortNameEn,
                        ProductName = result.ProductNameEn,
                        MoreDetails = result.MoreDetailsEn,
                        result.ProductPrice,
                        result.PriceAfterDiscount,
                        result.DiscountPercentage,
                        result.ProductImage
                    };
                    return Ok(mapped);
                }
                else
                {
                    var mapped = new
                    {
                        result.ProductId,
                        ShortName = result.ShortNameAr,
                        ProductName = result.ProductNameAr,
                        MoreDetails = result.MoreDetailsAr,
                        result.ProductPrice,
                        result.PriceAfterDiscount,
                        result.DiscountPercentage,
                        result.ProductImage
                    };
                    return Ok(mapped);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                 Admin section
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost("PostProduct")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddNewProduct(ProductsDtos.AddProductReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "معلومات هامه مفقوده" });
            }
            try
            {
                int id = await _ProductsRepo.AddProduct(req);
                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetProductsIds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult>GetProductsIds()
        {
            var ProductsIds = await _ProductsRepo.GetProductsIds();
            return Ok(ProductsIds);
        }

        [HttpPost("PostProductDetails")]
          [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddNewProductDetail(ProductsDtos.AddProductDetails req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "معلومات هامه مفقوده" });
            }
            try
            {
                int id = await _ProductsRepo.AddProductDetails(req);
                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("GetCategoriesNames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [Authorize(Roles ="Admin,Manager")]
        public async Task<ActionResult>GetCategoriesNames()
        {
            try
            {
              
                return Ok(await _ProductsRepo.GetCategoriesName());
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }

        }

        [HttpPost("UploadProductImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UploadProductImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // تحديد مسار المجلد (سوف نستخدم مجلد ProductsImages هنا لتناسق المسارات)
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // استخدام نفس مسار ProductsImages لجميع الدوال
            var fileUrl = $"/ProductsImages/{fileName}";
            return Ok(new { ImageUrl = fileUrl });
        }
        [HttpPut("UpdateProductImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UpdateProductImage(IFormFile imageFile, int ProductDetailsId)
        {
            try
            {
                await _ProductsRepo.DeleteLastProductImageAsync(ProductDetailsId);

                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("لم يتم تحميل أي ملف.");
                }

                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("نوع الملف غير مدعوم.");
                }

                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                string fileUrl = $"/ProductsImages/{fileName}";
                bool isUpdated = await _ProductsRepo.UpdateProductImageAsync(ProductDetailsId, fileUrl);

                return isUpdated ? Ok(new { FileUrl = fileUrl }) : BadRequest("حدث خطأ أثناء تحديث الصورة.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي في السيرفر: {ex.Message}");
            }
        }

        [HttpPut("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductsDtos.UpdateProductDto productDto)
        {
            bool IsUpdated=   await _ProductsRepo.UpdateProductAsync(productDto);

            if (IsUpdated)
                return Ok();
            else
                return BadRequest();
        }
        [HttpGet("GetFullProduct")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult>GetFullProduct(int ProductId)
        {
            try
            {

            return Ok(await _ProductsRepo.GetProductWithDetailsAsync(ProductId));
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.ToString() });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "معرف المنتج غير صالح" });
            }

            var deleted = await _ProductsRepo.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

}

}
