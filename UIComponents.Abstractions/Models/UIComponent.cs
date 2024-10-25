using System.Xml.Linq;

namespace UIComponents.Abstractions.Models;


/// <summary>
/// The base for most UICs
/// </summary>
public abstract class UIComponent : IUIComponent, IUICConditionalRender, IUICHasScriptCollection, IUICHasAttributes, IUICHasParent, IUICInitializeAsync
{
    #region Fields
    private bool _render = true;
    #endregion


    #region Ctor
    public UIComponent()
    {
    }
    #endregion

    #region Properties


    public bool Hidden { get; set; }

    /// <summary>
    /// If this option is False, this content will not be rendered in UI.
    /// </summary>
    public virtual bool Render
    {
        get
        {
            foreach (var condition in RenderConditions)
            {
                if (condition(this) == false)
                    return false;
            }
            return _render;
        }
        set => _render = value;
    }

    /// <summary>
    /// If one of these functions returns false, the component cannot be rendered.
    /// <br>This function takes itself as argument, this is safer than using <see href="this"/></br>
    /// </summary>
    /// <remarks>
    /// If you use <see href="this"/> instead of the variable, and you create a deep copy from the component, these conditions still check on the original component and not the copied one.
    /// </remarks>
    public List<Func<UIComponent,bool>> RenderConditions { get; set; } = new();

    public virtual string RenderLocation { get => this.CreateDefaultIdentifier(); }

    /// <summary>
    /// The scriptcollection is used to store scripts and styles so they can be rendered together
    /// </summary>
    public virtual IUICScriptCollection ScriptCollection { get; set; } = new UICScriptCollection();

    public Dictionary<string, string> Attributes { get; set; } = new();

    [UICIgnoreGetChildrenFunction]
    public IUIComponent? Parent { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Adding attributes to this element
    /// </summary>
    /// <param name="seperator">If this key already exists, add the value after this seperator</param>
    //public void AddAttribute(string key, string value, string seperator = " ")
    //{
    //    AddAttributeToDictionary(key, value, Attributes, seperator);
    //}
    public void AddAttributeToDictionary(string key, string value, Dictionary<string, string> dict, string seperator = " ")
    {
        key = key.ToLower();
        if (!dict.TryGetValue(key, out string existing))
            existing = "";

        if (!string.IsNullOrEmpty(existing) && existing.Contains(value))
            return;
        if (string.IsNullOrEmpty(existing))
            existing = value;
        else
            existing = string.Join(seperator, existing, value);

        dict[key] = existing;
        return;
    }




    public override string ToString()
    {
        string toString = "";
        if (!Render)
            toString += "<NotRendered!>";
        else if (Hidden)
            toString += "<HIDDEN>";
        toString = string.Join(" ", toString, GetType().Name);
        return toString;
    }

    public static string DefaultIdentifier(string UICType, object renderer = null)
    {
        if (UICType.StartsWith("UIC"))
            UICType = UICType.Substring(3);
        return $"/UIComponents/ComponentViews/{UICType}/{renderer ?? "Default"}";
    }

    protected virtual Task InitializeAsync()
    {
        this.AddClass("uic");
        if (Hidden)
            this.AddAttribute("hidden", "true");
        return Task.CompletedTask;
    }
    Task IUICInitializeAsync.InitializeAsync()
    {
        return InitializeAsync();
    }

    #endregion


    public static implicit operator Func<string>(UIComponent uIComponent)
    {
        return () => $"#{uIComponent.GetId()}";
    }
}

