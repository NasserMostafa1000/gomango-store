using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreServices.BannersServices;
using OnlineStoreAPIs.Services;

namespace OnlineStoreAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannersController : ControllerBase
    {
        private readonly IBanners _banners;
        public BannersController(IBanners banners)
        {
            _banners = banners;
        }

        [HttpPost("UploadBannerImage")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UploadBannerImage([FromForm] IFormFile imageFile)
        {
            try
            {
                var fileUrl = await ImageProcessingService.SaveCompressedImageAsync(imageFile, "BannersImages");
                return Ok(new { ImageUrl = fileUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive([FromQuery] string? lang = "ar")
        {
            var list = await _banners.GetActiveAsync();
            // إذا كانت اللغة en، نعيد TitleEn و SubTitleEn، وإلا نعيد Ar
            if (lang?.ToLower() == "en")
            {
                var result = list.Select(b => new
                {
                    b.Id,
                    Title = b.TitleEn,
                    SubTitle = b.SubTitleEn,
                    b.ImageUrl,
                    b.LinkUrl,
                    b.IsActive,
                    b.DisplayOrder,
                    b.StartsAt,
                    b.EndsAt
                });
                return Ok(result);
            }
            else
            {
                var result = list.Select(b => new
                {
                    b.Id,
                    Title = b.TitleAr,
                    SubTitle = b.SubTitleAr,
                    b.ImageUrl,
                    b.LinkUrl,
                    b.IsActive,
                    b.DisplayOrder,
                    b.StartsAt,
                    b.EndsAt
                });
                return Ok(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _banners.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _banners.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BannerDto dto)
        {
            var created = await _banners.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BannerDto dto)
        {
            dto.Id = id;
            var ok = await _banners.UpdateAsync(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _banners.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


