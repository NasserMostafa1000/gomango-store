using System;

namespace StoreServices.CurrencyServices
{
    public class CurrencyRateDto
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal RateToAED { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}


