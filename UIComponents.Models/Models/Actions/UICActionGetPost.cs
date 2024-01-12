using UIComponents.Models.Abstract;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// Get or post something to a controller. This post cannot be infected by clientside values.
/// </summary>
public class UICActionGetPost : UIComponent, IUIAction
{

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
    /// This is the serialized data that will be send with the ajax request.
    /// </summary>
    /// <remarks>
    /// If <see cref="Data"/> has value, this will replace the <see cref="RawData"/></remarks>
    public string RawData { get; set; }

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
    public IUIAction After { get; set; }

    #endregion

    public enum ActionTypeEnum
    {
        Get,
        Post
    }
}
