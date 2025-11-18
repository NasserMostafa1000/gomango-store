using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer;

namespace StoreServices.LoginServices
{
    public class LoginFactory
    {
        private readonly Dictionary<string, ILoginServices.ILoginServices> LoginProviderNames_loginsObjects;

        public LoginFactory(AppDbContext dbContext)
        {
            LoginProviderNames_loginsObjects = new Dictionary<string, ILoginServices.ILoginServices>
            {
                { "google", new GoogleLogin(dbContext) },
                { "facebook", new FaceBookLoginService(dbContext) },

               { "online store", new OnlineStoreLogin(dbContext) }

            };
        }
        public ILoginServices.ILoginServices GetLoginProvider(string providerName)
        {
            if (LoginProviderNames_loginsObjects.ContainsKey(providerName.ToLower()))
            {
                return LoginProviderNames_loginsObjects[providerName.ToLower()];
            }

            throw new Exception($"Login provider '{providerName}' is not supported.");
        }
    }
}
