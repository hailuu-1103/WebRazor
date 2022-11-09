using Microsoft.AspNetCore.SignalR;

namespace WebRazor.Hubs
{
    public class HubServer : Hub
    {
        public void HasNewData()
        {
            Clients.All.SendAsync("Reload");
        }
    }
}
