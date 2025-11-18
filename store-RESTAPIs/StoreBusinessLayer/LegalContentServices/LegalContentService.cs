using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using System;
using System.Threading.Tasks;

namespace StoreBusinessLayer.LegalContentServices
{
    public class LegalContentService : ILegalContentService
    {
        private readonly AppDbContext _context;

        public LegalContentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LegalContentDto> GetAsync()
        {
            var entity = await _context.LegalContents.AsNoTracking().FirstOrDefaultAsync();
            if (entity == null)
            {
                return new LegalContentDto();
            }

            return new LegalContentDto
            {
                TermsAr = entity.TermsAr,
                TermsEn = entity.TermsEn,
                PrivacyAr = entity.PrivacyAr,
                PrivacyEn = entity.PrivacyEn
            };
        }

        public async Task<bool> UpsertAsync(LegalContentDto dto)
        {
            var entity = await _context.LegalContents.FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = new LegalContent
                {
                    TermsAr = dto.TermsAr,
                    TermsEn = dto.TermsEn,
                    PrivacyAr = dto.PrivacyAr,
                    PrivacyEn = dto.PrivacyEn,
                    LastUpdated = DateTime.UtcNow
                };
                _context.LegalContents.Add(entity);
            }
            else
            {
                entity.TermsAr = dto.TermsAr;
                entity.TermsEn = dto.TermsEn;
                entity.PrivacyAr = dto.PrivacyAr;
                entity.PrivacyEn = dto.PrivacyEn;
                entity.LastUpdated = DateTime.UtcNow;
                _context.LegalContents.Update(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}

