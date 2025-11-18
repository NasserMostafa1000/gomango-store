using System;

namespace StoreDataAccessLayer.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty; // e.g., AED, SAR, QAR, OMR, USD
        public decimal RateToAED { get; set; } // Base currency is AED (Dirham)
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}


