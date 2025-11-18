using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreServices.Products.ProductInterfaces
{
    public interface IProductColor
    {
        Task<Dictionary<byte, string>> GetColorsByIdsAsync(List<byte> colorIds);
        Task<string> GetColorNameByIdAsync(byte? ColorId);
        Task<byte> GetColorIdByColorNameAsync(string? ColorName);
    }
}
