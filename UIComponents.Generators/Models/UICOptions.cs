using UIComponents.Defaults;
using UIComponents.Models.Models.Card;
using UIComponents.Models.Models.Inputs;

namespace UIComponents.Generators.Models;

/// <summary>
/// This is a options object used to create new IUElements.
/// </summary>
public class UICOptions
{
    #region Overrides
    public List<IUICGenerator> Generators { get; set; } = new();

    /// <summary>
    /// A dictionary where you can add all sort of options that can be used by the generators
    /// </summary>
    public Dictionary<string, object> OptionsDictionary { get; set; } = new();


    public UICOptions AddGenerator(IUICGenerator generator)
    {
        Generators.Add(generator);
        return this;
    }
    #endregion

    /// <summary>
    /// If this UIComponent can be generated from a controller, adding a partial to the path can prevent the entire page from reloading
    /// </summary>
    /// <remarks>
    /// This partial will automatically skip initial load & add a button in the first available card
    /// </remarks>
    public UICPartial? Partial { get; set; }

    #region Properties


    /// <<summary>
    /// Hide all properties with name "Id"
    /// </summary>>
    public bool HideId { get; set; } = OptionDefaults.HideId;

    /// <summary>
    /// Hide all readonly properties
    /// </summary>
    public bool HideReadonlyProperties { get; set; } = OptionDefaults.HideReadonlyProperties;

    /// <summary>
    /// A list with properties shown in the form, this is not case-sensitive.
    /// <br>The UI will also place the properties in this order</br>
    /// <br>Example: "name, description, created"</br>
    /// </summary>
    /// <remarks>
    /// Default: All properties from this type in order as set in the type.
    /// </remarks>
    public string IncludedProperties { get; set; }

    /// <summary>
    /// If true, add all undefined properties after the included properties. This does not include <see cref="ExcludedProperties"/>
    /// </summary>
    /// <remarks>
    /// Is always done if <see cref="IncludedProperties"/> is empty</remarks>
    public bool IncludedUndefinedProperties { get; set; } = OptionDefaults.IncludedUndefinedProperties;

    /// <summary>
    /// A opitional list of excluded properties.
    /// </summary>
    /// <remarks>
    /// Default: "IsDeleted"
    /// </remarks>
    public string ExcludedProperties { get; set; } = OptionDefaults.ExcludedProperties;


    #endregion

    #region Form

    /// <summary>
    /// If true, there will be no form created. Usefull for a subform or something that does not need to be posted 
    /// </summary>
    public bool NoForm { get; set; } = OptionDefaults.NoForm;

    /// <summary>
    /// The form is only readonly
    /// </summary>
    public bool FormReadonly { get; set; } = OptionDefaults.FormReadonly;

    /// <summary>
    /// If the form is in readonly modus, hide all inputs that do not have a value
    /// </summary>
    public bool HideEmptyInReadonly { get; set; } = OptionDefaults.HideEmptyInReadonly;

    /// <summary>
    /// The submit action when posting the form. Can be a custom action, but <see cref="UICActionGetPost"/> is prefered
    /// </summary>
    public ISubmitAction PostForm { get; set; }

    /// <summary>
    /// The action that should occur when posting the form successfully. may not work with a custom <see cref="PostForm"/>
    /// </summary>
    /// <remarks>
    /// result is available as argument, can be changed if <see cref="UICActionGetPost.ResultName"/> is changed</remarks>
    public IUICAction OnSuccessfullSubmit { get; set; } = OptionDefaults.OnSuccessfullSubmit;
    
    /// <summary>
    /// Post all properties as <see cref="UICActionGetPost.DefaultData" />. This means that properties that are not visualised, will still post their current value.
    /// </summary>
    public bool PostObjectAsDefault { get; set; } = OptionDefaults.PostObjectAsDefault;

    /// <summary>
    /// Post the Id property as <see cref="UICActionGetPost.FixedData"/>. This means this cannot be overwritten clientside
    /// </summary>
    /// <remarks>
    /// Warning! Only works on classes that inherit from <see cref="IDbEntity"/></remarks>
    public bool PostIdAsFixed { get; set; } = OptionDefaults.PostIdAsFixed;

    /// <summary>
    /// Replace the <see cref="UIComponents.Defaults.TranslationDefaults.ButtonSave"/> to <see cref="UIComponents.Defaults.TranslationDefaults.ButtonCreate"/>
    /// </summary>
    public bool ReplaceSaveButtonWithCreateButton { get; set; } = OptionDefaults.ReplaceSaveButtonWithCreateButton;

    /// <summary>
    /// Create a card inside the form and place its toolbar in the footer of the card
    /// </summary>
    public bool FormToolbarInCardFooter { get; set; } = OptionDefaults.FormToolbarInCardFooter;

    /// <summary>
    /// As long as there are validation errors clientside, disable the save button.
    /// </summary>
    public bool DisableSaveButtonOnValidationErrors { get; set; } = OptionDefaults.DisableSaveButtonOnValidationErrors;

    #region Buttons
    public ToolbarPosition ToolbarPosition { get; set; } = OptionDefaults.ToolbarPosition;

    /// <summary>
    /// Where in the toolbar are the generated buttons placed?
    /// </summary>
    public ButtonPosition ButtonPosition { get; set; } = OptionDefaults.ButtonPosition;
    public ButtonDistance ButtonDistance { get; set; } = OptionDefaults.ButtonDistance;

    /// <summary>
    /// reverse the order of the buttons in the toolbar
    /// </summary>
    public bool ReverseButtonOrder { get; set; } = OptionDefaults.ReverseButtonOrder;

    /// <summary>
    /// This button makes the form readonly at first, when clicking the edit button it transforms to a editable version.
    /// </summary>
    public bool ShowEditButton { get; set; } = OptionDefaults.ShowEditButton;

    /// <summary>
    /// Show the delete button in the toolbar.
    /// </summary>
    /// <remarks>
    /// The default button only available for <see cref="IDbEntity"/>
    /// </remarks>
    public bool ShowDeleteButton { get; set; } = OptionDefaults.ShowDeleteButton;

    /// <summary>
    /// The cancel button tries to close a modal, if this does not work it will go back to the previous page
    /// </summary>
    public bool ShowCancelButton { get; set; } = OptionDefaults.ShowCancelButton;
    #endregion
    #endregion

    #region Card
    /// <summary>
    /// Show or hide all the headers of all the cards
    /// </summary>
    public bool ShowCardHeaders { get; set; } = OptionDefaults.ShowCardHeaders;

    /// <summary>
    /// When providing a card here, the component is placed inside this card as a start
    /// </summary>
    public UICCard? StartInCard { get; set; } = OptionDefaults.StartInCard;

    /// <summary>
    /// When a subclass needs to be rendered, it will be rendered inside this card.
    /// </summary>
    public UICCard? SubClassesInCard { get; set; } = OptionDefaults.SubClassesInCard;

    /// <summary>
    /// Override the name of the subcards with one of these options
    /// </summary>
    public CardTitleOverride SubCardTitleOverride { get; set; } = CardTitleOverride.ClassType;

    #endregion

    #region Checkbox
    /// <summary>
    /// The default color assigned to all checkboxes and toggleswitches
    /// </summary>
    public IColor? CheckboxColor { get; set; } = OptionDefaults.CheckboxColor;

    /// <summary>
    /// This is used for all checkboxes and checkboxthreestates
    /// </summary>
    public CheckboxRenderer CheckboxRenderer { get; set; } = CheckboxRenderer.ToggleSwitch;
    #endregion

    #region SelectList
    /// <summary>
    /// Add a default item for selectlistitems
    /// </summary>
    public bool SelectlistAddEmptyItem { get; set; } = OptionDefaults.SelectlistAddEmptyItem;

    /// <summary>
    /// If a selectlist has more or equal as this amount of items, it will be searchable
    /// </summary>
    public int SelectlistSearableForItems { get; set; } = OptionDefaults.SelectlistSearableForItems;

    /// <summary>
    /// If the user has permission to add this type of entity, show the add button
    /// </summary>
    public bool SelectListShowAddButtonIfAllowed { get; set; } = OptionDefaults.SelectListShowAddButtonIfAllowed;

    #endregion

    public UICDatetimeStep DatetimePrecision { get; set; } = UICDatetimeStep.Minute;
    public UICTimeonlyEnum TimeOnlyPrecision { get; set; } = UICTimeonlyEnum.Minute;
    public bool InputGroupSingleRow { get; set; } = OptionDefaults.InputGroupSingleRow;

    /// <summary>
    /// If a input is required, visualise this on the label with a red *
    /// </summary>
    public bool MarkLabelsAsRequired { get; set; } = OptionDefaults.MarkLabelsAsRequired;

    public bool CheckReadPermissions { get; set; } = OptionDefaults.CheckReadPermissions;
    public bool CheckWritePermissions { get; set; } = OptionDefaults.CheckWritePermissions;

    public UICOptions AddDictionaryOption(string key, object value)
    {
        OptionsDictionary.Add(key, value);
        return this;
    }

}
public enum ButtonPosition
{
    Left,
    Center,
    Right,
}
public enum ToolbarPosition
{
    AboveForm,
    BelowForm
}
public enum CardTitleOverride
{
    /// <summary>
    /// Do not overwrite the title of the card
    /// </summary>
    NoOverride,

    /// <summary>
    /// Get the translated Name or ToString()
    /// </summary>
    ClassTranslatedNameOrTostring,

    /// <summary>
    /// Get the Tostring as untranslateable title
    /// </summary>
    ClassToString,

    /// <summary>
    /// Get the classtype.Name as title
    /// </summary>
    ClassType,

    PropertyName
}
