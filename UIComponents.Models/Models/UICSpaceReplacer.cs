namespace UIComponents.Models.Models;


/// <summary>
/// If there is enough available space, display one item (or group), if less space is available, do not display this.
/// </summary>
public class UICSpaceReplacer : UIComponent
{
    #region Ctor
    public UICSpaceReplacer()
    {
    }
    public UICSpaceReplacer(IUIComponent bigElement, IUIComponent smallElement)
    {

    }
    /// <summary>
    /// Display the list of buttons if available, else create a dropdownList of all the buttons
    /// </summary>
    /// <param name="buttons">A list of buttons or dropdowns</param>
    /// <param name="dropdownButton">A button used to create the dropdown</param>
    public UICSpaceReplacer(IEnumerable<IUIComponent> buttons, UICButton dropdownButton)
    {
        BigElement = new UICGroup()
        {
            Children = buttons.ToList(),
            Renderer = UICGroupRenderer.ContentOnly
        };
        var dropdown = new UICDropdown(dropdownButton);
        foreach(var item in buttons)
        {
            if (item is UICButton button)
                dropdown.Add(button.ConvertToDropdownItem());
            else if (item is UICDropdown dropdown2)
                dropdown.Add(dropdown2.ConvertToSubMenu());
            
        }
        SmallElement= dropdown;

    }
    #endregion

    #region Properties
    /// <summary>
    /// Element that is displayed if there is enough space
    /// </summary>
    public IUIComponent BigElement { get; set; }

    /// <summary>
    /// Element that is displayed if there is not enough space for the <see cref="BigElement"/>
    /// </summary>
    public IUIComponent SmallElement { get; set; }

    #endregion
}
