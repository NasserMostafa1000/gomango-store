using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreBusinessLayer.Visitors;
using StoreServices.VisitorsServices;

namespace StoreBusinessLayer.Visitors
{
    public class VisitorRepo : IVisitor
    {
        private readonly AppDbContext _context;
        private readonly TimeSpan _activeVisitorTimeout = TimeSpan.FromMinutes(30); // زائر نشط إذا كان آخر نشاط خلال 30 دقيقة

        public VisitorRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddVisitor(VisitorDtos.PostVisitorReq req)
        {
            try
            {
                var visitor = new Visitor
                {
                    IpAddress = req.IpAddress,
                    Country = req.Country,
                    CountryCode = req.CountryCode,
                    City = req.City,
                    Region = req.Region,
                    UserAgent = req.UserAgent,
                    Referrer = req.Referrer,
                    ClientId = req.ClientId,
                    VisitTime = DateTime.UtcNow,
                    LastActivityTime = DateTime.UtcNow,
                    IsActive = true
                };

                await _context.Visitors.AddAsync(visitor);
                await _context.SaveChangesAsync();

                return visitor.VisitorId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateVisitorActivity(int visitorId)
        {
            try
            {
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.VisitorId == visitorId);
                if (visitor != null)
                {
                    visitor.LastActivityTime = DateTime.UtcNow;
                    visitor.IsActive = true;
                    _context.Visitors.Update(visitor);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkVisitorAsInactive(int visitorId)
        {
            try
            {
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.VisitorId == visitorId);
                if (visitor != null)
                {
                    visitor.IsActive = false;
                    _context.Visitors.Update(visitor);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> MarkInactiveVisitors()
        {
            try
            {
                // تحديد الزوار غير النشطين (آخر نشاط قبل أكثر من 30 دقيقة)
                var cutoffTime = DateTime.UtcNow.Subtract(_activeVisitorTimeout);
                
                var inactiveVisitors = await _context.Visitors
                    .Where(v => v.IsActive && 
                           (v.LastActivityTime == null || v.LastActivityTime < cutoffTime))
                    .ToListAsync();

                if (inactiveVisitors.Any())
                {
                    foreach (var visitor in inactiveVisitors)
                    {
                        visitor.IsActive = false;
                    }
                    await _context.SaveChangesAsync();
                }

                return inactiveVisitors.Count;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<VisitorDtos.GetVisitorsAnalyticsReq> GetVisitorsAnalytics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Visitors.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(v => v.VisitTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(v => v.VisitTime <= endDate.Value);

            var totalVisits = await query.CountAsync();

            // الزوار النشطون حالياً (آخر نشاط خلال 30 دقيقة)
            var cutoffTime = DateTime.UtcNow.Subtract(_activeVisitorTimeout);
            var currentActiveVisitors = await _context.Visitors
                .Where(v => v.IsActive && v.LastActivityTime.HasValue && v.LastActivityTime >= cutoffTime)
                .CountAsync();

            // التوزيع الجغرافي
            var geographicDataRaw = await query
                .Where(v => !string.IsNullOrEmpty(v.Country))
                .GroupBy(v => new { v.Country, v.CountryCode, v.City })
                .Select(g => new
                {
                    Country = g.Key.Country!,
                    CountryCode = g.Key.CountryCode ?? "",
                    City = g.Key.City,
                    VisitCount = g.Count()
                })
                .OrderByDescending(x => x.VisitCount)
                .ToListAsync();

            var geographicData = geographicDataRaw.Select(g => new VisitorDtos.GeographicDistributionReq
            {
                Country = g.Country,
                CountryCode = g.CountryCode,
                City = g.City,
                VisitCount = g.VisitCount,
                CountryFlagIcon = GetCountryFlagIcon(g.CountryCode)
            }).ToList();

            // الزيارات اليومية (آخر 30 يوم)
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var dailyVisits = await _context.Visitors
                .Where(v => v.VisitTime >= thirtyDaysAgo)
                .GroupBy(v => v.VisitTime.Date)
                .Select(g => new VisitorDtos.DailyVisitsReq
                {
                    Date = g.Key,
                    VisitCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return new VisitorDtos.GetVisitorsAnalyticsReq
            {
                TotalVisits = totalVisits,
                CurrentActiveVisitors = currentActiveVisitors,
                GeographicDistribution = geographicData,
                DailyVisits = dailyVisits
            };
        }

        public async Task<List<VisitorDtos.GetCurrentVisitorsReq>> GetCurrentActiveVisitors()
        {
            // تحديث الزوار غير النشطين تلقائياً قبل جلب الزوار النشطين
            await MarkInactiveVisitors();
            
            var cutoffTime = DateTime.UtcNow.Subtract(_activeVisitorTimeout);

            var visitorsRaw = await _context.Visitors
                .Include(v => v.Client)
                    .ThenInclude(c => c.User)
                .Where(v => v.IsActive && v.LastActivityTime.HasValue && v.LastActivityTime >= cutoffTime)
                .OrderByDescending(v => v.LastActivityTime)
                .Select(v => new
                {
                    v.VisitorId,
                    v.IpAddress,
                    v.Country,
                    CountryCode = v.CountryCode ?? "",
                    v.City,
                    v.VisitTime,
                    v.LastActivityTime,
                    ClientName = v.Client != null ? $"{v.Client.User!.FirstName} {v.Client.User.SecondName}" : null
                })
                .ToListAsync();

            var visitors = visitorsRaw.Select(v => new VisitorDtos.GetCurrentVisitorsReq
            {
                VisitorId = v.VisitorId,
                IpAddress = v.IpAddress,
                Country = v.Country,
                CountryCode = v.CountryCode,
                City = v.City,
                VisitTime = v.VisitTime,
                LastActivityTime = v.LastActivityTime,
                CountryFlagIcon = GetCountryFlagIcon(v.CountryCode),
                ClientName = v.ClientName
            }).ToList();

            return visitors;
        }

        public async Task<int> GetTotalVisitsCount(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Visitors.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(v => v.VisitTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(v => v.VisitTime <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<int> GetCurrentActiveVisitorsCount()
        {
            var cutoffTime = DateTime.UtcNow.Subtract(_activeVisitorTimeout);
            return await _context.Visitors
                .Where(v => v.IsActive && v.LastActivityTime.HasValue && v.LastActivityTime >= cutoffTime)
                .CountAsync();
        }

        public async Task<VisitorDtos.GetAllVisitorsPagedReq> GetAllVisitors(int pageNumber = 1, int pageSize = 20)
        {
            // تحديث الزوار غير النشطين تلقائياً قبل جلب جميع الزوار
            await MarkInactiveVisitors();
            
            var query = _context.Visitors
                .Include(v => v.Client)
                    .ThenInclude(c => c.User)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var visitorsRaw = await query
                .OrderByDescending(v => v.VisitTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.VisitorId,
                    v.IpAddress,
                    v.Country,
                    CountryCode = v.CountryCode ?? "",
                    v.City,
                    v.VisitTime,
                    v.LastActivityTime,
                    v.IsActive,
                    ClientName = v.Client != null ? $"{v.Client.User!.FirstName} {v.Client.User.SecondName}" : null
                })
                .ToListAsync();

            var visitors = visitorsRaw.Select(v => new VisitorDtos.GetAllVisitorsReq
            {
                VisitorId = v.VisitorId,
                IpAddress = v.IpAddress,
                Country = v.Country,
                CountryCode = v.CountryCode,
                City = v.City,
                VisitTime = v.VisitTime,
                LastActivityTime = v.LastActivityTime,
                IsActive = v.IsActive,
                CountryFlagIcon = GetCountryFlagIcon(v.CountryCode),
                ClientName = v.ClientName
            }).ToList();

            return new VisitorDtos.GetAllVisitorsPagedReq
            {
                Visitors = visitors,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        // دالة مساعدة للحصول على أيقونة العلم بناءً على رمز البلد
        private static string GetCountryFlagIcon(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
                return "🌍"; // أيقونة افتراضية

            // استخدام emoji flags (Unicode flags)
            // يمكن استبدالها بـ URL لأيقونات إذا لزم الأمر
            var codePoints = countryCode.ToUpper()
                .Select(c => 0x1F1E6 + (c - 'A'))
                .ToArray();

            if (codePoints.Length == 2)
            {
                return char.ConvertFromUtf32(codePoints[0]) + char.ConvertFromUtf32(codePoints[1]);
            }

            return "🌍";
        }
    }
}

