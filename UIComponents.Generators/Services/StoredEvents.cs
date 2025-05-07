using Microsoft.Extensions.Logging;
using System.Text.Json;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Generators.Services;

public class StoredEvents : IUICStoredEvents
{
    private readonly Dictionary<string, StoredEventTrigger> _storedEventsTriggers = new Dictionary<string, StoredEventTrigger>();
    private readonly Dictionary<string, StoredEventListener> _storedEventsListeners = new Dictionary<string, StoredEventListener>();
    private readonly IUICSignalRService _signalRService;
    private readonly ILogger _logger;
    private readonly IUICSignalRHub _hub;
    
    public StoredEvents(ILogger<StoredEvents> logger, IUICSignalRService signalRService, IUICSignalRHub hub = null)
    {
        _logger = logger;
        _signalRService = signalRService;
        _hub = hub;

        if(_hub != null)
        {
            _hub.OnLeaveGroupEvent += (sender, args) =>
            {
                lock (_storedEventsListeners)
                {
                    if(_storedEventsListeners.TryGetValue(args.GroupName, out var storedListener))
                    {
                        storedListener.RemoveConnectionId(args.ConnectionId);
                    }
                }
            };
            _hub.OnJoinGroupEvent += (sender, args) =>
            {
                lock (_storedEventsListeners)
                {
                    if (_storedEventsListeners.TryGetValue(args.GroupName, out var storedListener))
                    {
                        try
                        {
                            storedListener.AddConnectionId(args.ConnectionId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to subscribe event. Event is removed from memory");
                            _storedEventsListeners.Remove(args.GroupName);
                        }
                    }
                }
            };

            _hub.OnDisconnectedEvent += (sender, args) =>
            {
                lock (_storedEventsListeners)
                {
                    var listeners = _storedEventsListeners.Values.Where(x=>x.ConnectionIds.ContainsKey(args.ConnectionId));
                    foreach(var listener in listeners)
                    {
                        listener.RemoveConnectionId(args.ConnectionId);
                    }
                }
            };

            _hub.OnConnectedEvent += (sender, args) =>
            {
                lock (_storedEventsListeners)
                {
                    var listeners = _storedEventsListeners.Values.Where(x => x.ConnectionIds.ContainsKey(args.ConnectionId));
                    foreach (var listener in listeners)
                    {
                        listener.AddConnectionId(args.ConnectionId);
                    }
                }
            };
        }
        
        _ = ClearStorageRecurring();
    }

    public virtual Task IncommingSignalRTrigger(string key, Dictionary<string, string> data)
    {
        Func<Dictionary<string, string>, Task> func = null;
        lock (_storedEventsTriggers)
        {
            if (!_storedEventsTriggers.ContainsKey(key))
                throw new KeyNotFoundException();
            var storedEvent = _storedEventsTriggers[key];
            if (storedEvent.MaxLifetime < DateTime.Now)
            {
                _storedEventsTriggers.Remove(key);
                throw new KeyNotFoundException();
            }
            if (storedEvent.SingleUse)
                _storedEventsTriggers.Remove(key);
            func = storedEvent.Func;
        }
        return func(data);
    }
    public virtual string SubscribeIncommingEvent(Func<Dictionary<string, string>, Task> func, TimeSpan lifeTime, bool singleUse)
    {
        lock (_storedEventsTriggers)
        {
            string key = string.Empty;
            do
            {
                key = $"UICKey{Guid.NewGuid().ToString("n")}";

            } while (_storedEventsTriggers.ContainsKey(key));

            _storedEventsTriggers[key] = new()
            {
                Func = func,
                SingleUse = singleUse,
                MaxLifetime = DateTime.Now.Add(lifeTime),
            };
            return key;
        }
    }



    private async Task ClearStorageRecurring()
    {

        try
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromHours(1));
                ClearStorage();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

    }

    private void ClearStorage()
    {
        var removeOlderThan = DateTime.Now;
        lock (_storedEventsTriggers)
        {
            foreach (var kvp in _storedEventsTriggers)
            {
                if (kvp.Value.MaxLifetime < removeOlderThan)
                    _storedEventsTriggers.Remove(kvp.Key);
            }
        }

        lock (_storedEventsListeners)
        {
            var unsubscribedExpired = DateTime.Now.AddHours(-1);
            foreach(var kvp in _storedEventsListeners)
            {
                if (kvp.Value.Subscribed)
                    return;

                if(kvp.Value.SubscriptionChanged < unsubscribedExpired)
                {
                    _storedEventsListeners.Remove(kvp.Key);
                }
            }
        }
    }

    public string SubscribeOnEvent<TArgs>(Action<EventHandler<TArgs>> subscribe, Action<EventHandler<TArgs>> unsubscribe, string? eventId=null) where TArgs : EventArgs
    {
        if (_hub == null)
            throw new Exception($"Cannot use {nameof(UICEvent<TArgs>)} before implementing {nameof(IUICSignalRHub)}");

        if(string.IsNullOrWhiteSpace(eventId))
            eventId = Guid.NewGuid().ToString();

        lock (_storedEventsListeners)
        {
            if (_storedEventsListeners.ContainsKey(eventId))
                return eventId;
        }
        EventHandler<TArgs> handler = (sender, args) =>
        {
            _logger.LogDebug("Passing event to SignalR {0}", eventId);
            var senderDict = new Dictionary<string, string>();
            var argsDict = new Dictionary<string, string>();
            foreach(var prop in sender.GetType().GetProperties())
            {
                try
                {
                    senderDict[prop.Name] = JsonSerializer.Serialize(prop.GetValue(args));
                }
                catch
                {

                }
            }

            foreach(var prop in args.GetType().GetProperties())
            {
                try
                {
                    argsDict[prop.Name] = JsonSerializer.Serialize(prop.GetValue(args));
                }
                catch
                {

                }
            }
            _signalRService.EventHandler(eventId, senderDict, argsDict);
        };
        var listener = new StoredEventListener()
        {
            Guid = eventId,
            SubscribeEvent = () => subscribe(handler),
            UnSubscribeEvent = () => unsubscribe(handler)
        };
        lock (_storedEventsListeners)
        {
            _storedEventsListeners[eventId] = listener;
        }
        return eventId;
    }
}
