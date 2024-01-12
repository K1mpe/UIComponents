namespace UIComponents.Generators.Models;

/// <summary>
/// A override for the UICOptions with diffrent presets for rendering a entire class object
/// </summary>
public class UICClassOptions : UICOptions
{
    public UICClassOptions()
    {
        InputGroupSingleRow = true;
        HideEmptyInReadonly = true;
    }
}

public class UICCreateOptions : UICOptions
{
    public UICCreateOptions()
    {
        ReplaceSaveButtonWithCreateButton = true;
        InputGroupSingleRow = true;
        ShowEditButton = false;
        ShowDeleteButton = false;
        ShowCancelButton = true;
        IdHidden = true;
    }
}

/// <summary>
/// This is a options object used to create new IUElements.
/// </summary>
public class UICOptions
{
    #region Overrides
    public List<IUICGenerator> Generators { get; set; } = new();

    /// <summary>
    /// A dictionary with additional options you can use for custom Generators
    /// </summary>
    public Dictionary<string, object> AdditionalOptions { get; set; } = new();

    #endregion

    #region Properties

    /// <summary>
    /// A dictionary where you can add all sort of options that can be used by the generators
    /// </summary>
    public Dictionary<string, object> OptionsDictionary { get; set; } = new();

    /// <summary>
    /// Hide all properties with name "Id"
    /// </summary>
    public bool IdHidden { get; set; } = true;

    /// <summary>
    /// Hide all readonly properties
    /// </summary>
    public bool HideReadonlyProperties { get; set; }

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
    /// A opitional list of excluded properties.
    /// </summary>
    /// <remarks>
    /// Default: "IsDeleted"
    /// </remarks>
    public string ExcludedProperties { get; set; } = "IsDeleted";

    #endregion

    #region Form

    public bool FormReadonly { get; set; }

    /// <summary>
    /// If the form is in readonly modus, hide all inputs that do not have a value
    /// </summary>
    public bool HideEmptyInReadonly { get; set; }

    public string FormPostUrl { get; set; }

    /// <summary>
    /// Replace the Save button with a Create button
    /// </summary>
    public bool ReplaceSaveButtonWithCreateButton { get; set; }

    /// <summary>
    /// As long as there are validation errors, disable the save button.
    /// </summary>
    public bool DisableSaveButtonOnValidationErrors { get; set; } = true;

    #region Buttons
    public ButtonPosition ButtonPosition { get; set; } = ButtonPosition.Right;
    public ButtonDistance ButtonDistance { get; set; } = ButtonDistance.Medium;

    public bool ReverseButtonOrder { get; set; }

    public bool ShowEditButton { get; set; } = true;
    public bool ShowDeleteButton { get; set; } = true;

    public bool ShowCancelButton { get; set; }
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
    public bool ShowAddButtonIfAllowed { get; set; } = true;

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
