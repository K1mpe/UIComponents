using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models;

public class UICCardWithTabs : UIComponent
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICCardWithTabs(string id)
    {
        Id = id;
    }
    #endregion

    #region Properties
    public List<IUITabCard> Tabs { get; set; } = new();

    /// <summary>
    /// If only one tab is available, only render the content from that single tab
    /// </summary>
    /// <remarks>
    /// Tabs can be added or removed based on permissions
    /// </remarks>
    public bool OnlyRenderSingleContent { get; set; }

    public UICCardWithTabsRenderer Renderer { get; set; } = UICCardWithTabsRenderer.HorizontalTab;

    public string Id
    {
        get
        {
            return Attributes["id"];
        }
        set
        {
            Attributes["id"] = value;
        }
    }

    public Dictionary<string, string> BodyAttributes { get; set; } = new();
    #endregion

    #region Methods
    public UICCardWithTabs AddBodyAttribute(string key, string value)
    {
        AddAttributeToDictionary(key, value, BodyAttributes);
        return this;
    }


    /// <summary>
    /// Add a item to the collection and return the <paramref name="item"/>
    /// </summary>
    /// <returns><paramref name="item"/></returns>
    public T Add<T>(T item) where T : class, IUITabCard
    {
        Tabs.Add(item);
        return item;
    }

    /// <summary>
    /// Add a item to the collection and return the current <see cref="UICCardWithTabs"/>
    /// </summary>
    /// <returns><see cref="UICCardWithTabs"/></returns>
    public UICCardWithTabs Add2(IUITabCard item)
    {
        Tabs.Add(item);
        return this;
    }
    #endregion
}

public enum UICCardWithTabsRenderer
{
    HorizontalTab,
    VerticalTab,
    HorizontalPill,
    VerticalPill,
}
