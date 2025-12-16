using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.NotificationServices;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.ILoginServices;

namespace StoreServices.LoginServices
{
    public class FaceBookLoginService : ILoginServices.ILoginServices
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        public FaceBookLoginService(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();  // تهيئة HttpClient داخلياً
        }
        public async Task<User> Login(string email, string token, string password)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("هناك خطأ في المصادقة مع فيسبوك يرجى المحاولة مرة أخرى");
            }

            try
            {
                // التحقق من التوكن وجلب بيانات المستخدم من فيسبوك
                var fbUser = await ValidateFacebookTokenAsync(token);

                if (fbUser == null || string.IsNullOrEmpty(fbUser.Email))
                {
                    throw new Exception("خطأ في رمز فيسبوك أو البيانات غير كاملة");
                }

                // البحث عن المستخدم في قاعدة البيانات بالبريد الإلكتروني أو معرف المصادقة
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailOrAuthId == fbUser.Email);

                if (user != null)
                {
                    return user;
                }
                else
                {
                    // إنشاء مستخدم جديد
                    var names = fbUser.Name?.Split(" ") ?? new string[0];
                    var newUser = new User
                    {
                        EmailOrAuthId = fbUser.Email,
                        AuthProvider = "Facebook",
                        FirstName = names.Length > 0 ? names[0] : "",
                        SecondName = names.Length > 1 ? names[1] : "",
                        RoleId = 3 // دور المستخدم حسب النظام
                    };

                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    var newClient = new Client
                    {
                        UserId = newUser.UserId
                    };

                    await _context.Clients.AddAsync(newClient);
                    await _context.SaveChangesAsync();

                    return newUser;
                }
            }
            catch (Exception ex)
            {
                // تمرير الخطأ مع رسالة واضحة
                throw new Exception(ex.Message);
            }
        }
        private async Task<FacebookUser> ValidateFacebookTokenAsync(string token)
        {
            try
            {
                var url = $"https://graph.facebook.com/me?fields=id,name,email&access_token={token}";
                var fbUser = await _httpClient.GetFromJsonAsync<FacebookUser>(url);
                return fbUser;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    public class FacebookUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
