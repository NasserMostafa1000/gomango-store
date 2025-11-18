using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoreBusinessLayer.Visitors;
using StoreServices.VisitorsServices;
using StoreDataAccessLayer;
using System.Security.Claims;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/Visitors")]
    [ApiController]
    public class VisitorsController : ControllerBase
    {
        private readonly IVisitor _visitorRepo;

        public VisitorsController(IVisitor visitorRepo)
        {
            _visitorRepo = visitorRepo;
        }

        [HttpPost("AddVisitor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddVisitor([FromBody] VisitorDtos.PostVisitorReq req)
        {
            try
            {
                // الحصول على IP من الطلب
                if (string.IsNullOrEmpty(req.IpAddress))
                {
                    req.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                // الحصول على User-Agent
                if (string.IsNullOrEmpty(req.UserAgent))
                {
                    req.UserAgent = Request.Headers["User-Agent"].ToString();
                }

                // الحصول على Referrer
                if (string.IsNullOrEmpty(req.Referrer))
                {
                    req.Referrer = Request.Headers["Referer"].ToString();
                }

                // الحصول على ClientId من Token إذا كان المستخدم مسجل دخول
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        // البحث عن ClientId من UserId
                        using (var scope = HttpContext.RequestServices.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<StoreDataAccessLayer.AppDbContext>();
                            var client = await dbContext.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
                            if (client != null)
                            {
                                req.ClientId = client.ClientId;
                            }
                        }
                    }
                }

                int visitorId = await _visitorRepo.AddVisitor(req);
                return Ok(new { VisitorId = visitorId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateActivity/{visitorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateVisitorActivity(int visitorId)
        {
            try
            {
                bool updated = await _visitorRepo.UpdateVisitorActivity(visitorId);
                if (updated)
                    return Ok(new { message = "تم تحديث النشاط بنجاح" });
                return BadRequest(new { message = "لم يتم العثور على الزائر" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAnalytics")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetVisitorsAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var analytics = await _visitorRepo.GetVisitorsAnalytics(startDate, endDate);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetCurrentVisitors")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetCurrentActiveVisitors()
        {
            try
            {
                var visitors = await _visitorRepo.GetCurrentActiveVisitors();
                return Ok(visitors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetTotalVisits")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetTotalVisits([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                int count = await _visitorRepo.GetTotalVisitsCount(startDate, endDate);
                return Ok(new { TotalVisits = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetCurrentActiveCount")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetCurrentActiveVisitorsCount()
        {
            try
            {
                int count = await _visitorRepo.GetCurrentActiveVisitorsCount();
                return Ok(new { CurrentActiveVisitors = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllVisitors")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllVisitors([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _visitorRepo.GetAllVisitors(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("MarkInactiveVisitors")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> MarkInactiveVisitors()
        {
            try
            {
                int count = await _visitorRepo.MarkInactiveVisitors();
                return Ok(new { Message = $"تم تحديث {count} زائر كغير نشط", Count = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

