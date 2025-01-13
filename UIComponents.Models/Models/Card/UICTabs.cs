using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Card;

namespace UIComponents.Models.Models.Card
{
    public class UICTabs : UIComponent, IUICHasChildren<IUICTab>, IUICTab, IUICSupportsTaghelperContent
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
        public bool ColorTabs { get; set; } = Defaults.Models.Card.UICTabs.ColorTabs;

        /// <summary>
        /// Remember what tab was last accessed. Requires <see cref="Id"/> to be assigned
        /// </summary>
        public bool RememberTabState { get; set; } = Defaults.Models.Card.UICTabs.RememberTabState;

        /// <summary>
        /// If true, the full width of the tabs is used and split even by the tabs. This makes the tabs larger than they need to be.
        /// </summary>
        /// <remarks>
        /// Only works on horizontal tabs
        /// </remarks>
        public bool CenterTabs { get; set; } = Defaults.Models.Card.UICTabs.CenterTabs;

        /// <summary>
        /// If only one tab is available, only render the content from that single tab
        /// </summary>
        /// <remarks>
        /// Tabs can be added or removed based on permissions
        /// </remarks>
        public bool OnlyRenderSingleContent { get; set; } = Defaults.Models.Card.UICTabs.OnlyRenderSingleContent;

        public UICCardWithTabsRenderer Renderer { get; set; } = UIComponents.Defaults.Models.Card.UICTabs.Renderer;
        public UICCardWithTabsRenderer TabOrientation{ 
            get => Renderer; 
            set => Renderer = value; 
        }
        public UICCardWithTabsStyle TabStyle { get; set; } = UIComponents.Defaults.Models.Card.UICTabs.TabStyle;

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

        public UICTabs Add<T>(out T added, T item) where T : IUICTab
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
        public IUICHeader Header { get; set; } = new UICCardHeader(TranslatableSaver.Save("Tab.NoHeader")) { Color = Defaults.Models.Card.UICTabs.HeaderColor?.Invoke() };

        IUICHasAttributes IUICTab.Content => this;

        bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;

        /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
        protected virtual async Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
        {
            var card = await UICCard.CreateFromContentAndAttributes(taghelperContent, attributes);
            this.Add(card);
        }
        Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
        #endregion
    }
    public enum UICCardWithTabsRenderer
    {
        Horizontal,
        Vertical,
    }
    public enum UICCardWithTabsStyle
    {
        Tabs,
        Pills
    }
}
namespace UIComponents.Defaults.Models.Card
{
    public static class UICTabs
    {
        public static bool ColorTabs { get; set; }

        public static bool RememberTabState { get; set; } = true;

        public static bool CenterTabs { get; set; }

        public static bool OnlyRenderSingleContent { get; set; }

        public static Func<IColor> HeaderColor { get; set; }
        public static UICCardWithTabsRenderer Renderer { get; set; }
        public static UICCardWithTabsStyle TabStyle { get; set; }
    }
}


