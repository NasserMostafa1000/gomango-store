namespace StoreDataAccessLayer.Entities
{
    public class Orders
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ClientId { get; set; }
        public bool IsGuest { get; set; }
        public string? GuestName { get; set; }
        public string? GuestPhone { get; set; }
        public string? GuestEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCoast { get; set; }
        public byte OrderStatusId { get; set; }
        public string? TransactionNumber { get; set; }
        public string? Address { get; set; } = null!;

        public string? RejectionReason { get; set; }
        public byte PaymentMethodId { get; set; } 


        public Client? Client { get; set; }
        public OrderStatus OrderStatus { get; set; } = null!;
        public PaymentsMethods PaymentMethod { get; set; } = null!;
        public ICollection<OrderDetails> OrderDetails { get; set; } = null!;
    }
}
