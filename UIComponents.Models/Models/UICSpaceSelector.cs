using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models;


/// <summary>
/// Provide a list of elements, the UI will choose the best fitting element to display on the screenside.
/// </summary>
/// <remarks>
/// UI can change automatically when content resizes 
/// </remarks>
public class UICSpaceSelector : UIComponent
{
    #region Ctor
    public UICSpaceSelector()
    {
    }
    public UICSpaceSelector(IUIComponent bigElement, IUIComponent smallElement)
    {
        Elements = new() { bigElement, smallElement };
    }
    /// <summary>
    /// Display the list of buttons if available, else create a dropdownList of all the buttons
    /// </summary>
    /// <param name="buttons">A list of buttons or dropdowns</param>
    /// <param name="dropdownButton">A button used to create the dropdown</param>
    public UICSpaceSelector(IEnumerable<IUIComponent> buttons, UICButton dropdownButton)
    {
        var BigElement = new UICGroup()
        {
            Children = buttons.ToList(),
            Renderer = UICGroupRenderer.ContentOnly
        };
        var dropdown = new UICDropdown(dropdownButton);
        foreach(var item in buttons)
        {
            if (item is UICButton button)
            {
                var id = button.GetAttribute("id");
                var dropdownItem =button.ConvertToDropdownItem();
                if(!string.IsNullOrEmpty(id))
                    dropdownItem.Attributes["id"] = "dropdown-"+id;
                dropdown.Add(dropdownItem);

            }
                
            else if (item is UICDropdown dropdown2)
                dropdown.Add(dropdown2.ConvertToSubMenu());
            
        }
        Elements = new() { BigElement, dropdown };

        }
    #endregion

    #region Properties

    /// <summary>
    /// Display the largest of these elements thats fits the available space
    /// </summary>
    public List<IUIComponent> Elements { get; set; } = new();

    /// <summary>
    /// A function that finds the closest parent of this selector
    /// </summary>
    /// <remarks>
    /// Example: 
    /// <br>() => $"#{card.GetId()}"</br>
    /// <br>() => '.card'</br>
    /// <br>() => '.my-selector'</br>
    /// </remarks>
    public Func<string> WatcherSelector { get; set; } = () => ".card, .card-body, .modal, .content";

    #endregion
}
