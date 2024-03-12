using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Generators.Services;

public class StoredComponents : IUICStoredComponents, IUICStoredEvents
{
    #region Fields
    private readonly Dictionary<string, StoredComponentModel> _components = new Dictionary<string, StoredComponentModel>();
    private readonly Dictionary<string, StoredEventTrigger> _storedEvents = new Dictionary<string, StoredEventTrigger>();
    private readonly ILogger<StoredComponents> _logger;
    #endregion

    #region Ctor
    public StoredComponents(ILogger<StoredComponents> logger)
    {
        _logger = logger;
        _ = ClearStorageRecurring();
    }
    #endregion

    #region Properties
    /// <summary>
    /// The timespan the components can be stored
    /// </summary>
    public TimeSpan ClearComponents { get; set; } = TimeSpan.FromHours(1);
    #endregion

    #region Methods

    /// <summary>
    /// Try to get a component with a given key.
    /// </summary>
    public bool TryGetComponent(string key, out IUIComponent component)
    {
        try
        {
            component = GetComponent(key);
            return component == null;
        }catch
        {
            component = null;
            return false;
        }
    }

    /// <summary>
    /// Get a component by key. Returns null if key not found or expired.
    /// </summary>
    public IUIComponent GetComponent(string key)
    {
        lock (_components)
        {
            var result = _components[key];
            if (result == null)
                return null;

            if (result.SingleUse)
                _components.Remove(key);
            if(result.MaxLifeTime <  DateTime.Now)
            {
                _components.Remove(key);
                return null;
            }    

            return result.StoredComponent;
        }
    }

    /// <summary>
    /// Store a component and get a key for later use (Example: Can be send through signalR)
    /// </summary>
    /// <param name="component">The component that should be stored</param>
    /// <param name="singleUse">If true, component is removed after a single get request</param>
    /// <returns></returns>
    public string StoreComponent(IUIComponent component, bool singleUse)
    {
        lock (_components)
        {
            string key = string.Empty;
            do
            {
                key = $"UICKey{Guid.NewGuid().ToString("n")}";

            } while (_components.ContainsKey(key));

            _components[key] = new()
            {
                StoredComponent = component,
                SingleUse = singleUse,
                MaxLifeTime = DateTime.Now.Add(ClearComponents),
            };
            return key;
        }
    }

    public Task IncommingSignalRTrigger(string key, Dictionary<string, string> data)
    {
        Func<Dictionary<string, string>, Task> func = null;
        lock (_storedEvents)
        {
            if (!_storedEvents.ContainsKey(key))
                throw new KeyNotFoundException();
            var storedEvent = _storedEvents[key];
            if(storedEvent.MaxLifetime < DateTime.Now)
            {
                _storedEvents.Remove(key);
                throw new KeyNotFoundException();
            }
            if(storedEvent.SingleUse)
                _storedEvents.Remove(key);
            func = storedEvent.Func;
        }
        return func(data);
    }
    public string SubscribeIncommingEvent(Func<Dictionary<string, string>, Task> func, bool singleUse)
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
                MaxLifetime = DateTime.Now.Add(ClearComponents),
            };
            return key;
        }
    }

    #endregion

    #region Private methods
    private async Task ClearStorageRecurring()
    {

        try
        {
            while (true)
            {
                await Task.Delay(ClearComponents);
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
        lock (_components)
        {
            foreach (var kvp in _components)
            {
                if (kvp.Value.MaxLifeTime < removeOlderThan)
                    _components.Remove(kvp.Key);
            }
        }
            
        ClearStorage();
        
    }

    
    #endregion
}
