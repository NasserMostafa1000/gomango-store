using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineStoreAPIs.Hubs;
using StoreBusinessLayer.AdminInfo;
using StoreBusinessLayer.Carts;
using StoreBusinessLayer.Clients;
using StoreBusinessLayer.Orders;
using StoreBusinessLayer.Products;
using StoreBusinessLayer.Shipping;
using StoreBusinessLayer.Users;
using StoreBusinessLayer.LegalContentServices;
using StoreDataAccessLayer;
using StoreServices.CartServices;
using StoreServices.BannersServices;
using StoreServices.ClientsServices;
using StoreServices.DiscountCodes;
using StoreServices.Discounts;
using StoreServices.LoginServices;
using StoreServices.OrdersServices;
using StoreServices.Products.ProductInterfaces;
using StoreServices.ShippingServices;
using StoreServices.UsersServices;
using StoreServices.CurrencyServices;
using StoreServices.ReviewsServices;
using StoreServices.VisitorsServices;
using StoreBusinessLayer.Visitors;
using Stripe;
using System.Text;
using OnlineStoreAPIs.Middleware;

namespace OnlineStoreAPIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                
                // إعداد Stripe - مع التحقق من وجود المفتاح (لا نرمي خطأ إذا لم يكن موجوداً)
                var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
                if (!string.IsNullOrWhiteSpace(stripeSecretKey))
                {
                    StripeConfiguration.ApiKey = stripeSecretKey;
                }
                
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                
                // إعداد قاعدة البيانات مع معالجة الأخطاء
                var connectionString = builder.Configuration.GetConnectionString("ConnStr");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    // في Production، نحاول استخدام Connection String من Environment Variables
                    connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ConnStr") 
                        ?? builder.Configuration["ConnectionStrings:ConnStr"];
                    
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new InvalidOperationException("Connection string 'ConnStr' is missing or empty. Please check appsettings.json or environment variables.");
                    }
                }
                
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

            // تسجيل خدمات الأعمال (Business Layer)
            builder.Services.AddScoped<IProductCategory,CategoriesRepo>();
            builder.Services.AddScoped<IShipping,ShippingRepo>();
            builder.Services.AddScoped<IUser,UsersRepo>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<IClient,ClientsRepo>();
            builder.Services.AddScoped<IProduct,ProductsRepo>();
            builder.Services.AddScoped<IProductColor,ColorsRepo>();
            builder.Services.AddScoped<IProductSize,SizesRepo>();
            builder.Services.AddScoped<IOrder,OrdersRepo>();
            builder.Services.AddScoped<IPendingOrderStore, PendingOrderStore>();
            builder.Services.AddScoped<ISearchLogs, SearchingLogsRepo>();
            builder.Services.AddScoped<IShippingDiscountCodesRepo, shippingDiscountCodesRepo>();
            builder.Services.AddScoped<ICart,CartsRepo>();
            builder.Services.AddScoped<IAdminContactInfo, AdminContactInfo>();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IBanners, BannersRepo>();
            builder.Services.AddScoped<IAnnouncementBar, AnnouncementBarRepo>();
            builder.Services.AddScoped<ICurrency, CurrencyRepo>();
            builder.Services.AddScoped<IReviews, ReviewsRepo>();
            builder.Services.AddScoped<IReviewReplies, ReviewRepliesRepo>();
            builder.Services.AddScoped<IVisitor, VisitorRepo>();
            builder.Services.AddScoped<ILegalContentService, LegalContentService>();

            builder.Services.AddHttpClient<FaceBookLoginService>();


            // إعداد المصادقة باستخدام JWT
            var jwtKey = builder.Configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("JwtSettings__Key");
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is missing. Please check appsettings.json or environment variables.");
            }
            
            var key = Encoding.UTF8.GetBytes(jwtKey);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JwtSettings__Issuer"),
                        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JwtSettings__Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    
                });

            // تفعيل التصريح (Authorization)
            builder.Services.AddAuthorization();

            // تفعيل CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin() 
                              .AllowAnyMethod() 
                              .AllowAnyHeader(); 
                    });
            });



            var app = builder.Build();
            
            // معالجة الأخطاء - تسجيل الأخطاء في Production
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // استخدام lambda بدلاً من path لتجنب InvalidOperationException
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\":\"An error occurred while processing your request.\"}");
                    });
                });
                app.UseHsts();
            }
            
            // إضافة Error Logging Middleware بعد Exception Handler
            app.UseErrorLogging();
            
            // ترتيب مهم: CORS أولاً
            app.UseCors("AllowAll");
            
            // ثم Routing
            app.UseRouting();
            
            // ثم Authentication و Authorization
            app.UseAuthentication();
            app.UseAuthorization();
            
            // ثم Static Files
            app.UseStaticFiles();
            
            // ثم Endpoints
            app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
                _ = endpoints.MapHub<OrderHub>("/orderHub");
            });

            // HTTPS Redirection في النهاية
            if (app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            // تفعيل Swagger في وضع التطوير
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger";
                });
            }

                app.Run();
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ في ملف log أو Event Viewer
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "startup-error.txt");
                var logDir = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                System.IO.File.WriteAllText(logPath, $"{DateTime.UtcNow}: {ex.Message}\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
                
                // في Production، لا نعرض تفاصيل الخطأ للمستخدم
                throw;
            }
        }
    }
}
