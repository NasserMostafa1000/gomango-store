using System.Security.Claims;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreBusinessLayer.Carts;
using StoreBusinessLayer.Clients;
using StoreDataAccessLayer.Entities;
using StoreServices.CartServices;
using StoreServices.ClientsServices;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/Carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICart _CartRepo;
       private readonly IClient _ClientsRepo;
        public  CartsController(ICart Cart ,IClient Clients)
        {
            _CartRepo=Cart;
            _ClientsRepo = Clients;
        }

        [HttpPost("PostCartDetails")]
        [Authorize(Roles ="User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult>PostCartDetails(CartDtos.AddCartDetailsReq req)
        {
            try
            {
                int UserID =int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int ClientId =await _ClientsRepo.GetClientIdByUserId(UserID);
                return Ok(await _CartRepo.AddCartDetailsToSpecificClient(req, ClientId));
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("GetCartDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles="User")] 
        public async Task<ActionResult> GetCartDetailsByClientId()
        {
            int Users = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int clientId = await _ClientsRepo.GetClientIdByUserId(Users);
            try
            {
                var cartDetails = await _CartRepo.GetCartDetailsByClientId(clientId);

                if (cartDetails == null || cartDetails.Count == 0)
                {
                    return NotFound("السلة فارغة أو العميل غير موجود.");
                }

                return Ok(cartDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء جلب بيانات السلة: {ex.Message}");
            }
        }
        [HttpGet("GetCartCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetCartCountByClientId()
        {
            int Users = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int clientId = await _ClientsRepo.GetClientIdByUserId(Users);
            try
            {
                var cartDetails = await _CartRepo.GetCartDetailsByClientId(clientId);

                if (cartDetails == null || cartDetails.Count == 0)
                {
                    Ok(0);
                }

                return Ok(cartDetails?.Count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء جلب بيانات السلة: {ex.Message}");
            }
        }
        [HttpDelete("RemoveCartDetails/{cartId}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RemoveCartDetailsByCartId(int cartId)
        {
            try
            {
                bool isRemoved = await _CartRepo.RemoveCartDetailsByCartId(cartId);

                if (!isRemoved)
                {
                    return BadRequest("لم يتم العثور على تفاصيل السلة أو تم حذفها بالفعل.");
                }

                return Ok("تم حذف تفاصيل السلة بنجاح.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("RemoveProduct/{CartDetailsId}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RemoveProductDetailsById(int CartDetailsId)
        {
            try
            {
                bool isRemoved = await _CartRepo.RemoveItemOnCartByCartDetailsId(CartDetailsId);

                if (!isRemoved)
                {
                    return BadRequest("لم يتم العثور على المنتج أو تم حذفه بالفعل.");
                }

                return Ok("تم حذف المنتج بنجاح.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("RemoveGuestProduct/{CartDetailsId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveGuestProductDetailsById(int CartDetailsId, [FromHeader] string? sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    return BadRequest(new { message = "SessionId مطلوب" });
                }

                bool isRemoved = await _CartRepo.RemoveItemOnCartByCartDetailsId(CartDetailsId);

                if (!isRemoved)
                {
                    return BadRequest("لم يتم العثور على المنتج أو تم حذفه بالفعل.");
                }

                return Ok("تم حذف المنتج بنجاح.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //---------------------------------------------------------------------------------------------------
        //                                       Guest Cart Section
        //---------------------------------------------------------------------------------------------------
        [HttpPost("PostGuestCartDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostGuestCartDetails(CartDtos.AddCartDetailsReq req, [FromHeader] string? sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    return BadRequest(new { message = "SessionId مطلوب" });
                }

                int cartId = await _CartRepo.AddCartDetailsToGuestCart(req, sessionId);
                return Ok(new { CartId = cartId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetGuestCartDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetGuestCartDetails([FromHeader] string? sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest(new { message = "SessionId مطلوب" });
            }

            try
            {
                var cartDetails = await _CartRepo.GetCartDetailsBySessionId(sessionId);

                if (cartDetails == null || cartDetails.Count == 0)
                {
                    return Ok(new List<CartDtos.GetCartReq>());
                }

                return Ok(cartDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء جلب بيانات السلة: {ex.Message}");
            }
        }

        [HttpPost("MergeGuestCart")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> MergeGuestCartToClientCart([FromHeader] string? sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    return BadRequest(new { message = "SessionId مطلوب" });
                }

                int UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int ClientId = await _ClientsRepo.GetClientIdByUserId(UserID);

                bool merged = await _CartRepo.MergeGuestCartToClientCart(sessionId, ClientId);

                if (merged)
                {
                    return Ok(new { message = "تم دمج السلة المؤقتة مع سلة المستخدم بنجاح" });
                }

                return Ok(new { message = "لا توجد سلة مؤقتة للدمج" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }

}

