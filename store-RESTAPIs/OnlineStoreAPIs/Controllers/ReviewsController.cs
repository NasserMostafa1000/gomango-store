using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreServices.ReviewsServices;

namespace OnlineStoreAPIs.Controllers
{
    [ApiController]
    [Route("api/products/{productId:int}/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviews _reviews;
        public ReviewsController(IReviews reviews)
        {
            _reviews = reviews;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reviews.GetByProductAsync(productId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("average")]
        public async Task<IActionResult> Average(int productId)
        {
            var avg = await _reviews.GetAverageRatingAsync(productId);
            return Ok(new { productId, average = avg });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(int productId, [FromBody] ReviewDto dto)
        {
            try
            {
                // استخراج UserId من JWT token
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    dto.UserId = userId;
                }
                else
                {
                    return Unauthorized(new { message = "لم يتم العثور على معرف المستخدم في التوكن" });
                }
                
                dto.ProductId = productId;
                var saved = await _reviews.AddAsync(dto);
                return CreatedAtAction(nameof(Get), new { productId, page = 1, pageSize = 10 }, saved);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int userId)
        {
            // التحقق من أن المستخدم يطلب تعليقاته فقط (ما لم يكن أدمن)
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");
            
            if (!isAdmin && (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId) || currentUserId != userId))
            {
                return Forbid();
            }
            
            var reviews = await _reviews.GetByUserIdAsync(userId);
            return Ok(reviews);
        }

        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "لم يتم العثور على معرف المستخدم في التوكن" });
            }
            
            var reviews = await _reviews.GetByUserIdAsync(userId);
            return Ok(reviews);
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int reviewId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");
                
                int? userId = null;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var deleted = await _reviews.DeleteAsync(reviewId, userId, isAdmin);
                
                if (!deleted)
                {
                    return Forbid();
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var reviews = await _reviews.GetAllAsync(page, pageSize);
            return Ok(reviews);
        }
    }
}


