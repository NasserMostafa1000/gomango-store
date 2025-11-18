using System.ComponentModel.DataAnnotations;

namespace StoreBusinessLayer.Orders
{
    public class PaymentsDtos
    {
        public class CheckoutProductDto
        {
            public int ProductDetailsId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class CreateStripeCheckoutSessionReq
        {
            [Required]
            public string Address { get; set; } = null!;
            [Required]
            public decimal TotalPrice { get; set; }
            [Required]
            public decimal ShippingCoast { get; set; }
            public byte PaymentMethodId { get; set; } = 1;
            public string Currency { get; set; } = "aed";
            public bool FromCart { get; set; }
            public string? DiscountCode { get; set; }
            [MinLength(1, ErrorMessage = "Products list cannot be empty")]
            public List<CheckoutProductDto> Products { get; set; } = new();
        }
    }
}

