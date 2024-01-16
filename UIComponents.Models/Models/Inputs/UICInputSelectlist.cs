using System.Web.Mvc;
using UIComponents.Abstractions.Models;

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
    /// Get a searchfield inside the selectlist
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// If the user has permission, show a button to add a item
    /// </summary>
    public bool AllowButtonAdd { get; set; }

    public List<SelectListItem> SelectListItems { get; set; }

    public bool ValidationRequired { get; set; }

    /// <summary>
    /// If no <see cref="SelectListItems"/> are available, A item is generated with this text
    /// </summary>
    public ITranslateable NoItemsText { get; set; } = TranslationDefaults.SelectListNoItems;


    /// <summary>
    /// Action that is triggered on opening the selectlist
    /// </summary>
    public IUIAction? OnListOpen { get; set; }
    #endregion
}
