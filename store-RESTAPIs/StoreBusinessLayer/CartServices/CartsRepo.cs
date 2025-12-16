using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.Carts;
using StoreBusinessLayer.Products;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.Products.ProductInterfaces;
using static StoreBusinessLayer.Carts.CartDtos;

namespace StoreServices.CartServices
{

    public  class CartsRepo:ICart
    {
        private readonly IProductColor _colors;
        private readonly IProductSize _Sizes;
        AppDbContext _context;
        public CartsRepo(AppDbContext context,IProductColor colors,IProductSize Sizes)
        {
            _context = context;
            _colors = colors;
            _Sizes = Sizes;
        }
        public async Task<int> AddCartDetailsToSpecificClient(CartDtos.AddCartDetailsReq req, int ClientId)
        {
            try
            {
                // البحث عن السلة الخاصة بالعميل
                Cart? cart = await _context.Carts.FirstOrDefaultAsync(c => c.ClientId == ClientId);

                // إذا لم يكن لديه سلة، يتم إنشاء واحدة جديدة
                if (cart == null)
                {
                    cart = new Cart { ClientId = ClientId };
                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync(); // حفظ التغييرات للحصول على `CartId`
                }

                // إنشاء تفاصيل السلة وإضافتها
                CartDetails details = new CartDetails
                {
                    CartId = cart.CartId, // ✅ استخدام `cart.CartId` بعد التأكد من عدم كونه `null`
                    ProductDetailsId = req.ProductDetailsId,
                    Quantity = req.Quantity
                };
                await _context.CartDetails.AddAsync(details);
                await _context.SaveChangesAsync();

                return details.CartId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Cart?> GetClientCart(int clientId)
        {
            var Cart = await _context.Carts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cart => cart.ClientId == clientId);
            return Cart;
        }
        public async Task<List<CartDtos.GetCartReq>> GetCartDetailsByClientId(int clientId)
        {
            if (clientId <= 0)
            {
                throw new ArgumentException("Id must be greater than 0", nameof(clientId));
            }

            try
            {
                Cart clientCart =await GetClientCart(clientId);

                if (clientCart == null)
                {
                    return new List<CartDtos.GetCartReq>();
                }

                var cartDetails = await _context.CartDetails
                    .Where(cd => cd.CartId == clientCart.CartId)
                    .Include(cd => cd.productDetails)
                    .ThenInclude(pd => pd.Product)
                    .AsNoTracking()
                    .Select(cd => new
                    {
                        cd.CartId,
                        cd.CartDetailsId,
                        cd.Quantity,
                        cd.ProductDetailsId,
                        cd.productDetails.ColorId,
                        cd.productDetails.SizeId,
                        cd.productDetails.Product.ProductPrice,
                        cd.productDetails.Product.DiscountPercentage,
                        cd.productDetails.Product.ProductNameAr,
                        cd.productDetails.Product.ProductNameEn,
                        cd.productDetails.Product.ProductId,
                        cd.productDetails.ProductImage
                    })
                    .ToListAsync();

                if (!cartDetails.Any())
                {
                    return new List<CartDtos.GetCartReq>();
                }

                var colorIds = cartDetails.Select(cd => cd.ColorId).Distinct().ToList();
                var sizeIds = cartDetails
                    .Where(cd => cd.SizeId.HasValue)
                    .Select(cd => cd.SizeId!.Value)
                    .Distinct()
                    .ToList();

                var colorNames = await _colors.GetColorsByIdsAsync(colorIds);
                var sizeNames = await _Sizes.GetSizesByIdsAsync(sizeIds);

                return cartDetails.Select(cd =>
                {
                    decimal unitPriceBeforeDiscount = cd.ProductPrice;
                    decimal discountAmount = (cd.ProductPrice * cd.DiscountPercentage) / 100m;
                    decimal unitPriceAfterDiscount = unitPriceBeforeDiscount - discountAmount;
                    decimal totalAmount = cd.Quantity * unitPriceAfterDiscount;

                    return new CartDtos.GetCartReq
                    {
                        DiscountPercentage = cd.DiscountPercentage,
                        UnitPriceBeforeDiscount = unitPriceBeforeDiscount,
                        UnitPriceAfterDiscount = unitPriceAfterDiscount,
                        TotalPrice = totalAmount,
                        CartId = cd.CartId,
                        CartDetailsId = cd.CartDetailsId,
                        Color = colorNames.TryGetValue(cd.ColorId, out var colorName) ? colorName : "غير معروف",
                        Size = cd.SizeId.HasValue && sizeNames.TryGetValue(cd.SizeId.Value, out var sizeName) ? sizeName : null,
                        Quantity = cd.Quantity,
                        ProductName = cd.ProductNameAr,
                        ProductNameAr = cd.ProductNameAr,
                        ProductNameEn = cd.ProductNameEn,
                        ProductId = cd.ProductId,
                        Image = cd.ProductImage,
                        ProductDetailsId=cd.ProductDetailsId
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<bool> RemoveItemOnCartByCartDetailsId(int CartDetailsId)
        {
            try
            {        
                    var productDetails = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.CartDetailsId == CartDetailsId);

                if (productDetails == null)
                {
                    return false;
                }
                
                _context.CartDetails.Remove(productDetails);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing product details: " + ex.Message);
            }
        }
        public async Task<bool> RemoveCartDetailsByCartId(int cartId)
        {
            try
            {
                var Cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId);
                // البحث عن تفاصيل السلة المرتبطة بـ cartId
                var cartDetails = await _context.CartDetails
                    .Where(cd => cd.CartId == cartId)
                    .ToListAsync();

                // إذا لم توجد تفاصيل السلة، نعيد false
                if (cartDetails.Count == 0)
                {
                    return false;
                }

                _context.CartDetails.RemoveRange(cartDetails);
                await _context.SaveChangesAsync();
                _context.Carts.Remove(Cart!);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // في حال حدوث خطأ، يمكن إضافة معالجة للخطأ هنا
                throw new Exception("Error removing cart details: " + ex.Message);
            }
        }
        public async Task<bool>RemoveCartDetailsByClientId(int ClientId)
        {
            var ClientCart = await this.GetClientCart(ClientId);
            if (ClientCart == null)
            {
                return false;
            }
            bool result = await RemoveCartDetailsByCartId(ClientCart.CartId);
            return result;
        }

        // Guest Cart Methods
        public async Task<int> AddCartDetailsToGuestCart(CartDtos.AddCartDetailsReq req, string sessionId)
        {
            try
            {
                // البحث عن السلة المؤقتة الخاصة بالجلسة
                Cart? cart = await _context.Carts.FirstOrDefaultAsync(c => c.SessionId == sessionId);

                // إذا لم يكن لديه سلة، يتم إنشاء واحدة جديدة
                if (cart == null)
                {
                    cart = new Cart { SessionId = sessionId, ClientId = null };
                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }

                // إنشاء تفاصيل السلة وإضافتها
                CartDetails details = new CartDetails
                {
                    CartId = cart.CartId,
                    ProductDetailsId = req.ProductDetailsId,
                    Quantity = req.Quantity
                };
                await _context.CartDetails.AddAsync(details);
                await _context.SaveChangesAsync();

                return details.CartId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cart?> GetGuestCart(string sessionId)
        {
            var Cart = await _context.Carts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cart => cart.SessionId == sessionId);
            return Cart;
        }

        public async Task<List<CartDtos.GetCartReq>> GetCartDetailsBySessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("SessionId cannot be null or empty", nameof(sessionId));
            }

            try
            {
                Cart? guestCart = await GetGuestCart(sessionId);

                if (guestCart == null)
                {
                    return new List<CartDtos.GetCartReq>();
                }

                var cartDetails = await _context.CartDetails
                    .Where(cd => cd.CartId == guestCart.CartId)
                    .Include(cd => cd.productDetails)
                    .ThenInclude(pd => pd.Product)
                    .AsNoTracking()
                    .Select(cd => new
                    {
                        cd.CartId,
                        cd.CartDetailsId,
                        cd.Quantity,
                        cd.ProductDetailsId,
                        cd.productDetails.ColorId,
                        cd.productDetails.SizeId,
                        cd.productDetails.Product.ProductPrice,
                        cd.productDetails.Product.DiscountPercentage,
                        cd.productDetails.Product.ProductNameAr,
                        cd.productDetails.Product.ProductNameEn,
                        cd.productDetails.Product.ProductId,
                        cd.productDetails.ProductImage
                    })
                    .ToListAsync();

                if (!cartDetails.Any())
                {
                    return new List<CartDtos.GetCartReq>();
                }

                var colorIds = cartDetails.Select(cd => cd.ColorId).Distinct().ToList();
                var sizeIds = cartDetails
                    .Where(cd => cd.SizeId.HasValue)
                    .Select(cd => cd.SizeId!.Value)
                    .Distinct()
                    .ToList();

                var colorNames = await _colors.GetColorsByIdsAsync(colorIds);
                var sizeNames = await _Sizes.GetSizesByIdsAsync(sizeIds);

                return cartDetails.Select(cd =>
                {
                    decimal unitPriceBeforeDiscount = cd.ProductPrice;
                    decimal discountAmount = (cd.ProductPrice * cd.DiscountPercentage) / 100m;
                    decimal unitPriceAfterDiscount = unitPriceBeforeDiscount - discountAmount;
                    decimal totalAmount = cd.Quantity * unitPriceAfterDiscount;

                    return new CartDtos.GetCartReq
                    {
                        DiscountPercentage = cd.DiscountPercentage,
                        UnitPriceBeforeDiscount = unitPriceBeforeDiscount,
                        UnitPriceAfterDiscount = unitPriceAfterDiscount,
                        TotalPrice = totalAmount,
                        CartId = cd.CartId,
                        CartDetailsId = cd.CartDetailsId,
                        Color = colorNames.TryGetValue(cd.ColorId, out var colorName) ? colorName : "غير معروف",
                        Size = cd.SizeId.HasValue && sizeNames.TryGetValue(cd.SizeId.Value, out var sizeName) ? sizeName : null,
                        Quantity = cd.Quantity,
                        ProductName = cd.ProductNameAr,
                        ProductNameAr = cd.ProductNameAr,
                        ProductNameEn = cd.ProductNameEn,
                        ProductId = cd.ProductId,
                        Image = cd.ProductImage,
                        ProductDetailsId = cd.ProductDetailsId
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public async Task<bool> MergeGuestCartToClientCart(string sessionId, int clientId)
        {
            try
            {
                // الحصول على السلة المؤقتة
                var guestCart = await GetGuestCart(sessionId);
                if (guestCart == null)
                {
                    return false; // لا توجد سلة مؤقتة للدمج
                }

                // الحصول على سلة العميل
                var clientCart = await GetClientCart(clientId);

                // إذا لم يكن لدى العميل سلة، أنشئ واحدة جديدة
                if (clientCart == null)
                {
                    clientCart = new Cart { ClientId = clientId };
                    await _context.Carts.AddAsync(clientCart);
                    await _context.SaveChangesAsync();
                }

                // الحصول على تفاصيل السلة المؤقتة
                var guestCartDetails = await _context.CartDetails
                    .Where(cd => cd.CartId == guestCart.CartId)
                    .ToListAsync();

                if (guestCartDetails.Any())
                {
                    // دمج تفاصيل السلة المؤقتة مع سلة العميل
                    foreach (var guestDetail in guestCartDetails)
                    {
                        // التحقق من وجود نفس المنتج في سلة العميل
                        var existingDetail = await _context.CartDetails
                            .FirstOrDefaultAsync(cd => cd.CartId == clientCart.CartId && 
                                                     cd.ProductDetailsId == guestDetail.ProductDetailsId);

                        if (existingDetail != null)
                        {
                            // إذا كان المنتج موجوداً، قم بزيادة الكمية
                            existingDetail.Quantity += guestDetail.Quantity;
                            _context.CartDetails.Update(existingDetail);
                            // حذف التفصيل من السلة المؤقتة
                            _context.CartDetails.Remove(guestDetail);
                        }
                        else
                        {
                            // إذا لم يكن موجوداً، انقله إلى سلة العميل
                            guestDetail.CartId = clientCart.CartId;
                            _context.CartDetails.Update(guestDetail);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                // حذف السلة المؤقتة
                _context.Carts.Remove(guestCart);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error merging guest cart: " + ex.Message);
            }
        }
    }
}
