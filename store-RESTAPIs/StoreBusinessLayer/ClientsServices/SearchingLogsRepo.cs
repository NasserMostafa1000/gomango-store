using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreDataAccessLayer;  // عدّل حسب الـ namespace الصحيح عندك
using StoreDataAccessLayer.Entities;
using StoreDataLayer.Entities; // فيه تعريف الكيانات مثل SearchingLogs و Client

namespace StoreServices.ClientsServices
{
    
    public class SearchingLogsRepo : ISearchLogs
    {
        private readonly AppDbContext _context;

        public SearchingLogsRepo(AppDbContext context)
        {
            _context = context;
        }

        // إضافة سجل بحث جديد
        public async Task AddSearchAsync(string searchTerm, int clientId = 0)
        {
            var log = new SearchingLogs
            {
                SearchKeyWord = searchTerm,
                ClientId = clientId,
                SearchDate = DateTime.Now
            };

            _context.SearchingLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        // جلب الكلمات التي بحث عنها عميل محدد (بكلمات مميزة)
        public async Task<List<string>> FindById(int clientId)
        {
            try
            {
                var ClientSearhings= await _context.SearchingLogs
          .Where(log => log.ClientId == clientId)
          .Select(log => log.SearchKeyWord)
          .Distinct()
          .ToListAsync();
                return ClientSearhings.Count > 0? ClientSearhings:new List<string>();
            }
      catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString().Trim());
            }
        }

        // أكثر 10 كلمات بحثًا بشكل عام
        public async Task<Dictionary<string, int>> FindTheMostlyProductThatPeopleSearchedOn()
        {
            var grouped = await _context.SearchingLogs
                .GroupBy(log => log.SearchKeyWord)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            // الآن نحول النتيجة إلى Dictionary في الذاكرة
            return grouped.ToDictionary(g => g.Key, g => g.Count);
        }


        public class SearchLogWithClientName
        {
            public int? ClientId { get; set; }
            public string SearchKeyWord { get; set; }
            public string ClientFullName { get; set; }
            public DateTime SearchDate { get; set; }
        }

        // جلب جميع كلمات البحث المميزة
        public async Task<List<SearchLogWithClientName>> GetAll()
        {
            string FullName;
            return await _context.SearchingLogs
                .Include(log => log.Client)  // جلب بيانات العميل المرتبطة
                .Select(log => new SearchLogWithClientName
                {
                    SearchKeyWord = log.SearchKeyWord,
                    ClientId=log.ClientId,
                    SearchDate=log.SearchDate,
                    ClientFullName = (log.Client.User.FirstName + " " + log.Client.User.SecondName).IsNullOrEmpty() ? "غير مسجل لدينا" : log.Client.User.FirstName + " " + log.Client.User.SecondName
                })
                .ToListAsync();
        }

   
    }
}
