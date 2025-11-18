namespace StoreDataAccessLayer.Entities
{
    public class Colors
    {
        public byte ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public ICollection<ProductsDetails>? ProductDetails { get; set; }
    }
}
