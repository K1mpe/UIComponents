using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models;

public class UICContextMenuItem : IUIComponent, IUICHasScriptCollection
{
    #region Fields
    public string RenderLocation => this.CreateDefaultIdentifier();
    #endregion

    #region Ctor
    public UICContextMenuItem()
    {
    }

    public UICContextMenuItem(Func<string> selector, IDropdownItem dropdownItem, IUIAction onClick = null)
    {
        Selector = selector;
        DropdownItem = dropdownItem;
        OnClick = onClick;
    }

    public UICContextMenuItem(string selector, IDropdownItem dropdownItem, IUIAction onClick = null) : this(() => selector, dropdownItem, onClick)
    {
    }


    #endregion

    #region Properties

    /// <summary>
    /// The selector to define what element this menuItem can be used for
    /// </summary>
    /// <remarks>
    /// In Html: '#id' or '.class' or 'button'
    /// </remarks>
    public Func<string> Selector { get; set; }

    /// <summary>
    /// A dropdown item that is used to display this menu item
    /// </summary>
    public IDropdownItem DropdownItem { get; set; }

    /// <summary>
    /// The position in the contextMenu
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// Optional: Only one element can have this Id, if multiple have the same id, the most specific selector is used.
    /// </summary>
    /// <remarks>
    /// this can be used to overwrite some menu options for specific selectors
    /// </remarks>
    public string Id { get; set; }

    /// <summary>
    /// If provided, the categoryId that is used to group the items
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// The context menu will not show if all menu items are optional
    /// </summary>
    public bool Optional { get; set; }

    /// <summary>
    /// Optional: Action that is executed when clicking the menuItem
    /// </summary>
    /// <remarks>
    /// available properties:
    /// <br> target: the target that matches the selector</br>
    /// <br> clickedElement: the lowest element where the contextMenu has opened</br>
    /// <br> event: the clickevent</br>
    /// </remarks>
    public IUIAction OnClick { get; set; }

    /// <summary>
    /// Optional: A function that returns a string with the text that is displayed in the menuItem
    /// </summary>
    /// available properties:
    /// <br> target: the target that matches the selector</br>
    /// <br> clickedElement: the lowest element where the contextMenu has opened</br>
    /// </remarks>
    public IUIAction Text { get; set; }

    /// <summary>
    /// Optional: A function that returns a string with the text that is displayed in the tooltip
    /// </summary>
    /// available properties:
    /// <br> target: the target that matches the selector</br>
    /// <br> clickedElement: the lowest element where the contextMenu has opened</br>
    /// </remarks>
    public IUIAction Title { get; set; }

    /// <summary>
    /// Optional: A function that returns a object where each property is set as a attribute
    /// </summary>
    /// available properties:
    /// <br> target: the target that matches the selector</br>
    /// <br> clickedElement: the lowest element where the contextMenu has opened</br>
    /// </remarks>
    public new IUIAction Attributes { get; set; }

    IUICScriptCollection IUICHasScriptCollection.ScriptCollection { get; set; } = new UICScriptCollection();



    #endregion

}
