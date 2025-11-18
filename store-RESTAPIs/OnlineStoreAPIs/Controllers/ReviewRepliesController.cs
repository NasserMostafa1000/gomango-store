using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreServices.ReviewsServices;

namespace OnlineStoreAPIs.Controllers
{
    [ApiController]
    [Route("api/reviews/{reviewId:int}/replies")]
    public class ReviewRepliesController : ControllerBase
    {
        private readonly IReviewReplies _replies;
        
        public ReviewRepliesController(IReviewReplies replies)
        {
            _replies = replies;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int reviewId)
        {
            var replies = await _replies.GetByReviewIdAsync(reviewId);
            return Ok(replies);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(int reviewId, [FromBody] ReviewReplyDto dto)
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
                
                dto.ReviewId = reviewId;
                var saved = await _replies.AddAsync(dto);
                return CreatedAtAction(nameof(Get), new { reviewId }, saved);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{replyId:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int replyId)
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
                
                var deleted = await _replies.DeleteAsync(replyId, userId, isAdmin);
                
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
    }
}


