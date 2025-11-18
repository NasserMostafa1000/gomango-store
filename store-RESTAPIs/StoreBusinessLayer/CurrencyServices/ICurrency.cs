using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreServices.CurrencyServices
{
    public interface ICurrency
    {
        Task<IEnumerable<CurrencyRateDto>> GetAllAsync();
        Task<CurrencyRateDto?> GetByCodeAsync(string code);
        Task<CurrencyRateDto> UpsertAsync(CurrencyRateDto dto);
        Task<bool> DeactivateAsync(int id);
    }
}


