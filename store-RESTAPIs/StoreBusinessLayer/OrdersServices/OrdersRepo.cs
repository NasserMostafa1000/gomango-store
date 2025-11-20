
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.OrdersServices;

namespace StoreBusinessLayer.Orders
{
    public class OrdersRepo:IOrder
    {

      private readonly  AppDbContext _Context;


        public OrdersRepo(AppDbContext context)
        {
            _Context = context;
        }
        public async Task<int> PostOrder(OrdersDtos.ClientOrders.PostOrderReq req, int ClientId)
        {
            try
            {
                // إنشاء الطلب الجديد
                var newOrder = new StoreDataAccessLayer.Entities.Orders
                {
                    ClientId = ClientId,
                    // الحالة الافتراضية قيد المعالجة
                    OrderStatusId = 1,
                    PaymentMethodId = req.PaymentMethodId,
                    ShippingCoast = req.ShippingCoast,
                    TotalAmount = req.TotalPrice,
                    // قد يكون null إذا كانت طريقة الدفع "عند استلام المنتج"
                    TransactionNumber = req.TransactionNumber,
                    Address = req.Address,
                };

                // إضافة الطلب إلى قاعدة البيانات
                await _Context.Orders.AddAsync(newOrder);
                await _Context.SaveChangesAsync();

                // إرسال رسالة شكر مع تعليمات متابعة الطلب
                var client = await _Context.Clients
                    .Include(c => c.User) // التأكد من تحميل الـ User مع العميل
                    .FirstOrDefaultAsync(c => c.ClientId == ClientId);
                if (client != null)
                {
                    string customerName = string.IsNullOrEmpty(client.User?.FirstName) ? " " : client.User.FirstName;
                    string orderNumber = newOrder.OrderId.ToString();
                    string message = $@"
عزيزي/عزيزتي {customerName}،

نشكرك على ثقتك بنا! تم استلام طلبك بنجاح في جومانجو ونحن الآن بصدد معالجته.

📦 <strong>رقم طلبك:</strong> {orderNumber}

يمكنك متابعة حالة طلبك في أي وقت عبر حسابك الشخصي على موقعنا الإلكتروني.

إذا كان لديك أي استفسار، لا تتردد في التواصل معنا.

مع أطيب التحيات،
فريق جومانجو";

                    // التحقق من البريد الإلكتروني وإرسال الإشعار
                    var clientEmail = client.User?.EmailOrAuthId; // التأكد من الحصول على البريد الإلكتروني أو معرّف المصادقة
                    if (!string.IsNullOrEmpty(clientEmail))
                    {
                        await NotificationServices.NotificationsCreator.SendNotification(
                            "شكرًا لطلبك في جومانجو",
                           message,
                           clientEmail,  // البريد الإلكتروني الفعلي
                           "gmail");  // يمكنك استبدال "Gmail" بأي مزود إشعار آخر إن كان مختلفًا
                    } 
                    }
                

                return newOrder.OrderId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<int>PostOrderDetail(OrdersDtos.ClientOrders.PostOrderDetailsReq req)
        {
            try
            {
            var newOrderDetails = new OrderDetails
            {
                ProductDetailsId = req.ProductDetailsId,
                UnitPrice=req.UnitPrice,
                Quantity=req.Quantity,
                OrderId=req.OrderId

            };
             await  _Context.OrderDetails.AddAsync(newOrderDetails);
                await _Context.SaveChangesAsync();
                return newOrderDetails.OrderDetailsId;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }
        public List<OrderDetails> PostListOfOrderDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> details,int OrderId)
        {
            return details.Select(detail => new OrderDetails
            {
                ProductDetailsId = detail.ProductDetailsId,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                OrderId= OrderId,
            }).ToList();
        }
        public async Task<bool> PostListOfOrdersDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> req,int OrderId)
        {
           
            try
            {
                List<OrderDetails> Details = PostListOfOrderDetails(req, OrderId);
                await _Context.OrderDetails.AddRangeAsync(Details);
               int RowsAffected= await _Context.SaveChangesAsync();
                return RowsAffected > 0;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }
        public async Task<string>GetOrderStatusNameById(int ID)
        {
            var OrderStatus = await _Context.OrderStatus.FirstOrDefaultAsync(Os => Os.OrderStatusId == ID);
            if(OrderStatus!=null)
            {
                return OrderStatus.StatusName;
            }

            return "";
        }
        public async Task<List<OrdersDtos.ClientOrders.GetOrdersByClientIdReq>> GetOrdersByClientId(int ClientId)
        {
            var orders = await _Context.Orders
                .Where(O => O.ClientId == ClientId).OrderByDescending(O=>O.OrderId)
                .Select(O => new OrdersDtos.ClientOrders.GetOrdersByClientIdReq
                {
                    OrderId = O.OrderId,
                    TotalAmount = O.TotalAmount,
                    OrderStatus = O.OrderStatusId.ToString(), 
                    ShippingCoast=O.ShippingCoast,
                    RejectionReason = O.RejectionReason,
                    OrderDate = O.CreatedAt
                })
                .ToListAsync(); 

            foreach (var order in orders)
            {
                order.OrderStatus = await GetOrderStatusNameById(int.Parse(order.OrderStatus));
            }

            return orders;
        }

        public async Task<List<OrdersDtos.ClientOrders.GetOrderDetailsInSpecificOrderReq>> GetOrderDetailsInSpecificOrder(int OrderId)
        {
            var orderDetailsList = await _Context.OrderDetails
                .Where(detail => detail.OrderId == OrderId)
                .Include(detail => detail.ProductDetails)
                    .ThenInclude(pd => pd.Product)
                .Include(detail => detail.ProductDetails)
                    .ThenInclude(pd => pd.Color)
                .Include(detail => detail.ProductDetails)
                    .ThenInclude(pd => pd.Size)
                .Include(detail => detail.Order)
                    .ThenInclude(order => order.OrderStatus)
                .Select(detail => new OrdersDtos.ClientOrders.GetOrderDetailsInSpecificOrderReq
                {
                    ProductId = detail.ProductDetails.ProductId,
                    ProductName = detail.ProductDetails.Product.ProductNameAr,
                    ImagePath = detail.ProductDetails.ProductImage,
                    Quantity = detail.Quantity,
                    ColorName = detail.ProductDetails.Color.ColorName,
                    SizeName = detail.ProductDetails.Size != null ? detail.ProductDetails.Size.SizeName : null,
                    UnitPrice = detail.UnitPrice,
                    TotalAmount = detail.Quantity * detail.UnitPrice,
                    OrderStatus = detail.Order.OrderStatus.StatusName
                })
                .ToListAsync();

            return orderDetailsList;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        //                                                    Admin Section
        //-------------------------------------------------------------------------------------------------------------------------
        public async Task<List<OrdersDtos.AdminOrders.GetOrdersReq>>GetOrders(int PageNumber)
        {
            var orders = await _Context.Orders
        .OrderByDescending(O => O.OrderId)
        .Include(O => O.Client).ThenInclude(u=>u.User)
        .Include(O => O.OrderStatus)
        .Include(O => O.PaymentMethod)
        .Select(O => new OrdersDtos.AdminOrders.GetOrdersReq
        {
            OrderId = O.OrderId,
            CreatedAt = O.CreatedAt,
            TotalAmount = O.TotalAmount,
            PaymentMethod = O.PaymentMethod.Method,
            TransactionNumber = O.TransactionNumber,
            ShippingCoast = O.ShippingCoast,
            RejectionReason = O.RejectionReason,
            Address = O.Address!,
            FullName = O.Client.User!.FirstName + " " + O.Client.User.SecondName,
            ClientPhone = O.Client.PhoneNumber!,
            OrderStatus = O.OrderStatus.StatusName
        })
        .Paginate(PageNumber)  
        .ToListAsync();
            if(orders!=null)
            {
             return orders!;
            }
            return new List<OrdersDtos.AdminOrders.GetOrdersReq>();

        }
        public async Task<OrdersDtos.AdminOrders.GetOrdersReq?> FindOrder(int OrderId)
        {
            var order = await _Context.Orders
                .Include(O => O.Client).ThenInclude(c=>c.User)
                .Include(O => O.OrderStatus)
                .Include(O => O.PaymentMethod)
                .Where(O => O.OrderId == OrderId)
                .Select(O => new OrdersDtos.AdminOrders.GetOrdersReq
                {
                    OrderId = O.OrderId,
                    CreatedAt = O.CreatedAt,
                    TotalAmount = O.TotalAmount,
                    PaymentMethod = O.PaymentMethod.Method,
                    TransactionNumber = O.TransactionNumber,
                    ShippingCoast=O.ShippingCoast,
                    Address = O.Address!,
                    FullName = O.Client.User!.FirstName + " " + O.Client.User.SecondName,
                    ClientPhone = O.Client.PhoneNumber!,
                    OrderStatus = O.OrderStatus.StatusName
                })
                .FirstOrDefaultAsync();

            return order;
        }
        public  int GetOrderStatusId(string statusName)
        {
            var orderStatusDict = new Dictionary<string, int>
        {
            { "قيد المعالجة", 1 },
            { "تم التأكيد", 2 },
            { "قيد الشحن", 3 },
            { "تم التوصيل", 4 },
            { "تم الإلغاء", 5 },
            { "تم الإرجاع", 6 },
            { "تم الرفض", 7 }
        };

            return orderStatusDict.TryGetValue(statusName, out int statusId) ? statusId : 0;
        }
        //use when we return an order
        public async Task<bool> ProcessOfReturningOrders(int OrderId)
        {
            var OrderDetails = await _Context.OrderDetails
                .Where(Od => Od.OrderId == OrderId)
                .ToListAsync();

            foreach (var orderDetail in OrderDetails)
            {
                var productDetail = await _Context.ProductDetails
                    .FirstOrDefaultAsync(Pd => Pd.ProductDetailsId == orderDetail.ProductDetailsId);

                if (productDetail != null)
                {
                    productDetail.Quantity += orderDetail.Quantity; // استرجاع الكمية
                }
            }

            await _Context.SaveChangesAsync();
            return true;
        }
        //use when we Confirm Order
        public async Task<bool> ProcessOfConfirmingOrders(int OrderId)
        {
            var OrderDetails = await _Context.OrderDetails
                .Where(Od => Od.OrderId == OrderId)
                .ToListAsync();
           
            foreach (var orderDetail in OrderDetails)
            {
                var productDetail = await _Context.ProductDetails
                    .FirstOrDefaultAsync(Pd => Pd.ProductDetailsId == orderDetail.ProductDetailsId);
               
                if (productDetail != null)
                {
                    if (productDetail.Quantity >= orderDetail.Quantity) // التأكد من توفر الكمية
                    {
                        productDetail.Quantity -= orderDetail.Quantity; // خصم الكمية
                    }
                    else
                    {
                        throw new Exception("الكميه غير متوفره"); // فشل العملية بسبب عدم توفر كمية كافية
                    }
                }
            }

            await _Context.SaveChangesAsync();
            return true;
        }
        public string MessageBasedOnTheStatus(string statusName, string customerName, string orderNumber, string rejectionReason = "")
        {
            switch (statusName)
            {
                case "قيد المعالجة":
                    return $@"
عزيزي/عزيزتي {customerName}،

⏳ <strong>طلبك قيد المعالجة</strong>

نحن بصدد معالجة طلبك رقم {orderNumber} في جومانجو. سنقوم بالمتابعة معك وتحديث حالته في أقرب وقت ممكن.

يمكنك متابعة حالة طلبك في أي وقت عبر حسابك الشخصي.

مع أطيب التحيات،
فريق جومانجو";

                case "تم التأكيد":
                    return $@"
عزيزي/عزيزتي {customerName}،

✅ <strong>تم تأكيد طلبك بنجاح!</strong>

نحن سعداء بإبلاغك أن طلبك رقم {orderNumber} في جومانجو قد تم تأكيده بنجاح.

سيتم تجهيز طلبك في أقرب وقت ممكن. يمكنك متابعة حالة طلبك من الموقع الرسمي.

مع أطيب التحيات،
فريق جومانجو";

                case "قيد الشحن":
                    return $@"
عزيزي/عزيزتي {customerName}،

🚚 <strong>طلبك في الطريق إليك!</strong>

تم شحن طلبك رقم {orderNumber} في جومانجو بنجاح. سيتم تسليمه قريباً إلى العنوان الذي قمت بتحديده.

يمكنك متابعة حالة الشحن عبر حسابك الشخصي.

مع أطيب التحيات،
فريق جومانجو";

                case "تم التوصيل":
                    return $@"
عزيزي/عزيزتي {customerName}،

🎉 <strong>تم توصيل طلبك بنجاح!</strong>

نحن سعيدون بإبلاغك أن طلبك رقم {orderNumber} في جومانجو قد تم توصيله بنجاح إلى العنوان المحدد.

نتمنى أن تكون راضياً عن منتجاتك. إذا كان لديك أي استفسار أو تحتاج إلى مساعدة، نحن هنا لخدمتك.

مع أطيب التحيات،
فريق جومانجو";

                case "تم الإلغاء":
                    return $@"
عزيزي/عزيزتي {customerName}،

⚠️ <strong>تم إلغاء طلبك</strong>

نأسف لإبلاغك أنه تم إلغاء طلبك رقم {orderNumber} في جومانجو.

إذا كنت بحاجة إلى معرفة المزيد من التفاصيل أو كان لديك أي استفسار، يمكنك التواصل مع فريق الدعم لدينا.

مع أطيب التحيات،
فريق جومانجو";

                case "تم الإرجاع":
                    return $@"
عزيزي/عزيزتي {customerName}،

↩️ <strong>تم إرجاع طلبك</strong>

لقد تم إرجاع طلبك رقم {orderNumber} في جومانجو بنجاح.

إذا كنت بحاجة إلى المزيد من المساعدة أو لديك أي استفسار، لا تتردد في التواصل مع فريق الدعم لدينا.

مع أطيب التحيات،
فريق جومانجو";

                case "تم الرفض":
                    return $@"
عزيزي/عزيزتي {customerName}،

❌ <strong>تم رفض طلبك</strong>

نأسف لإبلاغك أن طلبك رقم {orderNumber} في جومانجو قد تم رفضه.

<strong>السبب:</strong> {rejectionReason}

إذا كنت بحاجة إلى أي مساعدة إضافية أو توضيحات، لا تتردد في التواصل معنا.

مع أطيب التحيات،
فريق جومانجو";

                default:
                    return $@"
عزيزي/عزيزتي {customerName}،

📋 <strong>تحديث حالة الطلب</strong>

تم تحديث حالة طلبك رقم {orderNumber} في جومانجو.

إذا كنت بحاجة إلى المزيد من المعلومات أو المساعدة، لا تتردد في التواصل مع فريق الدعم لدينا.

مع أطيب التحيات،
فريق جومانجو";
            }
        }
        public async Task<bool> UpdateOrderStatusByName(string statusName, int OrderId, string RejectionReason = "")
        {
            int StatusId = GetOrderStatusId(statusName);

            // العمليات الخاصة بتغير الكمية أو استرجاعها تبقى كما هي
            if (statusName == "تم الإرجاع")
            {
                await ProcessOfReturningOrders(OrderId);
            }
            else if (statusName == "تم التأكيد")
            {
                await ProcessOfConfirmingOrders(OrderId);
            }

            // تحديث حالة الطلب بالإضافة إلى سبب الرفض في حالة "تم الرفض"
            var order = await _Context.Orders
                .Include(o => o.Client) // تحميل العميل المرتبط بالطلب
                .ThenInclude(c => c.User) // تحميل المستخدم المرتبط بالعميل
                .FirstOrDefaultAsync(o => o.OrderId == OrderId);
            if (order != null)
            {
                order.RejectionReason = null!;  // تنظيف سبب الرفض
                order.OrderStatusId = (byte)StatusId;

                if (statusName == "تم الرفض")
                {
                    order.RejectionReason = RejectionReason;  // تخصيص سبب الرفض إذا كانت الحالة "تم الرفض"
                }

                _Context.Orders.Update(order);
                int RowsAffected = await _Context.SaveChangesAsync();

                {
                    if (order != null)
                    {


                        string customerName = order.Client.User!.FirstName+" "+order.Client.User.SecondName;  // استبدالها بالقيمة الفعلية
                        string orderNumber = order.OrderId.ToString();  // استبدالها بالقيمة الفعلية

                        string message = MessageBasedOnTheStatus(statusName, customerName, orderNumber, RejectionReason);

                        // التحقق من البريد الإلكتروني وإرسال الإشعار بناءً عليه
                        var clientEmail = order.Client?.User?.EmailOrAuthId;  // التأكد من الحصول على البريد الإلكتروني أو معرّف المصادقة

                        if (!string.IsNullOrEmpty(clientEmail))
                        {
                            // إذا كان البريد الإلكتروني موجودًا، أرسل الإشعار
                            await NotificationServices.NotificationsCreator.SendNotification(
                                $"تحديث حالة الطلب - {statusName}",
                                message,
                                clientEmail,  // البريد الإلكتروني الفعلي
                                "gmail");  // يمكنك استبدال "Gmail" بأي مزود إشعار آخر إن كان مختلفًا
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public async Task<List<OrdersDtos.AdminOrders.GetOrdersDetailsReq>> GetOrderDetails(int orderId)
        {
            var orderDetailsDto = await _Context.OrderDetails
                .Include(O => O.ProductDetails)
                    .ThenInclude(Pd => Pd.Product)
                .Include(O => O.ProductDetails.Size)
                .Include(O => O.ProductDetails.Color)
                .Where(O => O.OrderId == orderId)
                .Select(O => new OrdersDtos.AdminOrders.GetOrdersDetailsReq
                {
                    ProductId = O.ProductDetails.Product.ProductId,
                    ProductName = O.ProductDetails.Product.ProductNameAr,
                    UnitPrice = O.UnitPrice,
                    ColorName = O.ProductDetails.Color.ColorName,
                    SizeName = O.ProductDetails.Size!.SizeName,
                    Quantity = O.Quantity,
                    TotalPrice = O.UnitPrice * O.Quantity
                })
                .ToListAsync(); 

            return orderDetailsDto;
        }

        public async Task<OrdersDtos.AdminOrders.GetFinancialAnalyticsReq> GetFinancialAnalytics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _Context.Orders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            // الإيرادات الإجمالية
            var totalRevenue = await query.SumAsync(o => o.TotalAmount);
            var totalOrders = await query.CountAsync();

            // إيرادات اليوم
            var today = DateTime.UtcNow.Date;
            var totalRevenueToday = await _Context.Orders
                .Where(o => o.CreatedAt.Date == today)
                .SumAsync(o => o.TotalAmount);
            var ordersToday = await _Context.Orders
                .Where(o => o.CreatedAt.Date == today)
                .CountAsync();

            // إيرادات هذا الشهر
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var totalRevenueThisMonth = await _Context.Orders
                .Where(o => o.CreatedAt >= startOfMonth)
                .SumAsync(o => o.TotalAmount);
            var ordersThisMonth = await _Context.Orders
                .Where(o => o.CreatedAt >= startOfMonth)
                .CountAsync();

            // إيرادات هذه السنة
            var startOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
            var totalRevenueThisYear = await _Context.Orders
                .Where(o => o.CreatedAt >= startOfYear)
                .SumAsync(o => o.TotalAmount);
            var ordersThisYear = await _Context.Orders
                .Where(o => o.CreatedAt >= startOfYear)
                .CountAsync();

            // متوسط قيمة الطلب
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // الإيرادات اليومية (آخر 30 يوم)
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var dailyRevenue = await _Context.Orders
                .Where(o => o.CreatedAt >= thirtyDaysAgo)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new OrdersDtos.AdminOrders.DailyRevenueReq
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // الإيرادات الشهرية (آخر 12 شهر)
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);
            var monthlyRevenueRaw = await _Context.Orders
                .Where(o => o.CreatedAt >= twelveMonthsAgo)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var monthlyRevenue = monthlyRevenueRaw.Select(g => new OrdersDtos.AdminOrders.MonthlyRevenueReq
            {
                Year = g.Year,
                Month = g.Month,
                MonthName = GetMonthName(g.Month),
                Revenue = g.Revenue,
                OrderCount = g.OrderCount
            }).ToList();

            // عدد الطلبات حسب الحالة
            var orderStatusCounts = await _Context.Orders
                .Include(o => o.OrderStatus)
                .GroupBy(o => o.OrderStatus.StatusName)
                .Select(g => new OrdersDtos.AdminOrders.OrderStatusCountReq
                {
                    StatusName = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            return new OrdersDtos.AdminOrders.GetFinancialAnalyticsReq
            {
                TotalRevenue = totalRevenue,
                TotalRevenueToday = totalRevenueToday,
                TotalRevenueThisMonth = totalRevenueThisMonth,
                TotalRevenueThisYear = totalRevenueThisYear,
                TotalOrders = totalOrders,
                OrdersToday = ordersToday,
                OrdersThisMonth = ordersThisMonth,
                OrdersThisYear = ordersThisYear,
                AverageOrderValue = averageOrderValue,
                DailyRevenue = dailyRevenue,
                MonthlyRevenue = monthlyRevenue,
                OrderStatusCounts = orderStatusCounts
            };
        }

        private static string GetMonthName(int month)
        {
            var monthNames = new Dictionary<int, string>
            {
                { 1, "يناير" }, { 2, "فبراير" }, { 3, "مارس" }, { 4, "أبريل" },
                { 5, "مايو" }, { 6, "يونيو" }, { 7, "يوليو" }, { 8, "أغسطس" },
                { 9, "سبتمبر" }, { 10, "أكتوبر" }, { 11, "نوفمبر" }, { 12, "ديسمبر" }
            };
            return monthNames.TryGetValue(month, out var name) ? name : month.ToString();
        }
    }
}
