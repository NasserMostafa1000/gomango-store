using System;

namespace StoreDataAccessLayer.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = true;

        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}


