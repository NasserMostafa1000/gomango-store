using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreServices.DiscountCodes;

namespace StoreServices.Discounts
{
    public class shippingDiscountCodesRepo : IShippingDiscountCodesRepo
    {
        private readonly AppDbContext _context;

        public shippingDiscountCodesRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsValidCodeAndIfValidUpdateItFromActiveToNoneAsync(string Code)
        {
            var discount = await _context.ShippingDiscountsCodes
                .FirstOrDefaultAsync(d => d.promoCodeNumber == Code);

            if (discount == null)
                throw new Exception("الكود غير موجود.");

            if (!discount.IsActive)
                throw new Exception("تم استخدام هذا الكود من قبل.");

            discount.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string?> GetRandomActiveDiscountCodeAsync()
        {
            var activeCodes = await _context.ShippingDiscountsCodes
                .Where(d => d.IsActive)
                .Select(d => d.promoCodeNumber)
                .ToListAsync();

            if (activeCodes == null || !activeCodes.Any())
                return null;

            // اختيار كود عشوائي
            var random = new Random();
            var randomIndex = random.Next(activeCodes.Count);
            return activeCodes[randomIndex];
        }
    }
}
