using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreBusinessLayer.NotificationServices;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.LoginServices;
using StoreServices.UsersServices;

namespace StoreBusinessLayer.Users
{
    // business layer
    public class UsersRepo:IUser
    {
        private readonly AppDbContext _dbContext;
        private readonly TokenService _TokenServices;

        public UsersRepo(AppDbContext dbContext, TokenService TokenServices)
        {
            _dbContext = dbContext;
            _TokenServices = TokenServices;
        }

        // تسجيل الدخول
        public async Task<string> Login(UsersDtos.LoginReq req)
        {
            try
            {
                LoginFactory Factory = new LoginFactory(_dbContext);
                // استخدام نمط المصنع للحصول على مزود تسجيل الدخول
                var Provider = Factory.GetLoginProvider(req.AuthProvider);

                // تسجيل الدخول باستخدام Google يتطلب الرمز المميز، في حين أن تسجيل الدخول باستخدام متجرنا يتطلب البريد الإلكتروني وكلمة المرور
                var user = await Provider.Login(req.Email!, req.Token!, req.Password!);

                // عند الوصول إلى هذه السطر، يعني أن الحساب صحيح، سيتم إنشاء التوكن هنا
                return _TokenServices.CreateToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        // إضافة مستخدم جديد
        public async Task<int> PostUser(UsersDtos.PostUserReq userDto, byte RoleId)
        {
            var existingUser =await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailOrAuthId == userDto.EmailOrAuthId);
            if (existingUser != null)
            {
                throw new InvalidOperationException("هذا الحساب موجود من قبل");
            }
            try
            {
                string salt;
                string HashedPass = PasswordHelper.HashPassword(userDto.Password, out salt);
                var User = new StoreDataAccessLayer.Entities.User
                {
                    AuthProvider = userDto.AuthProvider,
                    FirstName = string.IsNullOrEmpty(userDto.FirstName)? "website": userDto.FirstName,
                    SecondName = string.IsNullOrEmpty(userDto.SecondName) ? "Admin" : userDto.SecondName,
                    EmailOrAuthId = userDto.EmailOrAuthId,
                    PasswordHash = HashedPass,
                    Salt = salt,
                    RoleId = RoleId
                };

                await _dbContext.Users.AddAsync(User);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Clients.AddAsync(new Client { UserId = User.UserId });
                await _dbContext.SaveChangesAsync();



                // إرسال إشعار عند إضافة مستخدم جديد
                string message = $"مرحبًا {userDto.EmailOrAuthId},\n\nتم إنشاء حسابك بنجاح.\n\nنتمنى لك تجربة رائعة في متجرنا.";

                await NotificationsCreator.SendNotification("نرحب بك في متجرنا", message, userDto.EmailOrAuthId, "gmail");

                return User.UserId;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message.ToString()}");
            }
        }

        public async Task<bool> ChangeOrForgotPasswordAsync(string Email, string NewPassword, bool ForgotPassword = false, string CurrenPassword = "")
        {
            //this method used to change Password with the current password  or create new password and send it to the gmail
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    throw new Exception("البريد الالكتروني فارغ");
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailOrAuthId == Email);
                if (user == null)
                {
                    throw new Exception("البريد الالكتروني غير موجود");
                }

                if (ForgotPassword)
                {
                    string Salt;
                    var NewHashPassword = PasswordHelper.HashPassword(NewPassword, out Salt);
                    user.PasswordHash = NewHashPassword;
                    user.Salt = Salt;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();

                    // إرسال إشعار عند تغيير كلمة المرور
                    string message = $"تم تغيير كلمة مرور حسابك بنجاح. يُوصى بشدة بتغيير كلمة المرور هذه في أقرب وقت.";
                    await NotificationsCreator.SendNotification("تغيير كلمة المرور", message, Email, "Gmail");

                    return true;
                }
                else
                {
                    if (string.IsNullOrEmpty(CurrenPassword))
                    {
                        throw new Exception("البسورد الحالي مطلوب");
                    }

                    bool IsPasswordCorrect = PasswordHelper.VerifyPassword(CurrenPassword, user.PasswordHash, user.Salt);
                    if (IsPasswordCorrect)
                    {
                        string Salt;
                        string HashedPassword = PasswordHelper.HashPassword(NewPassword, out Salt);
                        user.PasswordHash = HashedPassword;
                        user.Salt = Salt;
                        _dbContext.Users.Update(user);
                        await _dbContext.SaveChangesAsync();

                        // إرسال إشعار عند تغيير كلمة المرور
                        string message = $"تم تغيير كلمة المرور بنجاح. إذا لم تقم بهذا التغيير، يرجى الاتصال بنا على الفور.";
                        await NotificationsCreator.SendNotification("تغيير كلمة المرور", message, Email, "gmail");

                        return true;
                    }
                    else
                    {
                        throw new Exception("البسورد الحالي خاطئ");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ChangePassword: {ex.Message.ToString()}");
            }
        }

        // إرسال إشعار لإعادة تعيين كلمة المرور
        public async Task<bool> NotificationForForgotPassword(string Email, string NotificationProvider)
        {
            try
            {
                string randomPassword = PasswordHelper.GenerateRandomPassword(8);
                bool isPasswordChanged = await ChangeOrForgotPasswordAsync(Email, randomPassword, true);
                if (isPasswordChanged)
                {
                    string subject = "إعادة تعيين كلمة المرور - جومانجو";
                    string message = $@"
عزيزي/عزيزتي،

🔐 <strong>إعادة تعيين كلمة المرور</strong>

لقد تم إعلامنا بأنك قد نسيت كلمة مرور حسابك في جومانجو. لا داعي للقلق، فقد تم إنشاء كلمة سر جديدة عشوائية لك.

⚠️ <strong>كلمة السر الجديدة:</strong> {randomPassword}

<strong>نوصي بشدة بتغيير هذه الكلمة فوراً</strong> لضمان أمان حسابك. يمكنك تغيير كلمة المرور من خلال حسابك الشخصي.

إذا كنت بحاجة إلى مساعدة إضافية، لا تتردد في التواصل معنا.

مع أطيب التحيات،
فريق جومانجو";

                    // إرسال الإشعار باستخدام الكود
                    await NotificationsCreator.SendNotification(subject, message, Email, NotificationProvider);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message.ToString()}");
            }
        }

        // الحصول على معلومات المستخدم
        public async Task<UsersDtos.GetUserInfo> GetUserInfo(int UserId)
        {
            var UserObj = await _dbContext.Users.FirstOrDefaultAsync(U => U.UserId == UserId);
            if (UserObj != null)
            {
                return new UsersDtos.GetUserInfo { HashedPassword = UserObj.PasswordHash, UserName = UserObj.EmailOrAuthId };
            }
            else
            {
                throw new ArgumentNullException(nameof(UserObj));
            }
        }

        // تعيين كلمة مرور للمرة الأولى
        public async Task<bool> SetPasswordForFirstTime(string Password, string Email)
        {
            var UserObj = await _dbContext.Users.FirstOrDefaultAsync(U => U.EmailOrAuthId == Email);
            if (UserObj == null)
            {
                throw new ArgumentNullException(nameof(UserObj));
            }
            if (!string.IsNullOrWhiteSpace(UserObj.PasswordHash))
            {
                throw new Exception("Current Password Required");
            }
            string HashedPassword;
            string Salt;
            HashedPassword = PasswordHelper.HashPassword(Password, out Salt);
            UserObj.Salt = Salt;
            UserObj.PasswordHash = HashedPassword;
            _dbContext.Users.Update(UserObj);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<UsersDtos.GetManagersReq>> GetEmployees()
        {
            var managers = await _dbContext.Users.Include(u => u.Role)
                .Where(user =>  user.RoleId != 1&&user.RoleId!=3)
                .Select(user => new UsersDtos.GetManagersReq
                {
                    Email = user.EmailOrAuthId,
                    Password = user.PasswordHash,
                    FullName = user.FirstName + " " + user.SecondName,
                    RoleName = user.Role.RoleName 
                })
                .ToListAsync();

            return managers.Count > 0 ? managers : new List<UsersDtos.GetManagersReq>();
        }
        public async Task<bool> RemoveManager(string email)
        {
            try
            {
                var manager = await _dbContext.Users.FirstOrDefaultAsync(user => user.EmailOrAuthId == email&&user.UserId!=1);
                if (manager == null)
                    return false;

                var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.UserId == manager.UserId);
                if (client != null)
                {
                    _dbContext.Clients.Remove(client);
                }
                _dbContext.Users.Remove(manager);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
