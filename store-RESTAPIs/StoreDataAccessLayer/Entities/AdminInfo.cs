using System.ComponentModel.DataAnnotations.Schema;

namespace StoreDataAccessLayer.Entities
{
   [Table("AdminInfo")]
    public class AdminInfo
    {
        public byte Id { get; set; }
        public string TransactionNumber { get; set; } = null!;
        public string WhatsAppNumber { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? AboutUsAr { get; set; }
        public string? AboutUsEn { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
    }
}
