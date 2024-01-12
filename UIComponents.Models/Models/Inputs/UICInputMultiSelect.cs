using System.Web.Mvc;
using UIComponents.Models.Abstract;

namespace UIComponents.Models.Models.Inputs;

public class UICInputMultiSelect : UICInput<string[]>
{
    #region Fields
    public override bool HasClientSideValidation => false;
    #endregion

    #region Ctor
    public UICInputMultiSelect(string propertyName, List<SelectListItem> selectListItems) : base(propertyName)
    {
        SelectListItems = selectListItems;
    }
    public UICInputMultiSelect() : this(null, new())
    {

    }
    #endregion

    #region Properties
    public IColor? Color { get; set; }

    public List<SelectListItem> SelectListItems { get; set; }

    /// <summary>
    /// When selecting a option, close the selectlist
    /// </summary>
    public bool CloseOnSelect { get; set; }

    #endregion

}
