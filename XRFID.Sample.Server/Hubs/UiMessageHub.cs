using Microsoft.AspNetCore.SignalR;

namespace XRFID.Sample.Pages.Hubs;

public class UiMessageHub : Hub
{
    public async Task SendMessage()
    {
        await Clients.All.SendAsync("RefreshTag");
    }
}
