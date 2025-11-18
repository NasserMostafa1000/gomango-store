using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Shipping;

namespace StoreServices.ShippingServices
{
    public interface IShipping
    {
        Task<List<ShippingDtos.GetShippingCostReq>> GetShippingInfo();
        Task<bool> UpdateShippingPrice(string governorate, decimal newPrice);
        Task ResetToEmiratesAsync();
    }
}
