using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreServices.Products.ProductInterfaces;

namespace StoreBusinessLayer.Products
{

    public class SizesRepo:IProductSize
    {

        AppDbContext _Context;
        public SizesRepo(AppDbContext context)
        {
            _Context = context;
        }

        public async Task<Dictionary<byte, string>> GetSizesByIdsAsync(List<byte> sizeIds)
        {
            return await _Context.Sizes
                .Where(size => sizeIds.Contains(size.SizeId))
                .ToDictionaryAsync(size => size.SizeId, size => size.SizeName);
        }

        public async Task<string> GetSizeNameByIdAsync(int? SizeId)
        {
            var Size = await _Context.Sizes.FirstOrDefaultAsync(Size => Size.SizeId == SizeId);

            // تحقق إذا كان الحجم غير موجود
            if (Size == null)
            {
                return null! ;
            }

            return Size.SizeName;
        }
        public async Task<byte?> GetSizeIdByNameAsync(string SizeName)
        {
            var Size = await _Context.Sizes.FirstOrDefaultAsync(Size => Size.SizeName == SizeName);

            if (Size == null)
            {
                return null; 
            }

            return Size.SizeId;
        }

    }
}
