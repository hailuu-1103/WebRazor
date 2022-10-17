using Microsoft.AspNetCore.SignalR;

namespace WebRazor.Hubs
{
    public class HubServer : Hub
    {
        /*public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReloadProduct", user, message, DateTime.Now.ToShortTimeString);
        }*/
    }
}
