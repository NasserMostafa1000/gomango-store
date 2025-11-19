using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer.Entities;
using StoreDataAccessLayer.EntitiesConfigurations;
using StoreDataLayer.Entities;

namespace StoreDataAccessLayer
{
    public  class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
   
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ShippingCoasts> ShipPrices { get; set; }
        public DbSet<PaymentsMethods> PaymentMethods { get; set; }
        public DbSet<ShippingDiscountCodes>ShippingDiscountsCodes { get; set; }
        public DbSet<SearchingLogs> SearchingLogs { get; set; }
        public DbSet<Category> Category { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AdminInfo> AdminInfo { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sizes> Sizes { get; set; }
        public DbSet<Colors> Colors { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<PendingOrder> PendingOrders { get; set; }


        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<ProductsDetails> ProductDetails { get; set; }
        public DbSet<ProductDetailImages> ProductDetailImages { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<AnnouncementBar> AnnouncementBars { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ReviewReply> ReviewReplies { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<LegalContent> LegalContents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersConfigurations).Assembly);
        }
    }

    
    

}
