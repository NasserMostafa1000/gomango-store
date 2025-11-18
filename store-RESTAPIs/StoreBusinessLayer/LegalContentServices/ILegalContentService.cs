using System.Threading.Tasks;

namespace StoreBusinessLayer.LegalContentServices
{
    public interface ILegalContentService
    {
        Task<LegalContentDto> GetAsync();
        Task<bool> UpsertAsync(LegalContentDto dto);
    }
}

