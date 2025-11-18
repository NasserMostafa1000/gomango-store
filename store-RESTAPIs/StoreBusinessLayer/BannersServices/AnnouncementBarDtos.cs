using System;

namespace StoreServices.BannersServices
{
    public class AnnouncementBarDto
    {
        public int Id { get; set; }
        public string TextAr { get; set; } = string.Empty;
        public string TextEn { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

