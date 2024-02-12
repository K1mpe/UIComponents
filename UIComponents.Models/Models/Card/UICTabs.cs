using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Card;

public class UICTabs : UIComponent, IUICHasChildren<IUICTab>, IUICTab
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICTabs(string id)
    {
        Id = id;
    }
    #endregion

    #region Properties
    public List<IUICTab> Tabs { get; set; } = new();

    public List<IUIComponent> BeforeTabs { get; set; } = new();
    public List<IUIComponent> AfterTabs { get; set; } = new();  

    /// <summary>
    /// Allow each tab button to have content
    /// </summary>
    public bool ColorTabs { get; set; }

    /// <summary>
    /// Remember what tab was last accessed. Requires <see cref="Id"/> to be assigned
    /// </summary>
    public bool RememberTabState { get; set; } = true;

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
    public UICTabs AddBodyAttribute(string key, string value)
    {
        AddAttributeToDictionary(key, value, BodyAttributes);
        return this;
    }


    public UICTabs Add(IUICTab item)
    {
        return this.Add<UICTabs, IUICTab>(item);
    }

    public UICTabs Add<T>(out T added, T item) where T: IUICTab
    {
        return this.Add<UICTabs, T, IUICTab>(out added, item);
    }
    
    public UICTabs Add<T>(T item, Action<T> configure) where T : IUICTab
    {
        return this.Add<UICTabs, T, IUICTab>(item, configure);
    }

        #endregion

        #region Interface

    List<IUICTab> IUICHasChildren<IUICTab>.Children => Tabs;

    /// <summary>
    /// Only required if you use this tab as subtab
    /// </summary>
    public IHeader Header { get; set; } = new UICCardHeader(new Translatable("Tab.NoHeader"));

    IUIHasAttributes IUICTab.Content => this;

    #endregion
}

public enum UICCardWithTabsRenderer
{
    HorizontalTab,
    VerticalTab,
    HorizontalPill,
    VerticalPill,
}
