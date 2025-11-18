using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.Clients;
using static StoreBusinessLayer.Clients.ClientsDtos;

namespace StoreServices.ClientsServices
{
    public interface IClient
    {
        Task SendNotificationToUser(int userId, string message);
         Task<int> AddNewClient(ClientsDtos.PostClientReq Dto);
        Task<bool> UpdateClientName(string FirstName, string LastName, int ClientId);
        Task<bool> AddOrUpdatePhoneToClientById(int ClientId, string phoneNumber);
        Task<int> AddNewAddress(ClientsDtos.PostAddressReq req, int ClientId);
        Task<bool> UpdateClientEmail(int ClientId, string newEmail);
        Task<int> GetClientIdByUserId(int UserId);
        Task<string?> GetClientPhoneNumberById(int ClientId);
        Task<Dictionary<int, string>> GetClientAddresses(int ClientId);
        Task<ClientsDtos.GetClientReq> GetClientById(int ClientId);
        Task<List<GetClientsReq>> GetClientsAsync(int PageNum);
         Task<int> Count();

    }
}
