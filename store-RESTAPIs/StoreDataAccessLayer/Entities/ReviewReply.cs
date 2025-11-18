using System;

namespace StoreDataAccessLayer.Entities
{
    public class ReviewReply
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int? UserId { get; set; }
        public string Reply { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = true;

        public ProductReview? Review { get; set; }
        public User? User { get; set; }
    }
}


