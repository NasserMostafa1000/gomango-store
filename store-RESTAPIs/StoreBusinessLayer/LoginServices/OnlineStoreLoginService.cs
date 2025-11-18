using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.ILoginServices;

namespace StoreBusinessLayer.Users
{
    public class OnlineStoreLogin : ILoginServices
    {
        private readonly AppDbContext _Context;
        public OnlineStoreLogin(AppDbContext DbContext)
        {
            _Context = DbContext;
        }
       public async Task<StoreDataAccessLayer.Entities.User> Login(string Email,string Token, string Password)
        {
            if(string.IsNullOrEmpty(Email)||string.IsNullOrEmpty(Password))
            {
                throw new Exception("البريد الالكتروني او البسورد فارغ");
            }
            try
            {
                var user =  await _Context.Users.FirstOrDefaultAsync(user => user.EmailOrAuthId == Email);   
                if(user!=null&&string.IsNullOrEmpty(user.PasswordHash)!=true&&string.IsNullOrEmpty(user.Salt)!=true)
                {
                    bool IsPasswordValid = PasswordHelper.VerifyPassword(Password, user.PasswordHash, user.Salt);
                    return IsPasswordValid ? user :throw new Exception("كلمه سر خاطئه");
                }
                throw new Exception("حدث خطأ من الممكن ان هذا الحساب لم يسجل لدينا في الموقع من قبل قم بأنشاء حساب جديد بسهوله ويسر");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            
        }
    }
}
