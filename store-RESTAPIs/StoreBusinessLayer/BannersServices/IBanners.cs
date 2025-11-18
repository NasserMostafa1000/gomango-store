using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreServices.BannersServices
{
    public interface IBanners
    {
        Task<IEnumerable<BannerDto>> GetActiveAsync();
        Task<IEnumerable<BannerDto>> GetAllAsync();
        Task<BannerDto?> GetByIdAsync(int id);
        Task<BannerDto> CreateAsync(BannerDto dto);
        Task<bool> UpdateAsync(BannerDto dto);
        Task<bool> DeleteAsync(int id);
    }
}


