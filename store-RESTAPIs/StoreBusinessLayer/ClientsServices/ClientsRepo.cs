using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer.Entities;
using StoreDataAccessLayer;
using static StoreBusinessLayer.Clients.ClientsDtos;
using StoreBusinessLayer.Orders;
using StoreServices.ClientsServices;
using StoreServices.UsersServices;

namespace StoreBusinessLayer.Clients
{
    public class ClientsRepo:IClient
    {
        public AppDbContext _DbContext;
        private readonly IUser _UsersBL;

        public ClientsRepo(AppDbContext Context, IUser Users)
        {
            _DbContext = Context;
            _UsersBL = Users;
        }
        public async Task SendNotificationToUser(int userId, string message)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && !string.IsNullOrEmpty(user.EmailOrAuthId))
            {
                // إرسال إشعار عبر البريد الإلكتروني أو أي وسيلة أخرى.
                await NotificationServices.NotificationsCreator.SendNotification(
                    "تحديث في حسابك - جومانجو", // العنوان
                    message, // الرسالة المخصصة
                    user.EmailOrAuthId, // البريد الإلكتروني أو معرّف المصادقة
                    "Gmail" // يمكنك استبدال هذه الوسيلة حسب الحاجة
                );
            }
        }
        public async Task<int> AddNewClient(ClientsDtos.PostClientReq Dto)
        {
            try
            {
                int UserId = await _UsersBL.PostUser(
                    new UsersDtos.PostUserReq
                    {
                        FirstName = Dto.FirstName,
                        SecondName = Dto.SecondName,
                        EmailOrAuthId = Dto.Email,
                        Password = Dto.Password,
                        AuthProvider = "Online Store"
                    }, 3); // user role id => 3 (client role)

                Client NewClient = new Client
                {
                    
                    PhoneNumber = Dto.PhoneNumber,
                    UserId = UserId
                };

                await _DbContext.Clients.AddAsync(NewClient);
                await _DbContext.SaveChangesAsync();

                // إرسال إشعار للعميل عند إضافة حساب جديد
                string message = $"مرحبًا {Dto.FirstName} {Dto.SecondName}،\n\n🎉 <strong>مرحبًا بك في جومانجو!</strong>\n\nتم إنشاء حسابك بنجاح. يمكنك الآن الاستمتاع بجميع خدماتنا ومميزاتنا.\n\n📧 <strong>بريدك الإلكتروني:</strong> {Dto.Email}\n\nنتمنى أن تجد تجربتك معنا ممتعة ومفيدة. إذا كان لديك أي استفسار أو تحتاج إلى مساعدة، نحن هنا لخدمتك.";

                await SendNotificationToUser(UserId, message);

                return NewClient.ClientId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<bool> UpdateClientName(string FirstName, string LastName, int ClientId)
        {
            var Client = await _DbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == ClientId);
            if (Client != null)
            {
                var User = await _DbContext.Users.FirstOrDefaultAsync(U => U.UserId == Client.UserId);
                User.FirstName = FirstName;
                User.SecondName = LastName;
                _DbContext.Users.Update(User);
                await _DbContext.SaveChangesAsync();

                // إرسال إشعار عند تحديث الاسم
                string message = $"مرحبًا {FirstName} {LastName}،\n\n✅ تم تحديث اسمك في حسابك في جومانجو بنجاح.\n\nإذا كان لديك أي استفسار أو تحتاج إلى مساعدة، نحن هنا لخدمتك.";

                await SendNotificationToUser(Client.UserId, message);

                return true;
            }
            return false;
        }
        public async Task<bool> AddOrUpdatePhoneToClientById(int ClientId, string phoneNumber)
        {
            try
            {
                var targetClient = await _DbContext.Clients
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.ClientId == ClientId);
                if (targetClient == null)
                    return false;
                targetClient.PhoneNumber = phoneNumber;
                _DbContext.Update(targetClient);
                await _DbContext.SaveChangesAsync();
                string message = $"مرحبًا {targetClient.User.FirstName} {targetClient.User.SecondName}،\n\n✅ تم تحديث رقم الهاتف الخاص بك في حسابك في جومانجو بنجاح.\n\n📱 <strong>رقم الهاتف الجديد:</strong> {phoneNumber}\n\nإذا كان لديك أي استفسار أو تحتاج إلى مساعدة، نحن هنا لخدمتك.";

                await SendNotificationToUser(targetClient.UserId, message);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<int> AddNewAddress(ClientsDtos.PostAddressReq req, int ClientId)
        {
            var Address = new Address
            {
                Governorate = req.Governorate,
                St = req.street,
                City = req.City,
                ClientId = ClientId,
            };

            await _DbContext.Addresses.AddAsync(Address);
            await _DbContext.SaveChangesAsync();
            var targetClient = await _DbContext.Clients
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.ClientId == ClientId); if (targetClient != null)
            {
                string message = $"مرحبًا {targetClient.User.FirstName} {targetClient.User.SecondName}،\n\n✅ تم إضافة عنوان جديد إلى حسابك في جومانجو بنجاح.\n\n📍 <strong>تفاصيل العنوان:</strong>\n• المحافظة: {req.Governorate}\n• المدينة: {req.City}\n• الشارع: {req.street}\n\nيمكنك تعديل أو حذف هذا العنوان في أي وقت من خلال حسابك الشخصي.";

                await SendNotificationToUser(targetClient.UserId, message);
            }

            return Address.AddressId;
        }
        public async Task<bool> UpdateClientEmail(int ClientId, string newEmail)
        {
            var client = await _DbContext.Clients.Include(c=>c.User).FirstOrDefaultAsync(c => c.ClientId == ClientId);
            if (client != null)
            {
                client.User.EmailOrAuthId = newEmail;
                _DbContext.Clients.Update(client);
                await _DbContext.SaveChangesAsync();
                string message = $"مرحبًا {client.User.FirstName} {client.User.SecondName}،\n\n✅ تم تحديث البريد الإلكتروني الخاص بحسابك في جومانجو بنجاح.\n\n📧 <strong>البريد الإلكتروني الجديد:</strong> {newEmail}\n\nإذا كان لديك أي استفسار أو تحتاج إلى مساعدة، نحن هنا لخدمتك.";
                await SendNotificationToUser(client.UserId, message);

                return true;
            }
            return false;
        }
        public async Task<int> GetClientIdByUserId(int UserId)
        {
            var client = await _DbContext.Clients.FirstOrDefaultAsync(Client => Client.UserId == UserId);
            return client!.ClientId;
        }

        public async Task<string?> GetClientPhoneNumberById(int ClientId)
        {
            var client = await _DbContext.Clients.FirstOrDefaultAsync(Client => Client.ClientId == ClientId);
            return client!.PhoneNumber;
        }
        public async Task<Dictionary<int, string>> GetClientAddresses(int ClientId)
        {
            Dictionary<int, string> AddressesDic = await _DbContext.Addresses
                .Where(ad => ad.ClientId == ClientId)
                .ToDictionaryAsync(ad => ad.AddressId,
                    ad => ad.Governorate + "-" + " مدينه " + ad.City + "  شارع " + ad.St);

            return AddressesDic ?? new Dictionary<int, string>();
        }

        // دالة للحصول على بيانات العميل
        public async Task<ClientsDtos.GetClientReq> GetClientById(int ClientId)
        {
            var Client = await _DbContext.Clients.Include(c=>c.User) .Where(c => c.ClientId == ClientId).Include(c => c.Addresses).FirstOrDefaultAsync();
            if (Client != null)
            {
                return new ClientsDtos.GetClientReq
                {
                    FirstName = Client.User.FirstName,
                    LastName = Client.User.SecondName,
                    ClientAddresses = await GetClientAddresses(ClientId),
                    PhoneNumber = Client.PhoneNumber
                };
            }
            return null!;
        }

        // دالة لاسترجاع العملاء
        public async Task<List<GetClientsReq>> GetClientsAsync(int PageNum)
        {
                //users table cotaon all of the human on the app and the condtition to get persons within role id =3 its mean there are clients
            var clients = await _DbContext.Clients.Include(C => C.User).Where(C => C.User.RoleId == 3)
                .Select(c => new GetClientsReq
                {
                    FullName = c.User.FirstName + " " + c.User.SecondName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.User.EmailOrAuthId,
                    Password = c.User.PasswordHash, // تأكد من ضرورة إرجاع كلمة المرور وفقًا لمتطلبات الأمان
                    AuthProvider = c.User.AuthProvider
                })
                .Paginate(PageNum).ToListAsync();

            return clients ?? new List<GetClientsReq>();
        }
        public async Task<int> Count()
        {
            try
            {

                var ClientCount =await _DbContext.Clients.CountAsync();
                return ClientCount;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
}
    }
}
