using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Generators.Services;

public class StoredComponents : IUICStoredComponents
{
    #region Fields
    private readonly Dictionary<string, StoredComponentModel> _components = new Dictionary<string, StoredComponentModel>();
    private readonly ILogger<StoredComponents> _logger;
    private readonly IUICSignalRService _signalRService;
    #endregion

    #region Ctor
    public StoredComponents(ILogger<StoredComponents> logger, IUICSignalRService signalRService=null)
    {
        _logger = logger;
        _ = ClearStorageRecurring();
        _signalRService = signalRService;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The timespan the components can be stored
    /// </summary>
    public TimeSpan ClearComponents { get; set; } = TimeSpan.FromHours(1);
    #endregion

    #region Methods

    #region Get 
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

    public List<IUIComponent> GetComponentsByUser(object userId)
    {
        if (userId == null)
            throw new ArgumentNullException(nameof(userId));
        lock (_components)
        {
            var results = _components.Where(x => x.Value.UserIds.Contains(userId.ToString())).ToList();
            foreach (var result in results.Where(x => x.Value.SingleUse))
            {
                _components.Remove(result.Key);
            }
            return results.Select(x => x.Value.StoredComponent).ToList();
        }
    }
    #endregion

    #region Store
    /// <summary>
    /// Store a component and get a key for later use (Example: Can be send through signalR)
    /// </summary>
    /// <param name="component">The component that should be stored</param>
    /// <param name="singleUse">If true, component is removed after a single get request</param>
    /// <returns></returns>
    public string StoreComponentForUsers(IUIComponent component, IEnumerable<object> userIds, bool singleUse)
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
                UserIds = userIds.Where(x=>x != null).Select(x=>x.ToString()).ToHashSet<string>()
            };
            return key;
        }
    }

    #endregion

    #region Remove


    public void RemoveStoredComponent(string key)
    {
        lock (_components)
        {
            _components.Remove(key);
        }
    }

    public void RemoveStoredComponent(string key, object userId)
    {
        if(userId == null)
            throw new ArgumentNullException(nameof(userId));

        lock (_components)
        {
            if(_components.TryGetValue(key, out var result))
            {
                if (result.UserIds.Contains(userId.ToString()))
                {
                    if (result.UserIds.Count == 1)
                        _components.Remove(key);
                    else
                        result.UserIds.Remove(userId.ToString());
                }
            }
        }
    }

    #endregion

    public Task SendComponentToUserSignalR(IUIComponent component, object userId)
    {
        if (_signalRService == null)
            throw new Exception($"There is no implementation for {nameof(IUICSignalRService)} registrated.");

        if (userId == null)
            throw new ArgumentNullException(nameof(userId));

        var key = StoreComponentForUsers(component, new object[]{ userId }, true);
        return _signalRService.SendUIComponentToUser(new(key), userId.ToString());
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
    }

    #endregion
}
