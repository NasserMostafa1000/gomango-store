using System;

namespace StoreServices.ReviewsServices
{
    public class ReviewReplyDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Reply { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}


