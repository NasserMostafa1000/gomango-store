using System;

namespace StoreServices.BannersServices
{
    public class BannerDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SubTitleAr { get; set; }
        public string? SubTitleEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }
}


