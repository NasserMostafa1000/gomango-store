using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineStoreAPIs.Hubs;
using StoreBusinessLayer.Carts;
using StoreBusinessLayer.Clients;
using StoreBusinessLayer.Orders;
using StoreDataAccessLayer.Entities;
using StoreServices.CartServices;
using StoreServices.ClientsServices;
using StoreServices.DiscountCodes;
using StoreServices.OrdersServices;
using Stripe;
using Stripe.Checkout;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPendingOrderStore _pendingOrderStore;
        private readonly IClient _clientsRepo;
        private readonly IOrder _ordersRepo;
        private readonly ICart _cartRepo;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly string _successUrl;
        private readonly string _cancelUrl;
        private readonly string _webhookSecret;
        private readonly IShippingDiscountCodesRepo _discountCodesRepo;

        public PaymentsController(
            IConfiguration configuration,
            IPendingOrderStore pendingOrderStore,
            IClient clientsRepo,
            IOrder ordersRepo,
            ICart cartRepo,
            ILogger<PaymentsController> logger,
            IHubContext<OrderHub> hubContext,
            IShippingDiscountCodesRepo discountCodesRepo)
        {
            _configuration = configuration;
            _pendingOrderStore = pendingOrderStore;
            _clientsRepo = clientsRepo;
            _ordersRepo = ordersRepo;
            _cartRepo = cartRepo;
            _logger = logger;
            _hubContext = hubContext;
            _discountCodesRepo = discountCodesRepo;
            _successUrl = configuration["Stripe:SuccessUrl"] ?? "https://example.com/payment-success";
            _cancelUrl = configuration["Stripe:CancelUrl"] ?? "https://example.com/payment-cancelled";
            _webhookSecret = configuration["Stripe:WebhookSecret"] ?? string.Empty;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("PendingOrders")]
        public async Task<IActionResult> GetPendingOrders([FromQuery] bool includeCompleted = false)
        {
            var pendingOrders = await _pendingOrderStore.GetAllAsync(includeCompleted);

            var response = pendingOrders.Select(p => new
            {
                p.PendingOrderId,
                p.ClientId,
                p.TotalPrice,
                p.ShippingCoast,
                p.PaymentMethodId,
                p.StripeSessionId,
                p.Currency,
                p.CreatedAt,
                p.IsCompleted,
                p.OrderId,
                p.FromCart,
                p.PaymentIntentId,
                p.DiscountCode
            });

            return Ok(response);
        }

        [Authorize(Roles = "User,Manager,Shipping Man")]
        [HttpPost("CreateStripeCheckout")]
        public async Task<ActionResult> CreateStripeCheckout([FromBody] PaymentsDtos.CreateStripeCheckoutSessionReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "بيانات غير مكتملة", ModelState });
            }

            if (req.Products == null || !req.Products.Any())
            {
                return BadRequest(new { message = "قائمة المنتجات مطلوبة" });
            }

            var normalizedCurrency = string.IsNullOrWhiteSpace(req.Currency) ? "aed" : req.Currency.ToLower();
            if (normalizedCurrency != "aed")
            {
                return BadRequest(new { message = "يتم دعم الدرهم الإماراتي فقط حالياً" });
            }

            var itemsTotal = req.Products.Sum(p => p.UnitPrice * p.Quantity);
            var expectedTotal = Math.Round(itemsTotal + req.ShippingCoast, 2);
            if (Math.Round(req.TotalPrice, 2) != expectedTotal)
            {
                return BadRequest(new { message = "تم تعديل السعر، يرجى إعادة تحميل الصفحة" });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var clientId = await _clientsRepo.GetClientIdByUserId(userId);

            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = $"{_successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = _cancelUrl,
                Currency = normalizedCurrency,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = normalizedCurrency,
                            UnitAmount = (long)(req.TotalPrice * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Gomango Order",
                                Description = "طلب من متجر Gomango"
                            }
                        }
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    ["ClientId"] = clientId.ToString(),
                    ["Company"] = "Gomango",
                    ["Website"] = "gomango.shop"
                }
            };

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(sessionOptions);

            var pendingOrder = new PendingOrder
            {
                ClientId = clientId,
                Address = req.Address,
                ShippingCoast = req.ShippingCoast,
                TotalPrice = req.TotalPrice,
                PaymentMethodId = req.PaymentMethodId == 0 ? (byte)1 : req.PaymentMethodId,
                ProductsJson = JsonSerializer.Serialize(req.Products),
                StripeSessionId = session.Id,
                Currency = normalizedCurrency,
                FromCart = req.FromCart,
                DiscountCode = req.DiscountCode
            };

            await _pendingOrderStore.CreateAsync(pendingOrder);

            return Ok(new
            {
                sessionId = session.Id,
                checkoutUrl = session.Url
            });
        }

        [AllowAnonymous]
        [HttpPost("StripeWebhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            // التحقق من وجود WebhookSecret
            if (string.IsNullOrEmpty(_webhookSecret))
            {
                _logger.LogError("WebhookSecret غير موجود في الإعدادات");
                return BadRequest(new { 
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    message = "Webhook secret is not configured"
                });
            }

            // التحقق من وجود Stripe-Signature header
            if (!Request.Headers.ContainsKey("Stripe-Signature"))
            {
                _logger.LogError("Stripe-Signature header غير موجود في الطلب");
                return BadRequest(new { 
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    message = "Stripe-Signature header is missing"
                });
            }

            // تمكين buffering للـ body لضمان قراءته بشكل صحيح
            Request.EnableBuffering();
            Request.Body.Position = 0;

            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            
            // إعادة تعيين موضع الـ stream
            Request.Body.Position = 0;
            
            // التحقق من أن الـ body غير فارغ
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogError("Request body فارغ");
                return BadRequest(new { 
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    message = "Request body is empty"
                });
            }

            Stripe.Event stripeEvent;

            try
            {
                var signature = Request.Headers["Stripe-Signature"].ToString();
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _webhookSecret,
                    tolerance: 300,
                    throwOnApiVersionMismatch: false);
                _logger.LogInformation("تم التحقق من توقيع Stripe بنجاح. نوع الحدث: {EventType}", stripeEvent.Type);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "فشل التحقق من توقيع Stripe. الخطأ: {StripeError}", ex.StripeError?.Message ?? ex.Message);
                return BadRequest(new { 
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    message = "Invalid Stripe signature",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ غير متوقع أثناء التحقق من توقيع Stripe");
                return BadRequest(new { 
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Bad Request",
                    status = 400,
                    message = "Error processing webhook",
                    error = ex.Message
                });
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                {
                    _logger.LogWarning("لم يتمكن النظام من قراءة كائن Session من حدث Stripe.");
                }
                else
                {
                    bool isPaid = IsSessionPaid(session);

                    if (!isPaid)
                    {
                        try
                        {
                            var sessionService = new SessionService();
                            var refreshedSession = await sessionService.GetAsync(session.Id);
                            if (refreshedSession != null)
                            {
                                isPaid = IsSessionPaid(refreshedSession);
                                if (isPaid)
                                {
                                    session = refreshedSession;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "فشل في إعادة جلب جلسة Stripe للتأكد من حالة الدفع.");
                        }
                    }

                    if (isPaid)
                    {
                        await HandleCheckoutSuccess(session);
                    }
                    else
                    {
                        _logger.LogWarning("جلسة Stripe {SessionId} لم يتم تأكيد دفعها بعد. PaymentStatus: {PaymentStatus}, Status: {Status}", session.Id, session.PaymentStatus, session.Status);
                    }
                }
            }

            return Ok();
        }

        private static bool IsSessionPaid(Session session)
        {
            if (session == null) return false;

            bool paymentStatusPaid = session.PaymentStatus != null &&
                session.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase);

            bool statusComplete = session.Status != null &&
                session.Status.Equals("complete", StringComparison.OrdinalIgnoreCase);

            return paymentStatusPaid || statusComplete;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("ReprocessPendingOrders")]
        public async Task<IActionResult> ReprocessPendingOrders()
        {
            var pendings = await _pendingOrderStore.GetAllAsync(includeCompleted: false);
            if (pendings.Count == 0)
            {
                return Ok(new { message = "لا يوجد طلبات معلّقة لإعادة معالجتها حالياً." });
            }

            var sessionService = new SessionService();
            var processed = new List<object>();

            foreach (var pending in pendings)
            {
                try
                {
                    var session = await sessionService.GetAsync(pending.StripeSessionId);
                    if (session == null)
                    {
                        _logger.LogWarning("تعذر إعادة جلب جلسة Stripe للطلب المؤقت {PendingOrderId}", pending.PendingOrderId);
                        processed.Add(new
                        {
                            pending.PendingOrderId,
                            status = "session_not_found"
                        });
                        continue;
                    }

                    if (IsSessionPaid(session))
                    {
                        await HandleCheckoutSuccess(session);
                        processed.Add(new
                        {
                            pending.PendingOrderId,
                            status = "completed",
                            sessionId = session.Id
                        });
                    }
                    else
                    {
                        processed.Add(new
                        {
                            pending.PendingOrderId,
                            status = "not_paid",
                            sessionId = session.Id,
                            paymentStatus = session.PaymentStatus,
                            sessionStatus = session.Status
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "فشل إعادة معالجة الطلب المؤقت {PendingOrderId}", pending.PendingOrderId);
                    processed.Add(new
                    {
                        pending.PendingOrderId,
                        status = "error",
                        message = ex.Message
                    });
                }
            }

            return Ok(new
            {
                message = "تمت محاولة إعادة معالجة الطلبات المعلّقة.",
                results = processed
            });
        }

        [Authorize(Roles = "User,Manager,Shipping Man")]
        [HttpGet("CheckoutStatus")]
        public async Task<ActionResult> GetCheckoutStatus([FromQuery] string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return BadRequest(new { status = "invalid" });
            }

            var pending = await _pendingOrderStore.GetBySessionIdAsync(sessionId);
            if (pending == null)
            {
                return NotFound(new { status = "not_found" });
            }

            if (pending.IsCompleted && pending.OrderId.HasValue)
            {
                // بعد أن يحصل Frontend على OrderId، نحذف السجل المؤقت
                // ننتظر قليلاً للتأكد من أن Frontend حصل على البيانات
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000); // انتظار 5 ثواني
                    try
                    {
                        await _pendingOrderStore.DeleteAsync(pending.PendingOrderId);
                        _logger.LogInformation("تم حذف الطلب المؤقت {PendingOrderId} بعد استرجاع OrderId", pending.PendingOrderId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "فشل حذف الطلب المؤقت {PendingOrderId}", pending.PendingOrderId);
                    }
                });

                return Ok(new { 
                    status = "completed", 
                    orderId = pending.OrderId.Value,
                    discountCode = pending.DiscountCode
                });
            }

            return Ok(new { status = "pending" });
        }

        private async Task HandleCheckoutSuccess(Session session)
        {
            var pending = await _pendingOrderStore.GetBySessionIdAsync(session.Id);
            if (pending == null)
            {
                _logger.LogWarning("تعذر العثور على طلب معلق للجلسة {SessionId}", session.Id);
                return;
            }

            if (pending.IsCompleted)
            {
                return;
            }

            try
            {
                var orderReq = new OrdersDtos.ClientOrders.PostOrderReq
                {
                    Address = pending.Address,
                    TotalPrice = pending.TotalPrice,
                    ShippingCoast = pending.ShippingCoast,
                    PaymentMethodId = pending.PaymentMethodId,
                    TransactionNumber = session.PaymentIntentId ?? session.Id
                };

                int orderId = await _ordersRepo.PostOrder(orderReq, pending.ClientId);

                var products = JsonSerializer.Deserialize<List<PaymentsDtos.CheckoutProductDto>>(pending.ProductsJson) ?? new();

                if (products.Count == 1)
                {
                    var single = products[0];
                    await _ordersRepo.PostOrderDetail(new OrdersDtos.ClientOrders.PostOrderDetailsReq
                    {
                        OrderId = orderId,
                        ProductDetailsId = single.ProductDetailsId,
                        Quantity = single.Quantity,
                        UnitPrice = single.UnitPrice
                    });
                }
                else if (products.Count > 1)
                {
                    var details = products.Select(p => new OrdersDtos.ClientOrders.PostOrderDetailsReq
                    {
                        OrderId = orderId,
                        ProductDetailsId = p.ProductDetailsId,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice
                    }).ToList();

                    await _ordersRepo.PostListOfOrdersDetails(details, orderId);
                    await _cartRepo.RemoveCartDetailsByClientId(pending.ClientId);
                }

                // في حال لم يكن هناك كود خصم مخزّن مسبقاً، نحاول منح العميل كود شحن مجاني جديد
                if (pending.PaymentMethodId == 1 && string.IsNullOrWhiteSpace(pending.DiscountCode))
                {
                    try
                    {
                        var rewardCode = await _discountCodesRepo.GetRandomActiveDiscountCodeAsync();
                        if (!string.IsNullOrWhiteSpace(rewardCode))
                        {
                            pending.DiscountCode = rewardCode;
                            _logger.LogInformation("تم منح كود خصم للشحن للطلب {OrderId}: {Code}", orderId, rewardCode);
                        }
                        else
                        {
                            _logger.LogWarning("لا توجد أكواد خصم متاحة حالياً لمنحها للطلب {OrderId}", orderId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "حدث خطأ أثناء محاولة جلب كود خصم للشحن للطلب {OrderId}", orderId);
                    }
                }

                pending.IsCompleted = true;
                pending.OrderId = orderId;
                pending.PaymentIntentId = session.PaymentIntentId ?? session.Id;
                await _pendingOrderStore.UpdateAsync(pending);

                // ملاحظة: نحتفظ بالسجل المؤقت لفترة قصيرة للسماح لـ Frontend بالتحقق من حالة الطلب
                // يمكن إضافة Background Job لحذف السجلات المكتملة الأقدم من ساعة واحدة

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", $"new Order{orderId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل إنشاء الطلب النهائي بعد دفع Stripe");
            }
        }
    }
}

