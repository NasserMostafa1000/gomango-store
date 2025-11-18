using System;

namespace StoreServices.ReviewsServices
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProductName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ReviewsPageDto
    {
        public int ProductId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public double AverageRating { get; set; }
        public ReviewDto[] Items { get; set; } = Array.Empty<ReviewDto>();
    }
}


