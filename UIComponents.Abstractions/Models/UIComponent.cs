namespace UIComponents.Abstractions.Models;


/// <summary>
/// The base for most UICs
/// </summary>
public abstract class UIComponent : IUIComponent, IConditionalRender, IUICHasScriptCollection, IUIHasAttributes, IUICHasParent
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
                if (condition() == false)
                    return false;
            }
            return _render;
        }
        set => _render = value;
    }

    public List<Func<bool>> RenderConditions { get; set; } = new();

    public virtual string RenderLocation { get => this.CreateDefaultIdentifier(); }

    /// <summary>
    /// The scriptcollection is used to store scripts and styles so they can be rendered together
    /// </summary>
    public IUICScriptCollection ScriptCollection { get; set; } = new UICScriptCollection();

    public Dictionary<string, string> Attributes { get; set; } = new();

    [IgnoreGetChildrenFunction]
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
        return $"/UIComponents/{UICType}/{renderer ?? "Default"}";
    }

    #endregion


    public static implicit operator Func<string>(UIComponent uIComponent)
    {
        return () => $"#{uIComponent.GetId()}";
    }
}

