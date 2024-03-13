using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Generators.Services;

public class StoredEvents : IUICStoredEvents
{
    private readonly Dictionary<string, StoredEventTrigger> _storedEvents = new Dictionary<string, StoredEventTrigger>();

    private readonly ILogger _logger;

    public StoredEvents(ILogger<StoredEvents> logger)
    {
        _logger = logger;
        _= ClearStorageRecurring();
    }

    public Task IncommingSignalRTrigger(string key, Dictionary<string, string> data)
    {
        Func<Dictionary<string, string>, Task> func = null;
        lock (_storedEvents)
        {
            if (!_storedEvents.ContainsKey(key))
                throw new KeyNotFoundException();
            var storedEvent = _storedEvents[key];
            if (storedEvent.MaxLifetime < DateTime.Now)
            {
                _storedEvents.Remove(key);
                throw new KeyNotFoundException();
            }
            if (storedEvent.SingleUse)
                _storedEvents.Remove(key);
            func = storedEvent.Func;
        }
        return func(data);
    }
    public string SubscribeIncommingEvent(Func<Dictionary<string, string>, Task> func, TimeSpan lifeTime, bool singleUse)
    {
        lock (_storedEvents)
        {
            string key = string.Empty;
            do
            {
                key = $"UICKey{Guid.NewGuid().ToString("n")}";

            } while (_storedEvents.ContainsKey(key));

            _storedEvents[key] = new()
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
                await Task.Delay(TimeSpan.FromDays(1));
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
        lock (_storedEvents)
        {
            foreach (var kvp in _storedEvents)
            {
                if (kvp.Value.MaxLifetime < removeOlderThan)
                    _storedEvents.Remove(kvp.Key);
            }
        }

        ClearStorage();

    }
}
