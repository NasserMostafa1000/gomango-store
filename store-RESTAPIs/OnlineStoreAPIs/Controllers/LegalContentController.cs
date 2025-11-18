using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBusinessLayer.LegalContentServices;
using System.Threading.Tasks;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegalContentController : ControllerBase
    {
        private readonly ILegalContentService _legalContentService;

        public LegalContentController(ILegalContentService legalContentService)
        {
            _legalContentService = legalContentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<LegalContentDto>> Get()
        {
            var content = await _legalContentService.GetAsync();
            return Ok(content);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] LegalContentDto dto)
        {
            var updated = await _legalContentService.UpsertAsync(dto);
            if (updated)
            {
                return Ok(new { message = "تم تحديث المحتوى القانوني بنجاح." });
            }
            return StatusCode(500, new { message = "فشل تحديث المحتوى القانوني." });
        }
    }
}

