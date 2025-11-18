using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoreBusinessLayer.AdminInfo.AdminContactInfoDtos;

namespace StoreBusinessLayer.AdminInfo
{
    public interface IAdminContactInfo
    {
        Task<AdminContactInfoDtos.GetShipPercentageAndTransActionNumberReq> GetShippingPriceAndTransactionNum(string GovernorateName);
        Task<List<AdminContactInfoDtos.GetAdminInfoReq>> GetAdminInfoAsync();
        Task<bool> UpdateAdminInfoAsync(GetAdminInfoReq updateInfo);
    }
}
