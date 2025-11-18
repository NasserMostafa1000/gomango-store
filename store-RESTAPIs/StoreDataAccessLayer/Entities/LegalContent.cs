using System;

namespace StoreDataAccessLayer.Entities
{
    public class LegalContent
    {
        public int Id { get; set; }
        public string TermsAr { get; set; } = string.Empty;
        public string TermsEn { get; set; } = string.Empty;
        public string PrivacyAr { get; set; } = string.Empty;
        public string PrivacyEn { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}

