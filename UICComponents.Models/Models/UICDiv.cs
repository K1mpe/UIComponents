namespace UIComponents.ComponentModels.Models;

public class UICDiv : UIComponent
{
    #region Fields
    public override bool Render
    {
        get
        {
            if (!RenderWithoutContent)
            {
                foreach (var content in Components)
                {
                    if (content is IConditionalRender cr)
                        if (cr.Render)
                            return true;
                }
                return false;
            }
            return base.Render;
        }
        set => base.Render = value;
    }
    #endregion

    #region Properties
    public string Id
    {
        get => this.GetOrGenerateId();
        set => Attributes["id"] = value;
    }

    public List<IUIComponent> Components { get; set; } = new();

    /// <summary>
    /// If false, this div will not be rendered if it does not contain content or the content is not rendered
    /// </summary>
    public bool RenderWithoutContent { get; set; }
    #endregion

    #region Methods
    public override string ToString()
    {
        string result = "<div";
        if (Attributes.TryGetValue("id", out string divId))
        {
            result = string.Join(" ", result, $"id={divId}");
        }
        if (Attributes.TryGetValue("class", out string divClass))
        {
            result = string.Join(" ", result, $"class={divClass}");
        }

        if (Attributes.TryGetValue("style", out string divStyle))
        {
            result = string.Join(" ", result, $"style={divStyle}");
        }
        result += ">";
        return result;
    }


    /// <summary>
    /// Add a item to the collection and return the <paramref name="item"/>
    /// </summary>
    /// <returns><paramref name="item"/></returns>
    public T Add<T>(T item) where T : class, IUIComponent
    {
        Components.Add(item);
        return item;
    }

    /// <summary>
    /// Add a item to the collection and return the current <see cref="UICDiv"/>
    /// </summary>
    /// <returns>This<see cref="UICDiv"/></returns>
    public UICDiv Add2(IUIComponent item)
    {
        Components.Add(item);
        return this;
    }
    #endregion
}
