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

    #endregion

    /// <summary>
    /// If this UIComponent can be generated from a controller, adding a partial to the path can prevent the entire page from reloading
    /// </summary>
    /// <remarks>
    /// This partial will automatically skip initial load & add a button in the first available card
    /// </remarks>
    public UICPartial? Partial { get; set; }

    #region Properties


    /// <summary>
    /// Hide all properties with name "Id"
    /// </summary>
    public bool IdHidden { get; set; } = OptionDefaults.IdHidden;

    /// <summary>
    /// Hide all readonly properties
    /// </summary>
    public bool HideReadonlyProperties { get; set; } = OptionDefaults.HidereadonlyProperties;

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
    public bool IncludedUndefinedProperties { get; set; }

    /// <summary>
    /// A opitional list of excluded properties.
    /// </summary>
    /// <remarks>
    /// Default: "IsDeleted"
    /// </remarks>
    public string ExcludedProperties { get; set; } = OptionDefaults.ExcludedProperties;

    /// <summary>
    /// Auto exclude all properties that are included in the <see cref="PostForm"/>.<see cref="UICActionGetPost.Data"/>
    /// </summary>
    public bool ExcludePostDataProperties { get; set; } = OptionDefaults.ExcludePostDataProperties;

    #endregion

    #region Form

    public bool NoForm { get; set; }

    public bool FormReadonly { get; set; }

    /// <summary>
    /// If the form is in readonly modus, hide all inputs that do not have a value
    /// </summary>
    public bool HideEmptyInReadonly { get; set; } = OptionDefaults.HideEmptyInReadonly;

    public ISubmitAction PostForm { get; set; }

    /// <summary>
    /// Replace the Save button with a Create button
    /// </summary>
    public bool ReplaceSaveButtonWithCreateButton { get; set; }

    /// <summary>
    /// Create a card inside the form and place its toolbar in the footer of the card
    /// </summary>
    public bool FormToolbarInCardFooter { get; set; }

    /// <summary>
    /// As long as there are validation errors, disable the save button.
    /// </summary>
    public bool DisableSaveButtonOnValidationErrors { get; set; } = OptionDefaults.DisableSaveButtonOnValidationErrors;

    #region Buttons
    public ButtonPosition ButtonPosition { get; set; } = OptionDefaults.ButtonPosition;
    public ButtonDistance ButtonDistance { get; set; } = OptionDefaults.ButtonDistance;

    public bool ReverseButtonOrder { get; set; } = OptionDefaults.ReverseButtonOrder;

    public bool ShowEditButton { get; set; } = OptionDefaults.ShowEditButton;
    public bool ShowDeleteButton { get; set; } = OptionDefaults.ShowDeleteButton;

    public bool ShowCancelButton { get; set; } = OptionDefaults.ShowCancelButton;
    #endregion
    #endregion

    #region Card
    public bool ShowCardHeaders { get; set; }
    public UICCard? StartInCard { get; set; } 
    public CardTitleOverride StartCardTitleOverride { get; set; }

    public UICCard? SubClassesInCard { get; set; } = new();
    public CardTitleOverride SubCardTitleOverride { get; set; } = CardTitleOverride.ClassType;

    #endregion

    #region Checkbox
    /// <summary>
    /// The default color assigned to all checkboxes and toggleswitches
    /// </summary>
    public IColor? CheckboxColor { get; set; }

    /// <summary>
    /// This is used for all checkboxes & checkboxthreestates
    /// </summary>
    public CheckboxRenderer CheckboxRenderer { get; set; } = CheckboxRenderer.ToggleSwitch;
    #endregion

    #region SelectList
    /// <summary>
    /// Add a default item for selectlistitems
    /// </summary>
    public bool SelectlistAddEmptyItem { get; set; } = true;

    /// <summary>
    /// If a selectlist has more or equal as this amount of items, it will be searchable
    /// </summary>
    public int SelectlistSearableForItems { get; set; } = 10;

    /// <summary>
    /// If the user has permission to add this type of entity, show the add button
    /// </summary>
    public bool SelectListShowAddButtonIfAllowed { get; set; } = true;

    /// <summary>
    /// Example: User.Field.UserGroupId => User.Field.UserGroup
    /// </summary>
    public bool RemoveIdFromTranslationKey { get; set; } = true;

    #endregion

    public UICDatetimeStep DatetimePrecision { get; set; } = UICDatetimeStep.Minute;
    public bool InputGroupSingleRow { get; set; } = true;
    public bool MarkLabelsAsRequired { get; set; } = true;

    public bool CheckReadPermissions { get; set; } = true;
    public bool CheckWritePermissions { get; set; } = true;

    public bool EnableDefaultTooltipText { get; set; }
    public bool EnableDefaultInfoSpanText { get; set; } = true;
}
public enum ButtonPosition
{
    Left,
    Center,
    Right,
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
