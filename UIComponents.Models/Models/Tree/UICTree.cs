namespace UIComponents.Models.Models.Tree;

public class UICTree : IUIComponent, IUICHasScriptCollection
{
    #region Interface
    public string RenderLocation => this.CreateDefaultIdentifier();

    public IUICScriptCollection ScriptCollection { get; set; } = new UICScriptCollection();
    #endregion

    #region Ctor
    public UICTree()
    {

    }
    public UICTree(string id, UICTreeLoadAjaxData GetdataFunc)
    {
        Id= id;
        GetData = GetdataFunc;
    }

    public UICTree(string id, List<UICTreeItem> items)
    {
        Id= id;
        TreeItems= items;
    }

    #endregion


    #region Properties
    public string Id { get; set; }


    /// <summary>
    /// Save the collapsed state in local storage
    /// </summary>
    /// <remarks>
    /// This requires a Unique Id that remains constant to load the saved state
    /// </remarks>
    public bool SaveState { get; set; } = true;
    #endregion


    #region Checkbox
    public bool EnableCheckbox { get; set; }


    /// <summary>
    /// If checkbox is selected, also select parent checkboxes
    /// </summary>
    /// <remarks>
    /// Does not work while <see cref="CheckboxThreeState"/> is enabled
    /// </remarks>
    public bool CheckboxCascadeUp { get; set; }

    /// <summary>
    /// If checkbox is selected, also select all child checkboxes
    /// </summary>
    /// <remarks>
    /// Does not work while <see cref="CheckboxThreeState"/> is enabled
    /// </remarks>
    public bool CheckboxCascadeDown { get; set; } = true;

    public bool CheckboxThreeState { get; set; } = true;
    #endregion


    public bool EnableDragAndDrop { get; set; }

    /// <summary>
    /// Event that is executed when a item gets moved.
    /// </summary>
    /// <remarks>
    /// Available args: <i>event, data</i></remarks>
    public IUIComponent OnMove { get; set; }

    /// <summary>
    /// A function that gets the child elements through ajax request.
    /// </summary>
    /// Available args: <i>obj, callback</i></remarks>
    public IUIComponent GetData { get; set; }

    public List<UICTreeItem> TreeItems { get; set; } = new();


    public List<IUIComponent> CustomComponents { get; set; } = new();
}
