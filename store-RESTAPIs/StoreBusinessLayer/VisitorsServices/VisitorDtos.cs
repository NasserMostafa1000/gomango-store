using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreBusinessLayer.Visitors
{
    public class VisitorDtos
    {
        public class PostVisitorReq
        {
            public string? IpAddress { get; set; }
            public string? Country { get; set; }
            public string? CountryCode { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? UserAgent { get; set; }
            public string? Referrer { get; set; }
            public int? ClientId { get; set; }
        }

        public class GetVisitorsAnalyticsReq
        {
            public int TotalVisits { get; set; }
            public int CurrentActiveVisitors { get; set; }
            public List<GeographicDistributionReq> GeographicDistribution { get; set; } = new();
            public List<DailyVisitsReq> DailyVisits { get; set; } = new();
        }

        public class GeographicDistributionReq
        {
            public string Country { get; set; } = null!;
            public string CountryCode { get; set; } = null!;
            public string? City { get; set; }
            public int VisitCount { get; set; }
            public string CountryFlagIcon { get; set; } = null!; // URL أو emoji للعلم
        }

        public class DailyVisitsReq
        {
            public DateTime Date { get; set; }
            public int VisitCount { get; set; }
        }

        public class GetCurrentVisitorsReq
        {
            public int VisitorId { get; set; }
            public string? IpAddress { get; set; }
            public string? Country { get; set; }
            public string? CountryCode { get; set; }
            public string? City { get; set; }
            public DateTime VisitTime { get; set; }
            public DateTime? LastActivityTime { get; set; }
            public string CountryFlagIcon { get; set; } = null!;
            public string? ClientName { get; set; }
        }

        public class GetAllVisitorsReq
        {
            public int VisitorId { get; set; }
            public string? IpAddress { get; set; }
            public string? Country { get; set; }
            public string? CountryCode { get; set; }
            public string? City { get; set; }
            public DateTime VisitTime { get; set; }
            public DateTime? LastActivityTime { get; set; }
            public bool IsActive { get; set; }
            public string CountryFlagIcon { get; set; } = null!;
            public string? ClientName { get; set; }
        }

        public class GetAllVisitorsPagedReq
        {
            public List<GetAllVisitorsReq> Visitors { get; set; } = new();
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
        }
    }
}

