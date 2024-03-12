namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICStoredComponents
{
    /// <summary>
    /// Try to get a component with a given key.
    /// </summary>
    public bool TryGetComponent(string key, out IUIComponent component);

    /// <summary>
    /// Get a component by key. Returns null if key not found or expired.
    /// </summary>
    public IUIComponent GetComponent(string key);

    /// <summary>
    /// Store a component and get a key for later use (Example: Can be send through signalR)
    /// </summary>
    /// <param name="component">The component that should be stored</param>
    /// <param name="singleUse">If true, component is removed after a single get request</param>
    /// <returns></returns>
    public string StoreComponent(IUIComponent component, bool singleUse);
}
