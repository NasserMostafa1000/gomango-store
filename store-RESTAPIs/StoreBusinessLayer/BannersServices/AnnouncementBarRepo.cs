using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreServices.BannersServices
{
    public class AnnouncementBarRepo : IAnnouncementBar
    {
        private readonly AppDbContext _db;
        public AnnouncementBarRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AnnouncementBarDto?> GetActiveAsync()
        {
            return await _db.AnnouncementBars
                .AsNoTracking()
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementBarDto
                {
                    Id = a.Id,
                    TextAr = a.TextAr,
                    TextEn = a.TextEn,
                    LinkUrl = a.LinkUrl,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AnnouncementBarDto?> GetByIdAsync(int id)
        {
            return await _db.AnnouncementBars.AsNoTracking().Where(a => a.Id == id).Select(a => new AnnouncementBarDto
            {
                Id = a.Id,
                TextAr = a.TextAr,
                TextEn = a.TextEn,
                LinkUrl = a.LinkUrl,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).FirstOrDefaultAsync();
        }

        public async Task<AnnouncementBarDto> CreateAsync(AnnouncementBarDto dto)
        {
            var entity = new AnnouncementBar
            {
                TextAr = dto.TextAr,
                TextEn = dto.TextEn,
                LinkUrl = dto.LinkUrl,
                IsActive = dto.IsActive
            };
            _db.AnnouncementBars.Add(entity);
            await _db.SaveChangesAsync();
            dto.Id = entity.Id;
            dto.CreatedAt = entity.CreatedAt;
            return dto;
        }

        public async Task<bool> UpdateAsync(AnnouncementBarDto dto)
        {
            var entity = await _db.AnnouncementBars.FirstOrDefaultAsync(a => a.Id == dto.Id);
            if (entity == null) return false;
            entity.TextAr = dto.TextAr;
            entity.TextEn = dto.TextEn;
            entity.LinkUrl = dto.LinkUrl;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = System.DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.AnnouncementBars.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return false;
            _db.AnnouncementBars.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

