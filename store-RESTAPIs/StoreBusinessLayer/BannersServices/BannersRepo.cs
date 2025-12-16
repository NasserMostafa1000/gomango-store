using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreBusinessLayer.Utilities;

namespace StoreServices.BannersServices
{
    public class BannersRepo : IBanners
    {
        private readonly AppDbContext _db;
        public BannersRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<BannerDto>> GetActiveAsync()
        {
            var now = System.DateTime.UtcNow;
            return await _db.Banners
                .AsNoTracking()
                .Where(b => b.IsActive && (b.StartsAt == null || b.StartsAt <= now) && (b.EndsAt == null || b.EndsAt >= now))
                .OrderBy(b => b.DisplayOrder)
                .Select(b => new BannerDto
                {
                    Id = b.Id,
                    TitleAr = b.TitleAr,
                    TitleEn = b.TitleEn,
                    SubTitleAr = b.SubTitleAr,
                    SubTitleEn = b.SubTitleEn,
                    ImageUrl = b.ImageUrl,
                    LinkUrl = b.LinkUrl,
                    IsActive = b.IsActive,
                    DisplayOrder = b.DisplayOrder,
                    StartsAt = b.StartsAt,
                    EndsAt = b.EndsAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BannerDto>> GetAllAsync()
        {
            return await _db.Banners
                .AsNoTracking()
                .OrderBy(b => b.DisplayOrder)
                .Select(b => new BannerDto
                {
                    Id = b.Id,
                    TitleAr = b.TitleAr,
                    TitleEn = b.TitleEn,
                    SubTitleAr = b.SubTitleAr,
                    SubTitleEn = b.SubTitleEn,
                    ImageUrl = b.ImageUrl,
                    LinkUrl = b.LinkUrl,
                    IsActive = b.IsActive,
                    DisplayOrder = b.DisplayOrder,
                    StartsAt = b.StartsAt,
                    EndsAt = b.EndsAt
                })
                .ToListAsync();
        }

        public async Task<BannerDto?> GetByIdAsync(int id)
        {
            return await _db.Banners.AsNoTracking().Where(b => b.Id == id).Select(b => new BannerDto
            {
                Id = b.Id,
                TitleAr = b.TitleAr,
                TitleEn = b.TitleEn,
                SubTitleAr = b.SubTitleAr,
                SubTitleEn = b.SubTitleEn,
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                IsActive = b.IsActive,
                DisplayOrder = b.DisplayOrder,
                StartsAt = b.StartsAt,
                EndsAt = b.EndsAt
            }).FirstOrDefaultAsync();
        }

        public async Task<BannerDto> CreateAsync(BannerDto dto)
        {
            var entity = new Banner
            {
                TitleAr = dto.TitleAr,
                TitleEn = dto.TitleEn,
                SubTitleAr = dto.SubTitleAr,
                SubTitleEn = dto.SubTitleEn,
                ImageUrl = dto.ImageUrl,
                LinkUrl = dto.LinkUrl,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder,
                StartsAt = dto.StartsAt,
                EndsAt = dto.EndsAt
            };
            _db.Banners.Add(entity);
            await _db.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(BannerDto dto)
        {
            var entity = await _db.Banners.FirstOrDefaultAsync(b => b.Id == dto.Id);
            if (entity == null) return false;
            entity.TitleAr = dto.TitleAr;
            entity.TitleEn = dto.TitleEn;
            entity.SubTitleAr = dto.SubTitleAr;
            entity.SubTitleEn = dto.SubTitleEn;
            var previousImage = entity.ImageUrl;
            entity.ImageUrl = dto.ImageUrl;
            entity.LinkUrl = dto.LinkUrl;
            entity.IsActive = dto.IsActive;
            entity.DisplayOrder = dto.DisplayOrder;
            entity.StartsAt = dto.StartsAt;
            entity.EndsAt = dto.EndsAt;
            var saved = await _db.SaveChangesAsync() > 0;
            if (saved && !string.Equals(previousImage, dto.ImageUrl, StringComparison.OrdinalIgnoreCase))
            {
                ImageStorageHelper.TryDelete(previousImage);
            }
            return saved;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Banners.FirstOrDefaultAsync(b => b.Id == id);
            if (entity == null) return false;
            var previousImage = entity.ImageUrl;
            _db.Banners.Remove(entity);
            var deleted = await _db.SaveChangesAsync() > 0;
            if (deleted)
            {
                ImageStorageHelper.TryDelete(previousImage);
            }
            return deleted;
        }
    }
}


