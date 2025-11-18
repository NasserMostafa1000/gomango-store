using Microsoft.AspNetCore.SignalR;

namespace OnlineStoreAPIs.Hubs
{
    public class OrderHub  : Hub    
    {
        public async Task SendMessage(string Message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Message);
        }
    }
}
