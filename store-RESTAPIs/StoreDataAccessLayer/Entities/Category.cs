namespace StoreDataAccessLayer.Entities
{
    public class Category
    {
        public byte CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryNameEn { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
