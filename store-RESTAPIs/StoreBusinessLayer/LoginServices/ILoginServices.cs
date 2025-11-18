using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer.Entities;

namespace StoreServices.ILoginServices
{
    public interface ILoginServices
    {
        Task<User> Login(string Email, string Token, string Password);
    }

}
