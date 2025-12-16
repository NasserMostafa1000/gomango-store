using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
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
                // Use raw SQL query to handle NVARCHAR to INT conversion at database level
                var connection = _Context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        Id,
                        GovernorateName,
                        Price,
                        CASE 
                            WHEN ISNUMERIC(DeliveryTimeDays) = 1 AND CAST(DeliveryTimeDays AS FLOAT) > 0 
                            THEN CAST(DeliveryTimeDays AS INT)
                            ELSE 3
                        END AS DeliveryTimeDays
                    FROM ShipPrices";
                
                var prices = new List<ShippingDtos.GetShippingCostReq>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    prices.Add(new ShippingDtos.GetShippingCostReq
                    {
                        Id = reader.GetByte(0),
                        Governorate = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        DeliveryTimeDays = reader.GetInt32(3)
                    });
                }
                
                await connection.CloseAsync();
                return prices;
            }
            catch (Exception ex)
            {
                // Fallback: Try Entity Framework with manual conversion
                try
                {
                    var shipPrices = await _Context.ShipPrices
                        .Select(s => new { s.Id, s.GovernorateName, s.Price })
                        .ToListAsync();
                    
                    // Get DeliveryTimeDays separately using raw SQL
                    var deliveryTimes = new Dictionary<byte, int>();
                    var connection = _Context.Database.GetDbConnection();
                    await connection.OpenAsync();
                    
                    using var command = connection.CreateCommand();
                    command.CommandText = "SELECT Id, DeliveryTimeDays FROM ShipPrices";
                    using var reader = await command.ExecuteReaderAsync();
                    
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetByte(0);
                        var deliveryValue = reader.GetValue(1);
                        int deliveryDays = 3;
                        
                        if (deliveryValue != null && deliveryValue != DBNull.Value)
                        {
                            if (int.TryParse(deliveryValue.ToString(), out int parsedValue) && parsedValue > 0)
                            {
                                deliveryDays = parsedValue;
                            }
                        }
                        deliveryTimes[id] = deliveryDays;
                    }
                    
                    await connection.CloseAsync();
                    
                    var prices = shipPrices.Select(s => new ShippingDtos.GetShippingCostReq
                    {
                        Id = s.Id,
                        Governorate = s.GovernorateName,
                        Price = s.Price,
                        DeliveryTimeDays = deliveryTimes.ContainsKey(s.Id) ? deliveryTimes[s.Id] : 3
                    }).ToList();
                    
                    return prices;
                }
                catch (Exception innerEx)
                {
                    throw new Exception($"Error fetching shipping info: {ex.Message}. Inner: {innerEx.Message}");
                }
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

        public async Task<bool> UpdateDeliveryTime(string governorate, int deliveryTimeDays)
        {
            var shippingRecord = await _Context.ShipPrices
                .FirstOrDefaultAsync(s => s.GovernorateName == governorate);

            if (shippingRecord == null)
                return false;

            shippingRecord.DeliveryTimeDays = deliveryTimeDays;
            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddShippingArea(string governorate, decimal price, int deliveryTimeDays)
        {
            // Check if area already exists
            var existing = await _Context.ShipPrices
                .FirstOrDefaultAsync(s => s.GovernorateName == governorate);

            if (existing != null)
                return false; // Area already exists

            // Get the next available ID
            var maxId = await _Context.ShipPrices.MaxAsync(s => (byte?)s.Id) ?? 0;
            byte newId = (byte)(maxId + 1);

            var newArea = new StoreDataAccessLayer.Entities.ShippingCoasts
            {
                Id = newId,
                GovernorateName = governorate,
                Price = price,
                DeliveryTimeDays = deliveryTimeDays
            };

            _Context.ShipPrices.Add(newArea);
            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShippingArea(string governorate)
        {
            var shippingRecord = await _Context.ShipPrices
                .FirstOrDefaultAsync(s => s.GovernorateName == governorate);

            if (shippingRecord == null)
                return false;

            _Context.ShipPrices.Remove(shippingRecord);
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
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 1, GovernorateName = "أبوظبي",     Price = 20.00m, DeliveryTimeDays = 3 },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 2, GovernorateName = "دبي",        Price = 18.00m, DeliveryTimeDays = 2 },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 3, GovernorateName = "الشارقة",    Price = 18.00m, DeliveryTimeDays = 3 },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 4, GovernorateName = "عجمان",      Price = 17.00m, DeliveryTimeDays = 3 },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 5, GovernorateName = "أم القيوين", Price = 22.00m, DeliveryTimeDays = 4 },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 6, GovernorateName = "رأس الخيمة", Price = 25.00m, DeliveryTimeDays = 4 },
                new StoreDataAccessLayer.Entities.ShippingCoasts { Id = 7, GovernorateName = "الفجيرة",    Price = 24.00m, DeliveryTimeDays = 4 },
            });

            await _Context.SaveChangesAsync();
        }


    }
}
