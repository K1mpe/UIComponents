using Microsoft.AspNetCore.Routing;
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
        FixedData = (new RouteValueDictionary(data)).ToDictionary(x => x.Key, x => x.Value);
    }
    public UICActionGetPost(ActionTypeEnum actionType, string url, object data = null)
    {
        ActionType = actionType;
        Url = url;
        FixedData = (new RouteValueDictionary(data)).ToDictionary(x=> x.Key, x=>x.Value);
    }

    public UICActionGetPost()
    {

    }

    #endregion

    #region Properties

    public ActionTypeEnum ActionType { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }

    /// <summary>
    /// This will be included on post, This takes lowest priority and can be overwritten by <see cref="GetVariableData"/> and <see cref="FixedData"/>
    /// </summary>
    public Dictionary<string, object> DefaultData { get; set; } = new();

    /// <summary>
    /// Before sending the request, this action is called client side to get additional properties.
    /// <br>These properties have higher priority then <see cref="DefaultData"/> but lower than <see cref="FixedData"/></br>
    /// </summary>
    public IUICAction? GetVariableData { get; set; } = null;

    /// <summary>
    /// This will be included on post, and takes highest priority. This will overwrite all properties from <see cref="DefaultData"/> and <see cref="GetVariableData"/>
    /// </summary>
    public Dictionary<string, object> FixedData { get; set; } = new();

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
    public IUICAction? ClientSideOptions { get; set; } = null;



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
    public IUICAction OnSuccess { get; set; }

    /// <summary>
    /// This will be triggered after failing the request
    /// </summary>
    public IUICAction OnFailed { get; set; }

    #endregion


    #region Methods

    public UICActionGetPost AddDefaultData(string key, object value)
    {
        DefaultData[key] = value;
        return this;
    }
    public UICActionGetPost AddDefaultData(object data)
    {
        foreach(var kvp in new RouteValueDictionary(data))
        {
            AddDefaultData(kvp.Key, kvp.Value);
        }
        return this;
    }
    public UICActionGetPost AddFixedData(string key, object value)
    {
        FixedData[key] = value;
        return this;
    }
    public UICActionGetPost AddFixedData(object data)
    {
        foreach (var kvp in new RouteValueDictionary(data))
        {
            AddFixedData(kvp.Key, kvp.Value);
        }
        return this;
    }

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
        public List<IUICAction> Handlers { get; set; } = new();

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
    public UICActionGet(string url, object data = null) : base(ActionTypeEnum.Get, url, data)
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
    public UICActionPost(string url, object data = null) : base(ActionTypeEnum.Post, url, data)
    {

    }
    public UICActionPost()
    {
        ActionType = ActionTypeEnum.Post;
    }
} 
