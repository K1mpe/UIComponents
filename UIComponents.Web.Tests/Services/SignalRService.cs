using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Models.HtmlResponse;

namespace UIComponents.Web.Tests.Services;

public class MainHub: Hub<IUICSignalRService>
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGroup(string name)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, name);
    }

    public async Task LeaveGroup(string name)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, name);
    }
}

public class SignalRService : IUICSignalRService
{
    #region Ctor
    public SignalRService(MainHub signalRHub)
    {
        SignalRHub = signalRHub;
    }

    #endregion

    #region Properties

    public MainHub SignalRHub { get; set; }


    #endregion

    public async Task RemoveUIComponentWithId(string id)
    {
        await SignalRHub.Clients.All.RemoveUIComponentWithId(id);
    }

    public async Task SendUIComponentToUser(FetchComponent fetchComponent, string userId)
    {
        await SignalRHub.Clients.All.SendUIComponentToUser(fetchComponent, userId);
    }

    public Task EventHandler(string key, object sender, EventArgs args)
    {
        return SignalRHub.Clients.All.EventHandler(key, sender, args);
    }
}
