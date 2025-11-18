using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineStoreAPIs.Hubs;
using StoreBusinessLayer.Carts;
using StoreBusinessLayer.Clients;
using StoreBusinessLayer.NotificationServices;
using StoreBusinessLayer.Orders;
using StoreDataAccessLayer.Entities;
using StoreServices.CartServices;
using StoreServices.ClientsServices;
using StoreServices.OrdersServices;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/Orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrder _OrdersRepo;
        private readonly IClient _ClientsRepo;
        private readonly ICart _CartsRepo;
       public OrdersController(IOrder ordersBL, IClient ClientsBL, ICart carts)
        {
            _OrdersRepo = ordersBL;
            _ClientsRepo = ClientsBL;
            _CartsRepo = carts;
        }
        //---------------------------------------------------------------------------------------------------
        //                                       Client Section
        //---------------------------------------------------------------------------------------------------
        [Authorize(Roles = "User,Manager,Shipping Man")]
        [HttpPost("PostOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult>PostOrder(OrdersDtos.ClientOrders.PostOrderReq req)
        {
            int UserId =int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            try
            {
               int OrderId= await _OrdersRepo.PostOrder(req, ClientId);
                return Ok(new { OrderId = OrderId });
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
      

        }


        [Authorize(Roles = "User,Manager,Shipping Man")]
        [HttpPost("PostOrderDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostOrderDetails(OrdersDtos.ClientOrders.PostOrderDetailsReq req)
        {
            try
            {
                int OrderDetailsId = await _OrdersRepo.PostOrderDetail(req);
                return Ok(new { OrderDetailsId = OrderDetailsId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }

        }

        [HttpPost("PostListOfOrderDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "User,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostListOrderDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> req,int OrderId)
        {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "البيانات غير صحيحة", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            try
            {
                bool IsAdded = await _OrdersRepo.PostListOfOrdersDetails(req, OrderId);
                if(IsAdded)
                {
                   await _CartsRepo.RemoveCartDetailsByClientId(ClientId);
                return Ok();
                }
                return BadRequest( "internal server error");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.ToString() });
            }

        }
        [HttpGet("GetOrdersByClientId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "User,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult>GetOrdersByClientId()
        {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            try
            {
                var Orders =await _OrdersRepo.GetOrdersByClientId(ClientId);
                if(Orders!=null)
                {
                    return Ok(Orders);
                }
                return NotFound("No Orders For This Client");
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }


        [HttpGet("GetOrderDetailsInSpecificOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "User,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetOrderDetailsInSpecificOrder(int OrderId)
        {
            
            try
            {
                var Details =await _OrdersRepo.GetOrderDetailsInSpecificOrder(OrderId);
                if(Details!=null)
                {
                    return Ok(Details);

                }
                return NotFound("هذا الطلب لا يحتوي علي تفاصيل!");
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        //---------------------------------------------------------------------------------------------------
        //                                       Admin Section
        //---------------------------------------------------------------------------------------------------
        [HttpGet("GetOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult>GetOrders(short PageNum)
        {
            try
            {

            var Orders = await _OrdersRepo.GetOrders(PageNum);
            if(Orders!=null)
            {
                return Ok( Orders);
            }
            return NotFound("لا يوجد طلبات");
            }catch(Exception ex)
            {
                return Ok(new  { message = ex.Message.ToString() });
            }
        }


        [HttpGet("FindOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> FindOrders(int OrderId)
        {
            try
            {

                var Orders = await _OrdersRepo.FindOrder(OrderId);
                if (Orders != null)
                {
                    return Ok(Orders);
                }
                return NotFound("لا يوجد طلبات");
            }
            catch (Exception ex)
            {
                return Ok(new { message = ex.Message.ToString() });
            }
        }


        [HttpPut("UpdateOrderStatues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateOrderStatues(int OrderId,string StatusName,string RejectionReason="")
        {
            try
            {       
                bool IsUpdated = await _OrdersRepo.UpdateOrderStatusByName(StatusName,OrderId, RejectionReason);
                if (IsUpdated)
                {
                    return Ok();
                }
                return NotFound("خطأ في تحديث الحاله");
            }
            catch (Exception ex)
            {
                return Ok(new { message = ex.Message.ToString() });
            }
        }
        [HttpGet("GetOrderDetails")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Manager,Shipping Man")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> OrderDetails([FromQuery] int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("معرف الطلب غير صالح");
            }

            var orderDetails = await _OrdersRepo.GetOrderDetails(orderId);

            if (orderDetails == null )
            {
                return NotFound("لم يتم العثور على تفاصيل الطلب");
            }

            return Ok(orderDetails);
        }

        [HttpGet("GetFinancialAnalytics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetFinancialAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var analytics = await _OrdersRepo.GetFinancialAnalytics(startDate, endDate);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
