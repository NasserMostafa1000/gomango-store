namespace StoreDataAccessLayer.Entities
{
    public class Cart
    {
        public int CartId { get; set; }
        public int? ClientId { get; set; } // Nullable للسماح بالسلة المؤقتة
        public string? SessionId { get; set; } // للزوار غير المسجلين
        public Client? Client { get; set; }
        public ICollection<CartDetails> cartDetails { get; set; } = null!;
    }
}
