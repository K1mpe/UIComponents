using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Models.HtmlResponse;

namespace UIComponents.Web.Tests.Services;

public class MainHub : Hub<IUICSignalRService>, IUICSignalRHub
{
    public event EventHandler<SignalRHubEventArgs> OnConnectedEvent;
    public event EventHandler<SignalRHubEventArgs> OnDisconnectedEvent;
    public event EventHandler<SignalRHubEventArgs> OnJoinGroupEvent;
    public event EventHandler<SignalRHubEventArgs> OnLeaveGroupEvent;

    public override Task OnConnectedAsync()
    {
        OnConnectedEvent?.Invoke(this, GetArgs());
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        OnDisconnectedEvent?.Invoke(this, GetArgs());
        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGroup(string name)
    {
        OnJoinGroupEvent?.Invoke(this, GetArgs(name));
        await Groups.AddToGroupAsync(Context.ConnectionId, name);
    }

    public async Task LeaveGroup(string name)
    {
        OnLeaveGroupEvent?.Invoke(this, GetArgs(name));
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, name);
    }

    private SignalRHubEventArgs GetArgs(string groupName = null)
    {
        var args = new SignalRHubEventArgs()
        {
            UserIdentifier = Context.UserIdentifier,
            ConnectionId = Context.ConnectionId,
            Items = Context.Items,
            ConnectionAborted = Context.ConnectionAborted,
            GroupName = groupName
        };
        return args;
    }
}

public class SignalRService : IUICSignalRService
{
    #region Ctor
    public SignalRService(IHubContext<MainHub, IUICSignalRService> signalRHub)
    {
        SignalRHub = signalRHub;
    }

    #endregion

    #region Properties

    public IHubContext<MainHub, IUICSignalRService> SignalRHub { get; set; }


    #endregion

    public async Task RemoveUIComponentWithId(string id)
    {
        await SignalRHub.Clients?.All.RemoveUIComponentWithId(id);
    }

    public async Task SendUIComponentToUser(UICFetchComponent fetchComponent, string userId)
    {
        await SignalRHub.Clients?.All.SendUIComponentToUser(fetchComponent, userId);
    }

    public Task EventHandler(string key, object sender, object args)
    {
        return SignalRHub.Clients?.Group(key).EventHandler(key, sender, args);
    }

}
