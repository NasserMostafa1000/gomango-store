using System;
using System.Collections.Generic;
using StoreBusinessLayer.NotificationServices;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer;

namespace StoreServices.NotificationServices
{
    public class NotificationsFactoryDB
    {
        private Dictionary<string, INotifications> NotificationProviders;
        public NotificationsFactoryDB()
        {
            NotificationProviders = new Dictionary<string, INotifications>
            {
                { "gmail",  new GmailServes() },
            };
        }
        public INotifications GetNotificationProvider(string providerName)
        {
            if (NotificationProviders.ContainsKey(providerName.ToLower()))
            {
                return NotificationProviders[providerName.ToLower()];
            }

            throw new Exception($"Login provider '{providerName}' is not supported.");
        }
    }
}
