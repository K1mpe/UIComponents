namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICStoredComponents
{
    #region Get
    /// <summary>
    /// Try to get a component with a given key.
    /// </summary>
    public bool TryGetComponent(string key, out IUIComponent component);

    /// <summary>
    /// Get a component by key. Returns null if key not found or expired.
    /// </summary>
    public IUIComponent GetComponent(string key);

    /// <summary>
    /// Get all the stored components for this user
    /// </summary>
    public List<IUIComponent> GetComponentsByUser(object userId);
    #endregion

    #region Store

    /// <summary>
    /// Store a component and get a key for later use (Example: Can be send through signalR)
    /// </summary>
    /// <param name="component">The component that should be stored</param>
    /// <param name="singleUse">If true, component is removed after a single get request</param>
    /// <returns></returns>
    public string StoreComponentForUsers(IUIComponent component, IEnumerable<object> userIds, bool singleUse);

    #endregion

    #region Remove
    /// <summary>
    /// Remove the stored component by the key. This will remove for all users
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void RemoveStoredComponent(string key);

    /// <summary>
    /// Remove the stored component by key and userId
    /// </summary>
    /// <param name="key"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public void RemoveStoredComponent(string key, object userId);
    #endregion

    #region SignalR

    public Task SendComponentToUserSignalR(IUIComponent component, object userId);

    #endregion

}
