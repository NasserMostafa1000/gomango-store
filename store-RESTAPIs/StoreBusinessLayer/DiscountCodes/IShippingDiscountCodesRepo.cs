using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreServices.DiscountCodes
{
    public interface IShippingDiscountCodesRepo
    {
        Task<bool> IsValidCodeAndIfValidUpdateItFromActiveToNoneAsync(string Code);
        Task<string?> GetRandomActiveDiscountCodeAsync();
    }
}
