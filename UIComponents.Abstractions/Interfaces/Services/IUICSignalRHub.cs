using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces.Services
{
    public interface IUICSignalRHub
    {
        public event EventHandler<SignalRHubEventArgs> OnConnectedEvent;
        public event EventHandler<SignalRHubEventArgs> OnDisconnectedEvent;
        public event EventHandler<SignalRHubEventArgs> OnJoinGroupEvent;
        public event EventHandler<SignalRHubEventArgs> OnLeaveGroupEvent;
    }
    public class SignalRHubEventArgs : EventArgs
    {
        public string? UserIdentifier { get; init; }
        public string ConnectionId { get; init; }
        public IDictionary<object, object> Items { get; init; }
        public CancellationToken ConnectionAborted { get; init; }

        public string? GroupName { get; init; }
    }
}
