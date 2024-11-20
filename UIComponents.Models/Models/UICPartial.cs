using static UIComponents.Models.Models.Actions.UICActionGetPost;

namespace UIComponents.Models.Models;

public class UICPartial : UIComponent, IUICHasChildren<IUIComponent>, IUICSupportsTaghelperContent
{

    #region Ctor

    public UICPartial(string controller, string action, object data = null, ActionTypeEnum getPost = ActionTypeEnum.Get) : this(new UICActionGetPost(getPost,controller, action, data))
    {

    }

    public UICPartial(string url, object data = null, ActionTypeEnum getPost = ActionTypeEnum.Get) : this(new UICActionGetPost(getPost, url, data))
    {

    }

    public UICPartial(UICActionGetPost getHtml) : this()
    {
        GetHtml = getHtml;
    }
    public UICPartial()
    {

    }
    #endregion

    #region Properties

    public UICActionGetPost GetHtml { get; set; }

    /// <summary>
    /// Child elements that are displayed until the first load
    /// </summary>
    public List<IUIComponent> Children { get; set; } = new();

    /// <summary>
    /// Do not load the partial on initial load
    /// </summary>
    public bool SkipInitialLoad { get; set; }

    /// <summary>
    /// Show the overlay during load
    /// </summary>
    public bool ShowLoadingOverlay { get; set; } = true;

    /// <summary>
    /// When triggering 'uic-reload', this is the delay before the reload is exicuted.
    /// <br>This is usefull if multiple signalR triggers can influence the partial at the same time, only one trigger will be run</br>
    /// <br>This uses the same function as <see cref="UICActionDelayedAction"/></br>
    /// </summary>
    public TimeSpan? ReloadDelay { get; set; }

    /// <summary>
    /// if false, the reload will not be exexcuted if the content is hidden (example: partial in a closed card)
    /// </summary>
    /// <remarks>
    /// If the parent card or tab opens and a refresh was triggered while closed, the refresh will then be triggered again.
    /// <br>Use <see cref="ReloadIfParentOpens"/> to refresh on each opening</br>
    /// </remarks>
    public bool ReloadIfHidden { get; set; } = true;

    /// <summary>
    /// If the parent card opens, refresh the content
    /// </summary>
    public bool ReloadIfParentOpens { get; set; } = false;

    /// <summary>
    /// When reloading the partial, this partial container will be removed.
    /// </summary>
    /// <remarks>
    /// This should only be used if the loaded partial starts with a new partial
    /// </remarks>
    public bool ReplaceSelf { get; set; }
    #endregion

    #region Methods
    public IUICAction? BeforeFetch { get; set; }
    public IUICAction? AfterFetch { get; set; }
    #endregion


    #region Triggers
    public IUICAction TriggerReload() => new UICActionRefreshPartial(this);
    #endregion


    bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;
    /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
    protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        var child = new UICCustom(taghelperContent);
        this.Add(child);
        return Task.CompletedTask;
    }
    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
}
