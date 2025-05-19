using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces.Services
{

    /// <summary>
    /// Apply this interface to the Hub<> in signalR. This adds 4 events that can be watched by other services.
    /// </summary>
    public interface IUICSignalRHub
    {
        /// <summary>
        /// Triggered when a client connects or reconnects
        /// </summary>
        public event EventHandler<SignalRHubEventArgs> OnConnectedEvent;

        /// <summary>
        /// Triggered when a client disconnects 
        /// </summary>
        public event EventHandler<SignalRHubEventArgs> OnDisconnectedEvent;

        /// <summary>
        /// Triggered when a client joins a provided group
        /// </summary>
        public event EventHandler<SignalRHubEventArgs> OnJoinGroupEvent;

        /// <summary>
        /// Triggered when a client leaves a provided group
        /// </summary>
        public event EventHandler<SignalRHubEventArgs> OnLeaveGroupEvent;
    }
    public class SignalRHubEventArgs : EventArgs
    {
        /// <summary>
        /// The Identifier provided in the hubContext
        /// </summary>
        public string? UserIdentifier { get; init; }

        /// <summary>
        /// The ConnectionId provided by the hubContext
        /// </summary>
        public string ConnectionId { get; init; }

        /// <summary>
        /// The Items provided by the hubContext
        /// </summary>
        public IDictionary<object, object> Items { get; init; }

        /// <summary>
        /// The CancelationAborted provided by the hubContext
        /// </summary>
        public CancellationToken ConnectionAborted { get; init; }


        /// <summary>
        /// The name of the group that is joined or left, only used by <see cref="IUICSignalRHub.OnJoinGroupEvent"/> or <see cref="IUICSignalRHub.OnLeaveGroupEvent"/>
        /// </summary>
        public string? GroupName { get; init; }
    }
}
