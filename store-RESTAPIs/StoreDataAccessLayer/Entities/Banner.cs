using System;

namespace StoreDataAccessLayer.Entities
{
    public class Banner
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SubTitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }
}


