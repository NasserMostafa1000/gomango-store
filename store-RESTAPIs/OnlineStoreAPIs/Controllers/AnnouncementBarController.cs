using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreServices.BannersServices;

namespace OnlineStoreAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementBarController : ControllerBase
    {
        private readonly IAnnouncementBar _announcementBar;
        public AnnouncementBarController(IAnnouncementBar announcementBar)
        {
            _announcementBar = announcementBar;
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActive([FromQuery] string? lang = "ar")
        {
            try
            {
                var item = await _announcementBar.GetActiveAsync();
                if (item == null)
                {
                    // Log for debugging (only in development)
                    #if DEBUG
                    System.Diagnostics.Debug.WriteLine("AnnouncementBar: No active announcement found");
                    #endif
                    return NotFound(new { message = "No active announcement found" });
                }
                
                // إرجاع البيانات حسب اللغة
                if (lang?.ToLower() == "en")
                {
                    var result = new
                    {
                        item.Id,
                        Text = item.TextEn,
                        item.LinkUrl,
                        item.IsActive
                    };
                    return Ok(result);
                }
                else
                {
                    var result = new
                    {
                        item.Id,
                        Text = item.TextAr,
                        item.LinkUrl,
                        item.IsActive
                    };
                    return Ok(result);
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 208) // Invalid object name
            {
                // جدول AnnouncementBars غير موجود
                #if DEBUG
                System.Diagnostics.Debug.WriteLine($"AnnouncementBar: Table not found - {ex.Message}");
                #endif
                return NotFound(new { message = "AnnouncementBars table does not exist. Please run the migration or CREATE_AnnouncementBars_AND_DATA.sql script." });
            }
            catch (Exception ex)
            {
                // أي خطأ آخر
                #if DEBUG
                System.Diagnostics.Debug.WriteLine($"AnnouncementBar: Error - {ex.Message}");
                #endif
                return NotFound(new { message = "An error occurred while fetching announcement" });
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _announcementBar.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] AnnouncementBarDto dto)
        {
            var created = await _announcementBar.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] AnnouncementBarDto dto)
        {
            dto.Id = id;
            var ok = await _announcementBar.UpdateAsync(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _announcementBar.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

