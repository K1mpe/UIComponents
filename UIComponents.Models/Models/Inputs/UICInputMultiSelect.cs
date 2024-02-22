using Microsoft.AspNetCore.Mvc.Rendering;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Extensions;

namespace UIComponents.Models.Models.Inputs;

public class UICInputMultiSelect : UICInput<string[]>
{
    #region Fields
    public override bool HasClientSideValidation => false;
    #endregion

    #region Ctor
    public UICInputMultiSelect(string propertyName, List<SelectListItem> selectListItems) : base(propertyName)
    {
        SelectListItems = selectListItems.ToUIC();
    }
    public UICInputMultiSelect() : base(null)
    {

    }
    #endregion

    #region Properties
    public IColor? Color { get; set; }

    public List<UICSelectListItem> SelectListItems { get; set; } = new();

    /// <summary>
    /// When selecting a item by typing, clear the input after the item is added
    /// </summary>
    public bool ClearInputAfterSelecting { get; set; } = true;

    /// <summary>
    /// When selecting a option, close the selectlist
    /// </summary>
    public bool CloseOnSelect { get; set; }


    /// <summary>
    /// If true, the user can create new options by typing
    /// </summary>
    public bool AllowDynamicOptions { get; set; }

    public Translatable NoItemsText { get; set; } = TranslationDefaults.SelectListNoItems;

    #endregion

}
