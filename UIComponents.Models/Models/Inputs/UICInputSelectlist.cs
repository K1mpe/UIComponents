using System.Web.Mvc;
using UIComponents.Models.Abstract;

namespace UIComponents.Models.Models.Inputs;

public class UICInputSelectlist : UICInput<string>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired;

    #endregion

    #region Ctor
    public UICInputSelectlist() : this(null, new())
    {

    }
    public UICInputSelectlist(string propertyName, List<SelectListItem> selectListItems) : base(propertyName)
    {
        SelectListItems = selectListItems;
    }
    #endregion

    #region Properties

    /// <summary>
    /// The type of the items.
    /// </summary>
    /// <remarks>
    /// This is used for permissionchecks & the AllowButtonAdd
    /// </remarks>
    public Type ItemType { get; set; }

    /// <summary>
    /// Get a searchfield inside the selectlist
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// If the user has permission, show a button to add a item
    /// </summary>
    public bool AllowButtonAdd { get; set; }

    public List<SelectListItem> SelectListItems { get; set; }

    public bool ValidationRequired { get; set; }

    #endregion
}
