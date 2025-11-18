namespace StoreDataAccessLayer.Entities
{
    public class Visitor
    {
        public int VisitorId { get; set; }
        public string? IpAddress { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; } // مثل "SA", "EG", "US"
        public string? City { get; set; }
        public string? Region { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? LastActivityTime { get; set; }
        public bool IsActive { get; set; } // للزوار الحاليين
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public int? ClientId { get; set; } // إذا كان زائر مسجل دخول
        public Client? Client { get; set; }
    }
}

