using static UIComponents.Models.Models.Actions.UICActionGetPost;

namespace UIComponents.Models.Models;

public class UICPartial : UIComponent, IUICHasChildren<IUIComponent>
{

    #region Ctor

    public UICPartial(string controller, string action, object data = null, ActionTypeEnum getPost = ActionTypeEnum.Get) : this(new UICActionGetPost(getPost,controller, action, data))
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
    /// if false, the reload will not be exexcuted if the content is hidden (example: partial in a closed card)
    /// </summary>
    /// <remarks>
    /// If no content is loaded & a parent card or tab opens, this will be triggered (only first-time).
    /// <br>Use <see cref="ReloadIfParentOpens"/> to refresh on each opening</br>
    /// </remarks>
    public bool ReloadIfHidden { get; set; } = false;

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
    public IUIAction? BeforeFetch { get; set; }
    public IUIAction? AfterFetch { get; set; }
    #endregion


    #region Triggers
    public IUIAction TriggerReload()
    {
        throw new NotImplementedException();
    }
    #endregion
}
