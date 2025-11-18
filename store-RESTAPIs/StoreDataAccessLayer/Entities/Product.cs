namespace StoreDataAccessLayer.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductNameAr { get; set; } = null!;
        public string ProductNameEn { get; set; } = null!;
        public string ShortNameAr { get; set; } = null!;
        public string ShortNameEn { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public byte CategoryId { get; set; }
        public string MoreDetailsAr { get; set; } = null!;
        public string MoreDetailsEn { get; set; } = null!;
        public bool IsFeatured { get; set; }
        public ICollection<ProductsDetails> ProductDetails { get; set; } = null!;

        public Category Category { get; set; } = null!;


    }
}
