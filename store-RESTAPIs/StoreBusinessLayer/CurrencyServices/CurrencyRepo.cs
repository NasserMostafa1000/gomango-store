using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreServices.CurrencyServices
{
    public class CurrencyRepo : ICurrency
    {
        private readonly AppDbContext _db;
        public CurrencyRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CurrencyRateDto>> GetAllAsync()
        {
            return await _db.CurrencyRates.AsNoTracking().Where(c => c.IsActive)
                .Select(c => new CurrencyRateDto
                {
                    Id = c.Id,
                    CurrencyCode = c.CurrencyCode,
                    RateToAED = c.RateToAED,
                    UpdatedAt = c.UpdatedAt,
                    IsActive = c.IsActive
                }).ToListAsync();
        }

        public async Task<CurrencyRateDto?> GetByCodeAsync(string code)
        {
            code = code.ToUpper();
            return await _db.CurrencyRates.AsNoTracking()
                .Where(c => c.CurrencyCode == code && c.IsActive)
                .Select(c => new CurrencyRateDto
                {
                    Id = c.Id,
                    CurrencyCode = c.CurrencyCode,
                    RateToAED = c.RateToAED,
                    UpdatedAt = c.UpdatedAt,
                    IsActive = c.IsActive
                }).FirstOrDefaultAsync();
        }

        public async Task<CurrencyRateDto> UpsertAsync(CurrencyRateDto dto)
        {
            var code = dto.CurrencyCode.ToUpper();
            var entity = await _db.CurrencyRates.FirstOrDefaultAsync(c => c.CurrencyCode == code);
            if (entity == null)
            {
                entity = new CurrencyRate
                {
                    CurrencyCode = code,
                    RateToAED = dto.RateToAED,
                    IsActive = dto.IsActive
                };
                _db.CurrencyRates.Add(entity);
            }
            else
            {
                entity.RateToAED = dto.RateToAED;
                entity.IsActive = dto.IsActive;
                entity.UpdatedAt = System.DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();
            dto.Id = entity.Id;
            dto.CurrencyCode = entity.CurrencyCode;
            dto.UpdatedAt = entity.UpdatedAt;
            return dto;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await _db.CurrencyRates.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) return false;
            entity.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}


