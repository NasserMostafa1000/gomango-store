using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminInfo",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    WhatsAppNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    AboutUsAr = table.Column<string>(type: "NVARCHAR(4000)", maxLength: 4000, nullable: true),
                    AboutUsEn = table.Column<string>(type: "NVARCHAR(4000)", maxLength: 4000, nullable: true),
                    FacebookUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    InstagramUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    TikTokUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementBars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementBars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubTitleAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SubTitleEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    CategoryNameEn = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: true),
                    ImagePath = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RateToAED = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    OrderStatusId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    MethodId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Method = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.MethodId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "VARCHAR(55)", maxLength: 55, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ShippingDiscountCodes",
                columns: table => new
                {
                    promoCodeNumber = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingDiscountCodes", x => x.promoCodeNumber);
                });

            migrationBuilder.CreateTable(
                name: "ShipPrices",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "TINYINT", nullable: false),
                    GovernorateName = table.Column<string>(type: "NVARCHAR(55)", maxLength: 55, nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    SizeId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeName = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    SizeCategory = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.SizeId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductNameAr = table.Column<string>(type: "NVARCHAR(250)", maxLength: 250, nullable: false),
                    ProductNameEn = table.Column<string>(type: "NVARCHAR(250)", maxLength: 250, nullable: false),
                    ShortNameAr = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    ShortNameEn = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    ProductPrice = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    CategoryId = table.Column<byte>(type: "TINYINT", nullable: false),
                    MoreDetailsAr = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    MoreDetailsEn = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    IsFeatured = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    RoleId = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Companies_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailOrAuthId = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false),
                    AuthProvider = table.Column<string>(type: "VARCHAR(85)", maxLength: 85, nullable: true),
                    RoleId = table.Column<byte>(type: "TINYINT", nullable: false),
                    PasswordHash = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Salt = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    FirstName = table.Column<string>(type: "NVARCHAR(25)", maxLength: 25, nullable: false),
                    SecondName = table.Column<string>(type: "NVARCHAR(25)", maxLength: 25, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET() AT TIME ZONE 'Egypt Standard Time'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsDetails",
                columns: table => new
                {
                    ProductDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<byte>(type: "TINYINT", nullable: false),
                    SizeId = table.Column<byte>(type: "TINYINT", nullable: true),
                    Quantity = table.Column<int>(type: "INT", nullable: false),
                    ProductImage = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsDetails", x => x.ProductDetailsId);
                    table.ForeignKey(
                        name: "FK_ProductsDetails_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "ColorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsDetails_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "SizeId");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverName = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_Drivers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Carts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ClientsAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Governorate = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    St = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsAddresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_ClientsAddresses_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET() AT TIME ZONE 'Egypt Standard Time'"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    ShippingCoast = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    OrderStatusId = table.Column<byte>(type: "TINYINT", nullable: false),
                    TransactionNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(350)", maxLength: 350, nullable: false),
                    RejectionReason = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    PaymentMethodId = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "MethodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchingLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchKeyWord = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    SearchDate = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET() AT TIME ZONE 'Egypt Standard Time'"),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchingLogs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    VisitorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VisitTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.VisitorId);
                    table.ForeignKey(
                        name: "FK_Visitors_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReviewReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Reply = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewReplies_ProductReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "ProductReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewReplies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CartsDetails",
                columns: table => new
                {
                    CartDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    ProductDetailsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartsDetails", x => x.CartDetailsId);
                    table.ForeignKey(
                        name: "FK_CartsDetails_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartsDetails_ProductsDetails_ProductDetailsId",
                        column: x => x.ProductDetailsId,
                        principalTable: "ProductsDetails",
                        principalColumn: "ProductDetailsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductDetailsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "INT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailsId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderDetails_ProductsDetails_ProductDetailsId",
                        column: x => x.ProductDetailsId,
                        principalTable: "ProductsDetails",
                        principalColumn: "ProductDetailsId");
                });

            migrationBuilder.InsertData(
                table: "AdminInfo",
                columns: new[] { "Id", "AboutUsAr", "AboutUsEn", "Email", "FacebookUrl", "InstagramUrl", "PhoneNumber", "TikTokUrl", "TransactionNumber", "WhatsAppNumber" },
                values: new object[] { (byte)1, "نص تعريفي افتراضي عن المتجر يمكن تعديله من لوحة التحكم.", "Default about-us content that can be managed from the admin panel.", "info@website.com", "https://www.facebook.com/your-page", "https://www.instagram.com/your-page", "+201098765432", "https://www.tiktok.com/@your-page", "1234567890", "+201234567890" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CategoryNameEn", "ImagePath" },
                values: new object[,]
                {
                    { (byte)1, "رجالي", "Men", "/ProjectImages/ForMens.webp" },
                    { (byte)2, "نسائي", "Women", "/ProjectImages/ForWomens.png" },
                    { (byte)3, "شنط نسائية", "Women Bags", "/ProjectImages/WomenBags.jpg" },
                    { (byte)4, "إاكسسوارات", "Accessories", "/ProjectImages/Accessories.webp" }
                });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ColorId", "ColorName" },
                values: new object[,]
                {
                    { (byte)1, "أحمر" },
                    { (byte)2, "أزرق" },
                    { (byte)3, "أخضر" },
                    { (byte)4, "أصفر" },
                    { (byte)5, "أسود" },
                    { (byte)6, "أبيض" },
                    { (byte)7, "رمادي" },
                    { (byte)8, "برتقالي" },
                    { (byte)9, "بنفسجي" },
                    { (byte)10, "وردي" },
                    { (byte)11, "بني" },
                    { (byte)12, "ذهبي" },
                    { (byte)13, "فضي" },
                    { (byte)14, "تركواز" },
                    { (byte)15, "نيلي" },
                    { (byte)16, "كحلي" },
                    { (byte)17, "عنابي" },
                    { (byte)18, "بيج" },
                    { (byte)19, "خردلي" },
                    { (byte)20, "فيروزي" },
                    { (byte)21, "زهري" },
                    { (byte)22, "أرجواني" },
                    { (byte)23, "لافندر" },
                    { (byte)24, "موف" },
                    { (byte)25, "ليموني" },
                    { (byte)26, "أخضر زيتي" },
                    { (byte)27, "أخضر فاتح" },
                    { (byte)28, "أزرق سماوي" },
                    { (byte)29, "أزرق ملكي" },
                    { (byte)30, "قرمزي" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatus",
                columns: new[] { "OrderStatusId", "StatusName" },
                values: new object[,]
                {
                    { (byte)1, "قيد المعالجة" },
                    { (byte)2, "تم التأكيد" },
                    { (byte)3, "قيد الشحن" },
                    { (byte)4, "تم التوصيل" },
                    { (byte)5, "تم الإلغاء" },
                    { (byte)6, "تم الإرجاع" },
                    { (byte)7, "تم الرفض" }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "MethodId", "Method" },
                values: new object[,]
                {
                    { (byte)1, "المحفظة الإلكترونية" },
                    { (byte)2, "الدفع عند الاستلام" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { (byte)1, "Admin" },
                    { (byte)2, "Manager" },
                    { (byte)3, "User" },
                    { (byte)4, "Shipping Man" },
                    { (byte)5, "Cashier Man" },
                    { (byte)6, "Technical support" }
                });

            migrationBuilder.InsertData(
                table: "ShipPrices",
                columns: new[] { "Id", "GovernorateName", "Price" },
                values: new object[,]
                {
                    { (byte)1, "أبوظبي", 20.00m },
                    { (byte)2, "دبي", 18.00m },
                    { (byte)3, "الشارقة", 18.00m },
                    { (byte)4, "عجمان", 17.00m },
                    { (byte)5, "أم القيوين", 22.00m },
                    { (byte)6, "رأس الخيمة", 25.00m },
                    { (byte)7, "الفجيرة", 24.00m }
                });

            migrationBuilder.InsertData(
                table: "ShippingDiscountCodes",
                columns: new[] { "promoCodeNumber", "IsActive" },
                values: new object[,]
                {
                    { "PROMO0001", true },
                    { "PROMO0002", true },
                    { "PROMO0003", true },
                    { "PROMO0004", true },
                    { "PROMO0005", true },
                    { "PROMO0006", true },
                    { "PROMO0007", true },
                    { "PROMO0008", true },
                    { "PROMO0009", true },
                    { "PROMO0010", true },
                    { "PROMO0011", true },
                    { "PROMO0012", true },
                    { "PROMO0013", true },
                    { "PROMO0014", true },
                    { "PROMO0015", true },
                    { "PROMO0016", true },
                    { "PROMO0017", true },
                    { "PROMO0018", true },
                    { "PROMO0019", true },
                    { "PROMO0020", true },
                    { "PROMO0021", true },
                    { "PROMO0022", true },
                    { "PROMO0023", true },
                    { "PROMO0024", true },
                    { "PROMO0025", true },
                    { "PROMO0026", true },
                    { "PROMO0027", true },
                    { "PROMO0028", true },
                    { "PROMO0029", true },
                    { "PROMO0030", true },
                    { "PROMO0031", true },
                    { "PROMO0032", true },
                    { "PROMO0033", true },
                    { "PROMO0034", true },
                    { "PROMO0035", true },
                    { "PROMO0036", true },
                    { "PROMO0037", true },
                    { "PROMO0038", true },
                    { "PROMO0039", true },
                    { "PROMO0040", true },
                    { "PROMO0041", true },
                    { "PROMO0042", true },
                    { "PROMO0043", true },
                    { "PROMO0044", true },
                    { "PROMO0045", true },
                    { "PROMO0046", true },
                    { "PROMO0047", true },
                    { "PROMO0048", true },
                    { "PROMO0049", true },
                    { "PROMO0050", true },
                    { "PROMO0051", true },
                    { "PROMO0052", true },
                    { "PROMO0053", true },
                    { "PROMO0054", true },
                    { "PROMO0055", true },
                    { "PROMO0056", true },
                    { "PROMO0057", true },
                    { "PROMO0058", true },
                    { "PROMO0059", true },
                    { "PROMO0060", true },
                    { "PROMO0061", true },
                    { "PROMO0062", true },
                    { "PROMO0063", true },
                    { "PROMO0064", true },
                    { "PROMO0065", true },
                    { "PROMO0066", true },
                    { "PROMO0067", true },
                    { "PROMO0068", true },
                    { "PROMO0069", true },
                    { "PROMO0070", true },
                    { "PROMO0071", true },
                    { "PROMO0072", true },
                    { "PROMO0073", true },
                    { "PROMO0074", true },
                    { "PROMO0075", true },
                    { "PROMO0076", true },
                    { "PROMO0077", true },
                    { "PROMO0078", true },
                    { "PROMO0079", true },
                    { "PROMO0080", true },
                    { "PROMO0081", true },
                    { "PROMO0082", true },
                    { "PROMO0083", true },
                    { "PROMO0084", true },
                    { "PROMO0085", true },
                    { "PROMO0086", true },
                    { "PROMO0087", true },
                    { "PROMO0088", true },
                    { "PROMO0089", true },
                    { "PROMO0090", true },
                    { "PROMO0091", true },
                    { "PROMO0092", true },
                    { "PROMO0093", true },
                    { "PROMO0094", true },
                    { "PROMO0095", true },
                    { "PROMO0096", true },
                    { "PROMO0097", true },
                    { "PROMO0098", true },
                    { "PROMO0099", true },
                    { "PROMO0100", true },
                    { "PROMO0101", true },
                    { "PROMO0102", true },
                    { "PROMO0103", true },
                    { "PROMO0104", true },
                    { "PROMO0105", true },
                    { "PROMO0106", true },
                    { "PROMO0107", true },
                    { "PROMO0108", true },
                    { "PROMO0109", true },
                    { "PROMO0110", true },
                    { "PROMO0111", true },
                    { "PROMO0112", true },
                    { "PROMO0113", true },
                    { "PROMO0114", true },
                    { "PROMO0115", true },
                    { "PROMO0116", true },
                    { "PROMO0117", true },
                    { "PROMO0118", true },
                    { "PROMO0119", true },
                    { "PROMO0120", true },
                    { "PROMO0121", true },
                    { "PROMO0122", true },
                    { "PROMO0123", true },
                    { "PROMO0124", true },
                    { "PROMO0125", true },
                    { "PROMO0126", true },
                    { "PROMO0127", true },
                    { "PROMO0128", true },
                    { "PROMO0129", true },
                    { "PROMO0130", true },
                    { "PROMO0131", true },
                    { "PROMO0132", true },
                    { "PROMO0133", true },
                    { "PROMO0134", true },
                    { "PROMO0135", true },
                    { "PROMO0136", true },
                    { "PROMO0137", true },
                    { "PROMO0138", true },
                    { "PROMO0139", true },
                    { "PROMO0140", true },
                    { "PROMO0141", true },
                    { "PROMO0142", true },
                    { "PROMO0143", true },
                    { "PROMO0144", true },
                    { "PROMO0145", true },
                    { "PROMO0146", true },
                    { "PROMO0147", true },
                    { "PROMO0148", true },
                    { "PROMO0149", true },
                    { "PROMO0150", true },
                    { "PROMO0151", true },
                    { "PROMO0152", true },
                    { "PROMO0153", true },
                    { "PROMO0154", true },
                    { "PROMO0155", true },
                    { "PROMO0156", true },
                    { "PROMO0157", true },
                    { "PROMO0158", true },
                    { "PROMO0159", true },
                    { "PROMO0160", true },
                    { "PROMO0161", true },
                    { "PROMO0162", true },
                    { "PROMO0163", true },
                    { "PROMO0164", true },
                    { "PROMO0165", true },
                    { "PROMO0166", true },
                    { "PROMO0167", true },
                    { "PROMO0168", true },
                    { "PROMO0169", true },
                    { "PROMO0170", true },
                    { "PROMO0171", true },
                    { "PROMO0172", true },
                    { "PROMO0173", true },
                    { "PROMO0174", true },
                    { "PROMO0175", true },
                    { "PROMO0176", true },
                    { "PROMO0177", true },
                    { "PROMO0178", true },
                    { "PROMO0179", true },
                    { "PROMO0180", true },
                    { "PROMO0181", true },
                    { "PROMO0182", true },
                    { "PROMO0183", true },
                    { "PROMO0184", true },
                    { "PROMO0185", true },
                    { "PROMO0186", true },
                    { "PROMO0187", true },
                    { "PROMO0188", true },
                    { "PROMO0189", true },
                    { "PROMO0190", true },
                    { "PROMO0191", true },
                    { "PROMO0192", true },
                    { "PROMO0193", true },
                    { "PROMO0194", true },
                    { "PROMO0195", true },
                    { "PROMO0196", true },
                    { "PROMO0197", true },
                    { "PROMO0198", true },
                    { "PROMO0199", true },
                    { "PROMO0200", true },
                    { "PROMO0201", true },
                    { "PROMO0202", true },
                    { "PROMO0203", true },
                    { "PROMO0204", true },
                    { "PROMO0205", true },
                    { "PROMO0206", true },
                    { "PROMO0207", true },
                    { "PROMO0208", true },
                    { "PROMO0209", true },
                    { "PROMO0210", true },
                    { "PROMO0211", true },
                    { "PROMO0212", true },
                    { "PROMO0213", true },
                    { "PROMO0214", true },
                    { "PROMO0215", true },
                    { "PROMO0216", true },
                    { "PROMO0217", true },
                    { "PROMO0218", true },
                    { "PROMO0219", true },
                    { "PROMO0220", true },
                    { "PROMO0221", true },
                    { "PROMO0222", true },
                    { "PROMO0223", true },
                    { "PROMO0224", true },
                    { "PROMO0225", true },
                    { "PROMO0226", true },
                    { "PROMO0227", true },
                    { "PROMO0228", true },
                    { "PROMO0229", true },
                    { "PROMO0230", true },
                    { "PROMO0231", true },
                    { "PROMO0232", true },
                    { "PROMO0233", true },
                    { "PROMO0234", true },
                    { "PROMO0235", true },
                    { "PROMO0236", true },
                    { "PROMO0237", true },
                    { "PROMO0238", true },
                    { "PROMO0239", true },
                    { "PROMO0240", true },
                    { "PROMO0241", true },
                    { "PROMO0242", true },
                    { "PROMO0243", true },
                    { "PROMO0244", true },
                    { "PROMO0245", true },
                    { "PROMO0246", true },
                    { "PROMO0247", true },
                    { "PROMO0248", true },
                    { "PROMO0249", true },
                    { "PROMO0250", true },
                    { "PROMO0251", true },
                    { "PROMO0252", true },
                    { "PROMO0253", true },
                    { "PROMO0254", true },
                    { "PROMO0255", true },
                    { "PROMO0256", true },
                    { "PROMO0257", true },
                    { "PROMO0258", true },
                    { "PROMO0259", true },
                    { "PROMO0260", true },
                    { "PROMO0261", true },
                    { "PROMO0262", true },
                    { "PROMO0263", true },
                    { "PROMO0264", true },
                    { "PROMO0265", true },
                    { "PROMO0266", true },
                    { "PROMO0267", true },
                    { "PROMO0268", true },
                    { "PROMO0269", true },
                    { "PROMO0270", true },
                    { "PROMO0271", true },
                    { "PROMO0272", true },
                    { "PROMO0273", true },
                    { "PROMO0274", true },
                    { "PROMO0275", true },
                    { "PROMO0276", true },
                    { "PROMO0277", true },
                    { "PROMO0278", true },
                    { "PROMO0279", true },
                    { "PROMO0280", true },
                    { "PROMO0281", true },
                    { "PROMO0282", true },
                    { "PROMO0283", true },
                    { "PROMO0284", true },
                    { "PROMO0285", true },
                    { "PROMO0286", true },
                    { "PROMO0287", true },
                    { "PROMO0288", true },
                    { "PROMO0289", true },
                    { "PROMO0290", true },
                    { "PROMO0291", true },
                    { "PROMO0292", true },
                    { "PROMO0293", true },
                    { "PROMO0294", true },
                    { "PROMO0295", true },
                    { "PROMO0296", true },
                    { "PROMO0297", true },
                    { "PROMO0298", true },
                    { "PROMO0299", true },
                    { "PROMO0300", true },
                    { "PROMO0301", true },
                    { "PROMO0302", true },
                    { "PROMO0303", true },
                    { "PROMO0304", true },
                    { "PROMO0305", true },
                    { "PROMO0306", true },
                    { "PROMO0307", true },
                    { "PROMO0308", true },
                    { "PROMO0309", true },
                    { "PROMO0310", true },
                    { "PROMO0311", true },
                    { "PROMO0312", true },
                    { "PROMO0313", true },
                    { "PROMO0314", true },
                    { "PROMO0315", true },
                    { "PROMO0316", true },
                    { "PROMO0317", true },
                    { "PROMO0318", true },
                    { "PROMO0319", true },
                    { "PROMO0320", true },
                    { "PROMO0321", true },
                    { "PROMO0322", true },
                    { "PROMO0323", true },
                    { "PROMO0324", true },
                    { "PROMO0325", true },
                    { "PROMO0326", true },
                    { "PROMO0327", true },
                    { "PROMO0328", true },
                    { "PROMO0329", true },
                    { "PROMO0330", true },
                    { "PROMO0331", true },
                    { "PROMO0332", true },
                    { "PROMO0333", true },
                    { "PROMO0334", true },
                    { "PROMO0335", true },
                    { "PROMO0336", true },
                    { "PROMO0337", true },
                    { "PROMO0338", true },
                    { "PROMO0339", true },
                    { "PROMO0340", true },
                    { "PROMO0341", true },
                    { "PROMO0342", true },
                    { "PROMO0343", true },
                    { "PROMO0344", true },
                    { "PROMO0345", true },
                    { "PROMO0346", true },
                    { "PROMO0347", true },
                    { "PROMO0348", true },
                    { "PROMO0349", true },
                    { "PROMO0350", true },
                    { "PROMO0351", true },
                    { "PROMO0352", true },
                    { "PROMO0353", true },
                    { "PROMO0354", true },
                    { "PROMO0355", true },
                    { "PROMO0356", true },
                    { "PROMO0357", true },
                    { "PROMO0358", true },
                    { "PROMO0359", true },
                    { "PROMO0360", true },
                    { "PROMO0361", true },
                    { "PROMO0362", true },
                    { "PROMO0363", true },
                    { "PROMO0364", true },
                    { "PROMO0365", true },
                    { "PROMO0366", true },
                    { "PROMO0367", true },
                    { "PROMO0368", true },
                    { "PROMO0369", true },
                    { "PROMO0370", true },
                    { "PROMO0371", true },
                    { "PROMO0372", true },
                    { "PROMO0373", true },
                    { "PROMO0374", true },
                    { "PROMO0375", true },
                    { "PROMO0376", true },
                    { "PROMO0377", true },
                    { "PROMO0378", true },
                    { "PROMO0379", true },
                    { "PROMO0380", true },
                    { "PROMO0381", true },
                    { "PROMO0382", true },
                    { "PROMO0383", true },
                    { "PROMO0384", true },
                    { "PROMO0385", true },
                    { "PROMO0386", true },
                    { "PROMO0387", true },
                    { "PROMO0388", true },
                    { "PROMO0389", true },
                    { "PROMO0390", true },
                    { "PROMO0391", true },
                    { "PROMO0392", true },
                    { "PROMO0393", true },
                    { "PROMO0394", true },
                    { "PROMO0395", true },
                    { "PROMO0396", true },
                    { "PROMO0397", true },
                    { "PROMO0398", true },
                    { "PROMO0399", true },
                    { "PROMO0400", true },
                    { "PROMO0401", true },
                    { "PROMO0402", true },
                    { "PROMO0403", true },
                    { "PROMO0404", true },
                    { "PROMO0405", true },
                    { "PROMO0406", true },
                    { "PROMO0407", true },
                    { "PROMO0408", true },
                    { "PROMO0409", true },
                    { "PROMO0410", true },
                    { "PROMO0411", true },
                    { "PROMO0412", true },
                    { "PROMO0413", true },
                    { "PROMO0414", true },
                    { "PROMO0415", true },
                    { "PROMO0416", true },
                    { "PROMO0417", true },
                    { "PROMO0418", true },
                    { "PROMO0419", true },
                    { "PROMO0420", true },
                    { "PROMO0421", true },
                    { "PROMO0422", true },
                    { "PROMO0423", true },
                    { "PROMO0424", true },
                    { "PROMO0425", true },
                    { "PROMO0426", true },
                    { "PROMO0427", true },
                    { "PROMO0428", true },
                    { "PROMO0429", true },
                    { "PROMO0430", true },
                    { "PROMO0431", true },
                    { "PROMO0432", true },
                    { "PROMO0433", true },
                    { "PROMO0434", true },
                    { "PROMO0435", true },
                    { "PROMO0436", true },
                    { "PROMO0437", true },
                    { "PROMO0438", true },
                    { "PROMO0439", true },
                    { "PROMO0440", true },
                    { "PROMO0441", true },
                    { "PROMO0442", true },
                    { "PROMO0443", true },
                    { "PROMO0444", true },
                    { "PROMO0445", true },
                    { "PROMO0446", true },
                    { "PROMO0447", true },
                    { "PROMO0448", true },
                    { "PROMO0449", true },
                    { "PROMO0450", true },
                    { "PROMO0451", true },
                    { "PROMO0452", true },
                    { "PROMO0453", true },
                    { "PROMO0454", true },
                    { "PROMO0455", true },
                    { "PROMO0456", true },
                    { "PROMO0457", true },
                    { "PROMO0458", true },
                    { "PROMO0459", true },
                    { "PROMO0460", true },
                    { "PROMO0461", true },
                    { "PROMO0462", true },
                    { "PROMO0463", true },
                    { "PROMO0464", true },
                    { "PROMO0465", true },
                    { "PROMO0466", true },
                    { "PROMO0467", true },
                    { "PROMO0468", true },
                    { "PROMO0469", true },
                    { "PROMO0470", true },
                    { "PROMO0471", true },
                    { "PROMO0472", true },
                    { "PROMO0473", true },
                    { "PROMO0474", true },
                    { "PROMO0475", true },
                    { "PROMO0476", true },
                    { "PROMO0477", true },
                    { "PROMO0478", true },
                    { "PROMO0479", true },
                    { "PROMO0480", true },
                    { "PROMO0481", true },
                    { "PROMO0482", true },
                    { "PROMO0483", true },
                    { "PROMO0484", true },
                    { "PROMO0485", true },
                    { "PROMO0486", true },
                    { "PROMO0487", true },
                    { "PROMO0488", true },
                    { "PROMO0489", true },
                    { "PROMO0490", true },
                    { "PROMO0491", true },
                    { "PROMO0492", true },
                    { "PROMO0493", true },
                    { "PROMO0494", true },
                    { "PROMO0495", true },
                    { "PROMO0496", true },
                    { "PROMO0497", true },
                    { "PROMO0498", true },
                    { "PROMO0499", true },
                    { "PROMO0500", true },
                    { "PROMO0501", true },
                    { "PROMO0502", true },
                    { "PROMO0503", true },
                    { "PROMO0504", true },
                    { "PROMO0505", true },
                    { "PROMO0506", true },
                    { "PROMO0507", true },
                    { "PROMO0508", true },
                    { "PROMO0509", true },
                    { "PROMO0510", true },
                    { "PROMO0511", true },
                    { "PROMO0512", true },
                    { "PROMO0513", true },
                    { "PROMO0514", true },
                    { "PROMO0515", true },
                    { "PROMO0516", true },
                    { "PROMO0517", true },
                    { "PROMO0518", true },
                    { "PROMO0519", true },
                    { "PROMO0520", true },
                    { "PROMO0521", true },
                    { "PROMO0522", true },
                    { "PROMO0523", true },
                    { "PROMO0524", true },
                    { "PROMO0525", true },
                    { "PROMO0526", true },
                    { "PROMO0527", true },
                    { "PROMO0528", true },
                    { "PROMO0529", true },
                    { "PROMO0530", true },
                    { "PROMO0531", true },
                    { "PROMO0532", true },
                    { "PROMO0533", true },
                    { "PROMO0534", true },
                    { "PROMO0535", true },
                    { "PROMO0536", true },
                    { "PROMO0537", true },
                    { "PROMO0538", true },
                    { "PROMO0539", true },
                    { "PROMO0540", true },
                    { "PROMO0541", true },
                    { "PROMO0542", true },
                    { "PROMO0543", true },
                    { "PROMO0544", true },
                    { "PROMO0545", true },
                    { "PROMO0546", true },
                    { "PROMO0547", true },
                    { "PROMO0548", true },
                    { "PROMO0549", true },
                    { "PROMO0550", true },
                    { "PROMO0551", true },
                    { "PROMO0552", true },
                    { "PROMO0553", true },
                    { "PROMO0554", true },
                    { "PROMO0555", true },
                    { "PROMO0556", true },
                    { "PROMO0557", true },
                    { "PROMO0558", true },
                    { "PROMO0559", true },
                    { "PROMO0560", true },
                    { "PROMO0561", true },
                    { "PROMO0562", true },
                    { "PROMO0563", true },
                    { "PROMO0564", true },
                    { "PROMO0565", true },
                    { "PROMO0566", true },
                    { "PROMO0567", true },
                    { "PROMO0568", true },
                    { "PROMO0569", true },
                    { "PROMO0570", true },
                    { "PROMO0571", true },
                    { "PROMO0572", true },
                    { "PROMO0573", true },
                    { "PROMO0574", true },
                    { "PROMO0575", true },
                    { "PROMO0576", true },
                    { "PROMO0577", true },
                    { "PROMO0578", true },
                    { "PROMO0579", true },
                    { "PROMO0580", true },
                    { "PROMO0581", true },
                    { "PROMO0582", true },
                    { "PROMO0583", true },
                    { "PROMO0584", true },
                    { "PROMO0585", true },
                    { "PROMO0586", true },
                    { "PROMO0587", true },
                    { "PROMO0588", true },
                    { "PROMO0589", true },
                    { "PROMO0590", true },
                    { "PROMO0591", true },
                    { "PROMO0592", true },
                    { "PROMO0593", true },
                    { "PROMO0594", true },
                    { "PROMO0595", true },
                    { "PROMO0596", true },
                    { "PROMO0597", true },
                    { "PROMO0598", true },
                    { "PROMO0599", true },
                    { "PROMO0600", true },
                    { "PROMO0601", true },
                    { "PROMO0602", true },
                    { "PROMO0603", true },
                    { "PROMO0604", true },
                    { "PROMO0605", true },
                    { "PROMO0606", true },
                    { "PROMO0607", true },
                    { "PROMO0608", true },
                    { "PROMO0609", true },
                    { "PROMO0610", true },
                    { "PROMO0611", true },
                    { "PROMO0612", true },
                    { "PROMO0613", true },
                    { "PROMO0614", true },
                    { "PROMO0615", true },
                    { "PROMO0616", true },
                    { "PROMO0617", true },
                    { "PROMO0618", true },
                    { "PROMO0619", true },
                    { "PROMO0620", true },
                    { "PROMO0621", true },
                    { "PROMO0622", true },
                    { "PROMO0623", true },
                    { "PROMO0624", true },
                    { "PROMO0625", true },
                    { "PROMO0626", true },
                    { "PROMO0627", true },
                    { "PROMO0628", true },
                    { "PROMO0629", true },
                    { "PROMO0630", true },
                    { "PROMO0631", true },
                    { "PROMO0632", true },
                    { "PROMO0633", true },
                    { "PROMO0634", true },
                    { "PROMO0635", true },
                    { "PROMO0636", true },
                    { "PROMO0637", true },
                    { "PROMO0638", true },
                    { "PROMO0639", true },
                    { "PROMO0640", true },
                    { "PROMO0641", true },
                    { "PROMO0642", true },
                    { "PROMO0643", true },
                    { "PROMO0644", true },
                    { "PROMO0645", true },
                    { "PROMO0646", true },
                    { "PROMO0647", true },
                    { "PROMO0648", true },
                    { "PROMO0649", true },
                    { "PROMO0650", true },
                    { "PROMO0651", true },
                    { "PROMO0652", true },
                    { "PROMO0653", true },
                    { "PROMO0654", true },
                    { "PROMO0655", true },
                    { "PROMO0656", true },
                    { "PROMO0657", true },
                    { "PROMO0658", true },
                    { "PROMO0659", true },
                    { "PROMO0660", true },
                    { "PROMO0661", true },
                    { "PROMO0662", true },
                    { "PROMO0663", true },
                    { "PROMO0664", true },
                    { "PROMO0665", true },
                    { "PROMO0666", true },
                    { "PROMO0667", true },
                    { "PROMO0668", true },
                    { "PROMO0669", true },
                    { "PROMO0670", true },
                    { "PROMO0671", true },
                    { "PROMO0672", true },
                    { "PROMO0673", true },
                    { "PROMO0674", true },
                    { "PROMO0675", true },
                    { "PROMO0676", true },
                    { "PROMO0677", true },
                    { "PROMO0678", true },
                    { "PROMO0679", true },
                    { "PROMO0680", true },
                    { "PROMO0681", true },
                    { "PROMO0682", true },
                    { "PROMO0683", true },
                    { "PROMO0684", true },
                    { "PROMO0685", true },
                    { "PROMO0686", true },
                    { "PROMO0687", true },
                    { "PROMO0688", true },
                    { "PROMO0689", true },
                    { "PROMO0690", true },
                    { "PROMO0691", true },
                    { "PROMO0692", true },
                    { "PROMO0693", true },
                    { "PROMO0694", true },
                    { "PROMO0695", true },
                    { "PROMO0696", true },
                    { "PROMO0697", true },
                    { "PROMO0698", true },
                    { "PROMO0699", true },
                    { "PROMO0700", true },
                    { "PROMO0701", true },
                    { "PROMO0702", true },
                    { "PROMO0703", true },
                    { "PROMO0704", true },
                    { "PROMO0705", true },
                    { "PROMO0706", true },
                    { "PROMO0707", true },
                    { "PROMO0708", true },
                    { "PROMO0709", true },
                    { "PROMO0710", true },
                    { "PROMO0711", true },
                    { "PROMO0712", true },
                    { "PROMO0713", true },
                    { "PROMO0714", true },
                    { "PROMO0715", true },
                    { "PROMO0716", true },
                    { "PROMO0717", true },
                    { "PROMO0718", true },
                    { "PROMO0719", true },
                    { "PROMO0720", true },
                    { "PROMO0721", true },
                    { "PROMO0722", true },
                    { "PROMO0723", true },
                    { "PROMO0724", true },
                    { "PROMO0725", true },
                    { "PROMO0726", true },
                    { "PROMO0727", true },
                    { "PROMO0728", true },
                    { "PROMO0729", true },
                    { "PROMO0730", true },
                    { "PROMO0731", true },
                    { "PROMO0732", true },
                    { "PROMO0733", true },
                    { "PROMO0734", true },
                    { "PROMO0735", true },
                    { "PROMO0736", true },
                    { "PROMO0737", true },
                    { "PROMO0738", true },
                    { "PROMO0739", true },
                    { "PROMO0740", true },
                    { "PROMO0741", true },
                    { "PROMO0742", true },
                    { "PROMO0743", true },
                    { "PROMO0744", true },
                    { "PROMO0745", true },
                    { "PROMO0746", true },
                    { "PROMO0747", true },
                    { "PROMO0748", true },
                    { "PROMO0749", true },
                    { "PROMO0750", true },
                    { "PROMO0751", true },
                    { "PROMO0752", true },
                    { "PROMO0753", true },
                    { "PROMO0754", true },
                    { "PROMO0755", true },
                    { "PROMO0756", true },
                    { "PROMO0757", true },
                    { "PROMO0758", true },
                    { "PROMO0759", true },
                    { "PROMO0760", true },
                    { "PROMO0761", true },
                    { "PROMO0762", true },
                    { "PROMO0763", true },
                    { "PROMO0764", true },
                    { "PROMO0765", true },
                    { "PROMO0766", true },
                    { "PROMO0767", true },
                    { "PROMO0768", true },
                    { "PROMO0769", true },
                    { "PROMO0770", true },
                    { "PROMO0771", true },
                    { "PROMO0772", true },
                    { "PROMO0773", true },
                    { "PROMO0774", true },
                    { "PROMO0775", true },
                    { "PROMO0776", true },
                    { "PROMO0777", true },
                    { "PROMO0778", true },
                    { "PROMO0779", true },
                    { "PROMO0780", true },
                    { "PROMO0781", true },
                    { "PROMO0782", true },
                    { "PROMO0783", true },
                    { "PROMO0784", true },
                    { "PROMO0785", true },
                    { "PROMO0786", true },
                    { "PROMO0787", true },
                    { "PROMO0788", true },
                    { "PROMO0789", true },
                    { "PROMO0790", true },
                    { "PROMO0791", true },
                    { "PROMO0792", true },
                    { "PROMO0793", true },
                    { "PROMO0794", true },
                    { "PROMO0795", true },
                    { "PROMO0796", true },
                    { "PROMO0797", true },
                    { "PROMO0798", true },
                    { "PROMO0799", true },
                    { "PROMO0800", true },
                    { "PROMO0801", true },
                    { "PROMO0802", true },
                    { "PROMO0803", true },
                    { "PROMO0804", true },
                    { "PROMO0805", true },
                    { "PROMO0806", true },
                    { "PROMO0807", true },
                    { "PROMO0808", true },
                    { "PROMO0809", true },
                    { "PROMO0810", true },
                    { "PROMO0811", true },
                    { "PROMO0812", true },
                    { "PROMO0813", true },
                    { "PROMO0814", true },
                    { "PROMO0815", true },
                    { "PROMO0816", true },
                    { "PROMO0817", true },
                    { "PROMO0818", true },
                    { "PROMO0819", true },
                    { "PROMO0820", true },
                    { "PROMO0821", true },
                    { "PROMO0822", true },
                    { "PROMO0823", true },
                    { "PROMO0824", true },
                    { "PROMO0825", true },
                    { "PROMO0826", true },
                    { "PROMO0827", true },
                    { "PROMO0828", true },
                    { "PROMO0829", true },
                    { "PROMO0830", true },
                    { "PROMO0831", true },
                    { "PROMO0832", true },
                    { "PROMO0833", true },
                    { "PROMO0834", true },
                    { "PROMO0835", true },
                    { "PROMO0836", true },
                    { "PROMO0837", true },
                    { "PROMO0838", true },
                    { "PROMO0839", true },
                    { "PROMO0840", true },
                    { "PROMO0841", true },
                    { "PROMO0842", true },
                    { "PROMO0843", true },
                    { "PROMO0844", true },
                    { "PROMO0845", true },
                    { "PROMO0846", true },
                    { "PROMO0847", true },
                    { "PROMO0848", true },
                    { "PROMO0849", true },
                    { "PROMO0850", true },
                    { "PROMO0851", true },
                    { "PROMO0852", true },
                    { "PROMO0853", true },
                    { "PROMO0854", true },
                    { "PROMO0855", true },
                    { "PROMO0856", true },
                    { "PROMO0857", true },
                    { "PROMO0858", true },
                    { "PROMO0859", true },
                    { "PROMO0860", true },
                    { "PROMO0861", true },
                    { "PROMO0862", true },
                    { "PROMO0863", true },
                    { "PROMO0864", true },
                    { "PROMO0865", true },
                    { "PROMO0866", true },
                    { "PROMO0867", true },
                    { "PROMO0868", true },
                    { "PROMO0869", true },
                    { "PROMO0870", true },
                    { "PROMO0871", true },
                    { "PROMO0872", true },
                    { "PROMO0873", true },
                    { "PROMO0874", true },
                    { "PROMO0875", true },
                    { "PROMO0876", true },
                    { "PROMO0877", true },
                    { "PROMO0878", true },
                    { "PROMO0879", true },
                    { "PROMO0880", true },
                    { "PROMO0881", true },
                    { "PROMO0882", true },
                    { "PROMO0883", true },
                    { "PROMO0884", true },
                    { "PROMO0885", true },
                    { "PROMO0886", true },
                    { "PROMO0887", true },
                    { "PROMO0888", true },
                    { "PROMO0889", true },
                    { "PROMO0890", true },
                    { "PROMO0891", true },
                    { "PROMO0892", true },
                    { "PROMO0893", true },
                    { "PROMO0894", true },
                    { "PROMO0895", true },
                    { "PROMO0896", true },
                    { "PROMO0897", true },
                    { "PROMO0898", true },
                    { "PROMO0899", true },
                    { "PROMO0900", true },
                    { "PROMO0901", true },
                    { "PROMO0902", true },
                    { "PROMO0903", true },
                    { "PROMO0904", true },
                    { "PROMO0905", true },
                    { "PROMO0906", true },
                    { "PROMO0907", true },
                    { "PROMO0908", true },
                    { "PROMO0909", true },
                    { "PROMO0910", true },
                    { "PROMO0911", true },
                    { "PROMO0912", true },
                    { "PROMO0913", true },
                    { "PROMO0914", true },
                    { "PROMO0915", true },
                    { "PROMO0916", true },
                    { "PROMO0917", true },
                    { "PROMO0918", true },
                    { "PROMO0919", true },
                    { "PROMO0920", true },
                    { "PROMO0921", true },
                    { "PROMO0922", true },
                    { "PROMO0923", true },
                    { "PROMO0924", true },
                    { "PROMO0925", true },
                    { "PROMO0926", true },
                    { "PROMO0927", true },
                    { "PROMO0928", true },
                    { "PROMO0929", true },
                    { "PROMO0930", true },
                    { "PROMO0931", true },
                    { "PROMO0932", true },
                    { "PROMO0933", true },
                    { "PROMO0934", true },
                    { "PROMO0935", true },
                    { "PROMO0936", true },
                    { "PROMO0937", true },
                    { "PROMO0938", true },
                    { "PROMO0939", true },
                    { "PROMO0940", true },
                    { "PROMO0941", true },
                    { "PROMO0942", true },
                    { "PROMO0943", true },
                    { "PROMO0944", true },
                    { "PROMO0945", true },
                    { "PROMO0946", true },
                    { "PROMO0947", true },
                    { "PROMO0948", true },
                    { "PROMO0949", true },
                    { "PROMO0950", true },
                    { "PROMO0951", true },
                    { "PROMO0952", true },
                    { "PROMO0953", true },
                    { "PROMO0954", true },
                    { "PROMO0955", true },
                    { "PROMO0956", true },
                    { "PROMO0957", true },
                    { "PROMO0958", true },
                    { "PROMO0959", true },
                    { "PROMO0960", true },
                    { "PROMO0961", true },
                    { "PROMO0962", true },
                    { "PROMO0963", true },
                    { "PROMO0964", true },
                    { "PROMO0965", true },
                    { "PROMO0966", true },
                    { "PROMO0967", true },
                    { "PROMO0968", true },
                    { "PROMO0969", true },
                    { "PROMO0970", true },
                    { "PROMO0971", true },
                    { "PROMO0972", true },
                    { "PROMO0973", true },
                    { "PROMO0974", true },
                    { "PROMO0975", true },
                    { "PROMO0976", true },
                    { "PROMO0977", true },
                    { "PROMO0978", true },
                    { "PROMO0979", true },
                    { "PROMO0980", true },
                    { "PROMO0981", true },
                    { "PROMO0982", true },
                    { "PROMO0983", true },
                    { "PROMO0984", true },
                    { "PROMO0985", true },
                    { "PROMO0986", true },
                    { "PROMO0987", true },
                    { "PROMO0988", true },
                    { "PROMO0989", true },
                    { "PROMO0990", true },
                    { "PROMO0991", true },
                    { "PROMO0992", true },
                    { "PROMO0993", true },
                    { "PROMO0994", true },
                    { "PROMO0995", true },
                    { "PROMO0996", true },
                    { "PROMO0997", true },
                    { "PROMO0998", true },
                    { "PROMO0999", true },
                    { "PROMO1000", true }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "SizeId", "SizeCategory", "SizeName" },
                values: new object[,]
                {
                    { (byte)1, "ملابس", "S" },
                    { (byte)2, "ملابس", "M" },
                    { (byte)3, "ملابس", "L" },
                    { (byte)4, "ملابس", "XL" },
                    { (byte)5, "ملابس", "XXL" },
                    { (byte)6, "ملابس", "XXXL" },
                    { (byte)7, "ملابس", "XXXXL" },
                    { (byte)8, "ملابس", "XXXXXL" },
                    { (byte)9, "حماله صدر", "A" },
                    { (byte)10, "حماله صدر", "B" },
                    { (byte)11, "حماله صدر", "C" },
                    { (byte)12, "حماله صدر", "D" },
                    { (byte)13, "حماله صدر", "E" },
                    { (byte)14, "حماله صدر", "F" },
                    { (byte)15, "بناطيل", "22" },
                    { (byte)16, "بناطيل", "23" },
                    { (byte)17, "بناطيل", "24" },
                    { (byte)18, "بناطيل", "25" },
                    { (byte)19, "بناطيل", "26" },
                    { (byte)20, "بناطيل", "27" },
                    { (byte)21, "بناطيل", "28" },
                    { (byte)22, "بناطيل", "29" },
                    { (byte)23, "بناطيل", "30" },
                    { (byte)24, "بناطيل", "31" },
                    { (byte)25, "بناطيل", "32" },
                    { (byte)26, "بناطيل", "33" },
                    { (byte)27, "بناطيل", "34" },
                    { (byte)28, "بناطيل", "35" },
                    { (byte)29, "بناطيل/احذيه", "36" },
                    { (byte)30, "بناطيل/احذيه", "37" },
                    { (byte)31, "بناطيل/احذيه", "38" },
                    { (byte)32, "بناطيل/احذيه", "39" },
                    { (byte)33, "بناطيل/احذيه", "40" },
                    { (byte)34, "بناطيل/احذيه", "41" },
                    { (byte)35, "بناطيل/احذيه", "42" },
                    { (byte)36, "بناطيل/احذيه", "43" },
                    { (byte)37, "بناطيل/احذيه", "44" },
                    { (byte)38, "بناطيل/احذيه", "45" },
                    { (byte)39, "بناطيل/احذيه", "46" },
                    { (byte)40, "بناطيل/احذيه", "47" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ClientId",
                table: "Carts",
                column: "ClientId",
                unique: true,
                filter: "[ClientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartId",
                table: "CartsDetails",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartsDetails_ProductDetailsId",
                table: "CartsDetails",
                column: "ProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientsAddresses_ClientId",
                table: "ClientsAddresses",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Colors_ColorName",
                table: "Colors",
                column: "ColorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_RoleId",
                table: "Companies",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_CurrencyCode",
                table: "CurrencyRates",
                column: "CurrencyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserId",
                table: "Drivers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductDetailsId",
                table: "OrderDetails",
                column: "ProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ColorId_productId",
                table: "ProductsDetails",
                columns: new[] { "ColorId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ColorId_SizeId_ProductId",
                table: "ProductsDetails",
                columns: new[] { "ColorId", "SizeId", "ProductDetailsId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductsDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsDetails_SizeId",
                table: "ProductsDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReplies_ReviewId",
                table: "ReviewReplies",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReplies_UserId",
                table: "ReviewReplies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchingLogs_ClientId",
                table: "SearchingLogs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailOrAuthId",
                table: "Users",
                column: "EmailOrAuthId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_ClientId",
                table: "Visitors",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminInfo");

            migrationBuilder.DropTable(
                name: "AnnouncementBars");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "CartsDetails");

            migrationBuilder.DropTable(
                name: "ClientsAddresses");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ReviewReplies");

            migrationBuilder.DropTable(
                name: "SearchingLogs");

            migrationBuilder.DropTable(
                name: "ShippingDiscountCodes");

            migrationBuilder.DropTable(
                name: "ShipPrices");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductsDetails");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
