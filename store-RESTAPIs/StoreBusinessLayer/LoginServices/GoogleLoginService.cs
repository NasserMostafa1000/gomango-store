using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.ILoginServices;

namespace StoreBusinessLayer.Users
{
    public class GoogleLogin : ILoginServices
    {
        private readonly AppDbContext _context;

        public GoogleLogin(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoreDataAccessLayer.Entities.User> Login(string email ,string token, string password ) 
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new Exception("هناك خطأ في المصادقه مع جوجل يرجي المحاوله مره اخري بعد ثواني");
            }
            try
            {
                var payload = await ValidateGoogleTokenAsync(token);
                if (payload == null)
                {
                    throw new Exception("خطأ في رمز جوجل");
                }
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailOrAuthId == payload.Email);
                  //if User Exists Return it

                if (user != null)
                {
                    return user;
                }
                //if does not, create new user and return it
                else
                {
                    var newUser = new StoreDataAccessLayer.Entities.User
                    {
                        EmailOrAuthId = payload.Email, 
                        AuthProvider = "Google",
                        FirstName = payload.Name.Split(" ")[0],
                        SecondName = payload.Name.Split(" ")[1],
                        //User Role=3
                        RoleId =3
                    };
                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    var newClient = new Client
                    {
                    
                        UserId = newUser.UserId
                    };

                    await _context.Clients.AddAsync(newClient);
                    await _context.SaveChangesAsync();
                    string message = $@"
مرحبًا {newUser.FirstName} {newUser.SecondName}،

🎉 <strong>مرحبًا بك في جومانجو!</strong>

تم إنشاء حسابك بنجاح. يمكنك الآن الاستمتاع بجميع خدماتنا ومميزاتنا.

📧 <strong>بريدك الإلكتروني:</strong> {newClient.User!.EmailOrAuthId}

نتمنى أن تجد تجربتك معنا ممتعة ومفيدة. إذا كان لديك أي استفسار أو تحتاج إلى مساعدة، نحن هنا لخدمتك.

مع أطيب التحيات،
فريق جومانجو";

                    try
                    {

                    await NotificationServices.NotificationsCreator.SendNotification(
                        "مرحبًا بك في جومانجو - تم إنشاء حسابك بنجاح",  // العنوان
                        message,  // الرسالة الفعلية
                        newClient.User.EmailOrAuthId,  // البريد الإلكتروني الخاص بالعميل
                        "gmail"  // مزود الإشعار (يمكن تغييره حسب الخدمة المستخدمة)
                    );
                    }catch(Exception )
                    {
                        throw new Exception("خطأ في ارسال الاشعار ");
                    }




                    return newUser;
                }
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء
                  throw new Exception( ex.Message.ToString());
                
            }
        }
        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { "1002692311708-dv44b5us60jlovbgdcv87rbuvgfs01vo.apps.googleusercontent.com" } // ضع هنا clientId الخاص بك من جوجل
                };

                // تحقق من التوكن
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
     
    }
}
