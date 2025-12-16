using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Orders;
using StoreDataAccessLayer.Entities;

namespace StoreServices.OrdersServices
{
    public interface IOrder
    {
        Task<int> PostOrder(OrdersDtos.ClientOrders.PostOrderReq req, int ClientId);
        Task<int> PostOrderDetail(OrdersDtos.ClientOrders.PostOrderDetailsReq req);
        List<OrderDetails> PostListOfOrderDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> details, int OrderId);
        Task<bool> PostListOfOrdersDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> req, int OrderId);
        Task<int> PostGuestOrder(OrdersDtos.GuestOrders.PostGuestOrderReq req);
        Task<string> GetOrderStatusNameById(int ID);
        Task<List<OrdersDtos.ClientOrders.GetOrdersByClientIdReq>> GetOrdersByClientId(int ClientId);
        Task<List<OrdersDtos.ClientOrders.GetOrderDetailsInSpecificOrderReq>> GetOrderDetailsInSpecificOrder(int OrderId);

        //-------------------------------------------------------------------------------------------------------------------------
        //                                                    Admin Section
        //-------------------------------------------------------------------------------------------------------------------------
         Task<List<OrdersDtos.AdminOrders.GetOrdersReq>> GetOrders(int PageNumber);
         Task<OrdersDtos.AdminOrders.GetOrdersReq?> FindOrder(int OrderId);
         int GetOrderStatusId(string statusName);
         Task<bool> ProcessOfReturningOrders(int OrderId);
         Task<bool> ProcessOfConfirmingOrders(int OrderId);
         string MessageBasedOnTheStatus(string statusName, string customerName, string orderNumber, string rejectionReason = "");
         Task<bool> UpdateOrderStatusByName(string statusName, int OrderId, string RejectionReason = "");
         Task<List<OrdersDtos.AdminOrders.GetOrdersDetailsReq>> GetOrderDetails(int orderId);
         Task<OrdersDtos.AdminOrders.GetFinancialAnalyticsReq> GetFinancialAnalytics(DateTime? startDate = null, DateTime? endDate = null);

    }
}
