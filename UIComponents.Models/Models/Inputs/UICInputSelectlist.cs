using Microsoft.AspNetCore.Mvc.Rendering;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Inputs;

public class UICInputSelectlist : UICInput<string>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired;
    public override string RenderLocation
    { 
        get
        {
            var renderer = Renderer;
            if (SearchableIfMinimimResults<= SelectListItems.Count)
                renderer = SelectListRenderer.Select2;
            return this.CreateDefaultIdentifier(renderer);
        }
    }
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
    /// Get a searchfield inside the if there are at least this numer of selectlistitems.
    /// </summary>
    /// <remarks>
    /// 0 => always on
    /// <br> -1 => never on</br></remarks>
    public int SearchableIfMinimimResults { get; set; } = -1;

    /// <summary>
    /// If the user has permission, show a button to add a item
    /// </summary>
    public bool AllowButtonAdd { get; set; }

    public List<SelectListItem> SelectListItems { get; set; }

    public bool ValidationRequired { get; set; }

    /// <summary>
    /// If no <see cref="SelectListItems"/> are available, A item is generated with this text
    /// </summary>
    public Translatable NoItemsText { get; set; } = TranslationDefaults.SelectListNoItems;


    /// <summary>
    /// Action that is triggered on opening the selectlist
    /// </summary>
    public IUIAction? OnListOpen { get; set; }

    public SelectListRenderer Renderer { get; set; } = SelectListRenderer.Select2;
    #endregion


    public enum SelectListRenderer
    {
        Default,
        Select2
    }
}
