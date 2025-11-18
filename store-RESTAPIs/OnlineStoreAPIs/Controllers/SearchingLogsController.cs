using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBusinessLayer.Clients;
using StoreServices.ClientsServices;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchLogsController : ControllerBase
    {
        private readonly ISearchLogs _searchLogsService;
        private readonly IClient clientsRepo;

        public SearchLogsController(ISearchLogs searchLogsService,IClient Client)
        {
            _searchLogsService = searchLogsService;
            clientsRepo = Client;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSearch([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("يرجى إدخال كلمة البحث.");

            int clientId = 0;

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    int userId = int.Parse(userIdClaim);
                    clientId = await clientsRepo.GetClientIdByUserId(userId);
                }
            }
            catch
            {
            }

            try
            {
                await _searchLogsService.AddSearchAsync(searchTerm, clientId);
                return Ok("تمت إضافة كلمة البحث بنجاح.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء الحفظ: {ex.Message}");
            }
        }


       [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientSearches(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("رقم العميل غير صالح.");

            try
            {
                var results = await _searchLogsService.FindById(clientId);
                if (results.Count == 0)
                    return NotFound("لا توجد عمليات بحث لهذا العميل.");
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ: {ex.Message}");
            }
        }

       [Authorize(Roles ="Admin")]

        [HttpGet("top")]
        public async Task<IActionResult> GetTopSearches()
        {
            try
            {
                var topSearches = await _searchLogsService.FindTheMostlyProductThatPeopleSearchedOn();
                return Ok(topSearches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ: {ex.Message}");
            }
        }

        // ✅ جلب كل كلمات البحث مع أسماء العملاء (إن توفرت)
        [HttpGet("all")]
       [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllSearches()
        {
            try
            {
                var logs = await _searchLogsService.GetAll();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ: {ex.Message.ToString()}");
            }
        }
    }
}
