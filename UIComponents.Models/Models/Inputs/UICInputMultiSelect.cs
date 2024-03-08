using Microsoft.AspNetCore.Mvc.Rendering;
using UIComponents.Abstractions.Interfaces.Inputs;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Extensions;

namespace UIComponents.Models.Models.Inputs;

public class UICInputMultiSelect : UICInput<string[]>, IUICInputSelectList
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

    #region Methods

    public UICInputMultiSelect AddSource(UICActionGetPost source)
    {
        var selectSource = new UICInputSelectListSource(this, source);
        AddSource(selectSource);
        return this;
    }
    public UICInputMultiSelect AddSource(out UICInputSelectListSource outSource, UICActionGetPost source)
    {
        var selectSource = new UICInputSelectListSource(this, source);
        return AddSource(out outSource, selectSource);
    }
    public UICInputMultiSelect AddSource(UICActionGetPost source, Action<UICInputSelectListSource> action)
    {
        var selectSource = new UICInputSelectListSource(this, source);
        return AddSource(selectSource, action);
    }
    public UICInputMultiSelect AddSource(UICInputSelectListSource source)
    {
        source.InputSelectList = this;
        ScriptCollection.AddToScripts(source);
        return this;
    }
    public UICInputMultiSelect AddSource(out UICInputSelectListSource outSource, UICInputSelectListSource source)
    {
        outSource = source;
        return AddSource(source);
    }
    public UICInputMultiSelect AddSource(UICInputSelectListSource source, Action<UICInputSelectListSource> action)
    {
        AddSource(source);
        action(source);
        return this;
    }

    #endregion

    #region From Interface

    object IUICInputSelectList.Value => Value;

    #endregion

}
