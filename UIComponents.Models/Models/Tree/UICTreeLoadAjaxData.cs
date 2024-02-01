namespace UIComponents.Models.Models.Tree;

public class UICTreeLoadAjaxData : IUIAction
{
    #region Interfaces
    public string RenderLocation => this.CreateDefaultIdentifier();
    #endregion

    #region Ctor
    public UICTreeLoadAjaxData(UICActionGetPost getPostAction, string initialId)
    {
        InitialId= initialId;
        getPostAction.GetVariableData = new UICCustom("jsTreeNode");
        Result= getPostAction;
    }

    public UICTreeLoadAjaxData()
    {

    }
    #endregion

    #region Properties
    /// <summary>
    /// This Id is send on the first request, to get the initial items
    /// </summary>
    public string InitialId { get; set; }

    /// <summary>
    /// A function that creates a property 'result' with the data required to parse in the JsTree
    /// </summary>
    public IUIAction Result { get; set; }
    /*
     
    public async Task<IActionResult> GetData(string id)
    {
        var items = new UICTreeItems();
        items.Items = await GetItemsAsync(id);
        return Json(items);
    }
     */
    #endregion
}
