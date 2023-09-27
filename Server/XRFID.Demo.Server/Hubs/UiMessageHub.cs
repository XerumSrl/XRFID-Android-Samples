using Microsoft.AspNetCore.SignalR;

namespace XRFID.Demo.Pages.Hubs;

public class UiMessageHub : Hub
{
    public async Task SendMessage()
    {
        await Clients.All.SendAsync("RefreshTag");
        await Clients.All.SendAsync("RefreshState");
    }
}
