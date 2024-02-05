using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// Get or post something to a controller. This post cannot be infected by clientside values.
/// </summary>
public class UICActionGetPost : UIComponent, ISubmitAction
{
    #region Fields
    public override string RenderLocation => UIComponent.DefaultIdentifier("ActionGetPost");
    #endregion

    #region Ctor
    public UICActionGetPost(ActionTypeEnum actionType, string controller, string action, object data = null)
    {
        ActionType = actionType;
        Controller = controller;
        Action = action;
        Data = data;
    }

    public UICActionGetPost()
    {

    }



    #endregion

    #region Properties
    public ActionTypeEnum ActionType { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }


    public object Data { get; set; }

    /// <summary>
    /// If not empty, use the url instead of <see cref="Controller"/> and <see cref="Action"/>
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// When providing post options, these will overwrite the default options.
    /// </summary>
    public UICGetPostOptions? Options { get; set; } = null;


    /// <summary>
    /// A function that returns options. <see cref="Options"/> still takes priority over this
    /// </summary>
    public IUIAction? ClientSideOptions { get; set; } = null;


    /// <summary>
    /// Before sending the request, this action is called client side to get additional properties.
    /// </summary>
    /// If this result has the same properties as <see cref="Data"/>, the <see cref="Data"/> takes priority.
    public IUIAction? GetVariableData { get; set; } = null;

    /// <summary>
    /// This is the name of the responsevalue
    /// </summary>
    public string ResultName { get; set; } = "result";

    /// <summary>
    /// This will be triggered after posting.
    /// </summary>
    /// <remarks>
    /// ResultName is available for this action
    /// </remarks>
    public IUIAction OnSuccess { get; set; }

    /// <summary>
    /// This will be triggered after failing the request
    /// </summary>
    public IUIAction OnFailed { get; set; }
    #endregion

    public enum ActionTypeEnum
    {
        Get,
        Post
    }



    public class UICGetPostOptions
    {
        /// <summary>
        /// These handlers will run before the default handlers
        /// <br>A handler takes the result from the request. If the handler does not return a result, the next handler will be triggered.</br>
        /// </summary>
        public List<IUIAction> Handlers { get; set; } = new();

        /// <summary>
        /// If a previous request is still running, cancel the previous request
        /// </summary>
        public bool CancelPreviousRequests { get; set; }
    }
}

public class UICActionGet : UICActionGetPost
{
    public UICActionGet(string controller, string action, object data = null) : base(ActionTypeEnum.Get, controller, action, data)
    {
    }

    public UICActionGet()
    {
        ActionType = ActionTypeEnum.Get;
    }
}

public class UICActionPost : UICActionGetPost
{
    public UICActionPost(string controller, string action, object data = null): base(ActionTypeEnum.Post, controller, action, data) 
    {

    }
    public UICActionPost()
    {
        ActionType = ActionTypeEnum.Post;
    }
} 
