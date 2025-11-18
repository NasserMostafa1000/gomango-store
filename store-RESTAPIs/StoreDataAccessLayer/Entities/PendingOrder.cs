using System.ComponentModel.DataAnnotations;

namespace StoreDataAccessLayer.Entities
{
    public class PendingOrder
    {
        public int PendingOrderId { get; set; }
        public int ClientId { get; set; }
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public decimal TotalPrice { get; set; }
        [Required]
        public decimal ShippingCoast { get; set; }
        [Required]
        public byte PaymentMethodId { get; set; }
        [Required]
        public string ProductsJson { get; set; } = null!;
        [Required]
        public string StripeSessionId { get; set; } = null!;
        public string Currency { get; set; } = "aed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; }
        public int? OrderId { get; set; }
        public bool FromCart { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? DiscountCode { get; set; }
    }
}

