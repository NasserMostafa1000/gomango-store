using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Users;

namespace StoreServices.UsersServices
{
    public interface IUser
    {
        Task<bool> RemoveManager(string email);
        Task<List<UsersDtos.GetManagersReq>> GetEmployees();
        Task<bool> SetPasswordForFirstTime(string Password, string Email);
        Task<UsersDtos.GetUserInfo> GetUserInfo(int UserId);
        Task<bool> NotificationForForgotPassword(string Email, string NotificationProvider);
        Task<bool> ChangeOrForgotPasswordAsync(string Email, string NewPassword, bool ForgotPassword = false, string CurrenPassword = "");
        Task<int> PostUser(UsersDtos.PostUserReq userDto, byte RoleId);
        Task<string> Login(UsersDtos.LoginReq req);
    }
}
