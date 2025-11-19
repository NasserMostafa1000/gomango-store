namespace StoreDataAccessLayer.Entities
{
    public class ProductDetailImages
    {
        public int ProductDetailImageId { get; set; }
        public int ProductDetailsId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
        public ProductsDetails ProductDetails { get; set; } = null!;
    }
}

