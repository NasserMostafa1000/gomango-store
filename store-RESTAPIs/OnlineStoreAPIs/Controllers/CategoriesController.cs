using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBusinessLayer.Products;
using StoreServices.Products.ProductInterfaces;
using System.Linq;
using OnlineStoreAPIs.Services;

namespace OnlineStoreAPIs.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductCategory _categories;
        public CategoriesController(IProductCategory categories)
        {
            _categories = categories;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? lang = "ar")
        {
            var data = await _categories.GetCategoriesName();
            var langKey = (lang ?? "ar").ToLower();
            var response = data.Select(category => new
            {
                category.CategoryId,
                Name = langKey == "en" ? category.CategoryNameEn : category.CategoryNameAr,
                category.CategoryNameAr,
                category.CategoryNameEn,
                category.ImagePath
            });
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] ProductsDtos.UpsertCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _categories.CreateCategoryAsync(dto);
            return Ok(new { id });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(byte id, [FromBody] ProductsDtos.UpsertCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dto.CategoryId = id;
            var updated = await _categories.UpdateCategoryAsync(dto);
            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(byte id)
        {
            try
            {
                var deleted = await _categories.DeleteCategoryAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upload-image")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("لم يتم تحميل أي ملف.");
            }

            try
            {
                var fileUrl = await ImageProcessingService.SaveCompressedImageAsync(imageFile, "CategoryImages");
                return Ok(new { imageUrl = fileUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

