using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreBusinessLayer.Visitors;

namespace StoreServices.VisitorsServices
{
    public interface IVisitor
    {
        Task<int> AddVisitor(VisitorDtos.PostVisitorReq req);
        Task<bool> UpdateVisitorActivity(int visitorId);
        Task<bool> MarkVisitorAsInactive(int visitorId);
        Task<VisitorDtos.GetVisitorsAnalyticsReq> GetVisitorsAnalytics(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<VisitorDtos.GetCurrentVisitorsReq>> GetCurrentActiveVisitors();
        Task<VisitorDtos.GetAllVisitorsPagedReq> GetAllVisitors(int pageNumber = 1, int pageSize = 20);
        Task<int> GetTotalVisitsCount(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetCurrentActiveVisitorsCount();
        Task<int> MarkInactiveVisitors();
    }
}

