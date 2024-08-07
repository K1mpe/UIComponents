namespace UIComponents.Models.Models;

/// <summary>
/// The component that uses this will only be rendered once, and the html is stored in this component and can be reused when reloading the page.
/// <br>This means that all translations, permissionchecks, input values etc are all stored untill <see cref="ClearCache"/> occures</br>
/// </summary>
public class UICCached : IUIComponent
{
    #region Fields
    public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICCached));
    #endregion

    #region Ctor
    public UICCached(IUIComponent component)
    {
        Component = component;
    }
    public UICCached()
    {
        
    }
    #endregion

    #region Properties
    public IUIComponent Component { get; set; }

    public bool HasCachedValue { get; protected set; }

    public string CachedHtml { get; protected set; }
    #endregion

    #region Methods
    public UICCached ClearCache()
    {
        HasCachedValue = false;
        CachedHtml = string.Empty;
        return this;
    }

    public void SetCachedValue(RazerBlock block)
    {
        CachedHtml = block.GetContent();
    }
    #endregion
}
