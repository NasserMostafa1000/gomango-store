using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreServices.BannersServices
{
    public interface IAnnouncementBar
    {
        Task<AnnouncementBarDto?> GetActiveAsync();
        Task<AnnouncementBarDto?> GetByIdAsync(int id);
        Task<AnnouncementBarDto> CreateAsync(AnnouncementBarDto dto);
        Task<bool> UpdateAsync(AnnouncementBarDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

