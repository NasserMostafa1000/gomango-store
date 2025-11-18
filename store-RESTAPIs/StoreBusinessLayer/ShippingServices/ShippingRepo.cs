using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreServices.ShippingServices;

namespace StoreBusinessLayer.Shipping
{
    public class ShippingRepo : IShipping
    {
      private readonly   AppDbContext _Context;
        public  ShippingRepo(AppDbContext context)
        {
            _Context=context;
        }
        public async Task<List<ShippingDtos.GetShippingCostReq>> GetShippingInfo()
        {
            try
            {
                var prices = await _Context.ShipPrices
                    .Select(ShipPrice => new ShippingDtos.GetShippingCostReq
                    {
                        Governorate = ShipPrice.GovernorateName,
                        Price = ShipPrice.Price,
                        Id = ShipPrice.Id,
                    })
                    .ToListAsync(); // تحويل الاستعلام إلى قائمة

                return prices; // إرجاع البيانات
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching shipping info: {ex.Message}");
            }
        }
        public async Task<bool> UpdateShippingPrice(string governorate, decimal newPrice)
        {
            var shippingRecord = await _Context.ShipPrices
                .FirstOrDefaultAsync(s => s.GovernorateName == governorate);

            if (shippingRecord == null)
                return false;


            shippingRecord.Price = newPrice;


            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task ResetToEmiratesAsync()
        {
            // Clear existing data
            var all = await _Context.ShipPrices.ToListAsync();
            if (all.Count > 0)
            {
                _Context.ShipPrices.RemoveRange(all);
                await _Context.SaveChangesAsync();
            }

            // Seed seven emirates (AED)
            _Context.ShipPrices.AddRange(new[]
            {
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 1, GovernorateName = "أبوظبي",     Price = 20.00m },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 2, GovernorateName = "دبي",        Price = 18.00m },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 3, GovernorateName = "الشارقة",    Price = 18.00m },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 4, GovernorateName = "عجمان",      Price = 17.00m },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 5, GovernorateName = "أم القيوين", Price = 22.00m },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 6, GovernorateName = "رأس الخيمة", Price = 25.00m },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 7, GovernorateName = "الفجيرة",    Price = 24.00m },
            });

            await _Context.SaveChangesAsync();
        }


    }
}
