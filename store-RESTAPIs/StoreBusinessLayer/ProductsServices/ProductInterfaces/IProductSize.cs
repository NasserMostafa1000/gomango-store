using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreServices.Products.ProductInterfaces
{
    public interface IProductSize
    {
        Task<Dictionary<byte, string>> GetSizesByIdsAsync(List<byte> sizeIds);
        Task<string> GetSizeNameByIdAsync(int? SizeId);
        Task<byte?> GetSizeIdByNameAsync(string SizeName);
    }
}
