
using UIComponents.Models.Models.Card;

namespace UIComponents.Defaults;

public static class OptionDefaults
{
    /// <inheritdoc cref="UICOptions.CheckReadPermissions" />
    public static bool CheckReadPermissions = true;

    /// <inheritdoc cref="UICOptions.CheckWritePermissions" />
    public static bool CheckWritePermissions = true;

    /// <inheritdoc cref="UICOptions.CheckClientSideValidation" />
    public static bool CheckClientSideValidation = true;

    /// <inheritdoc cref="UICOptions.DisableSaveButtonOnValidationErrors" />
    public static bool DisableSaveButtonOnValidationErrors = true;

    /// <inheritdoc cref="UICOptions.FormReadonly" />
    public static bool FormReadonly = false;

    /// <inheritdoc cref="UICOptions.FormToolbarInCardFooter" />
    public static bool FormToolbarInCardFooter = false;

    /// <inheritdoc cref="UICOptions.HideEmptyInReadonly" />
    public static bool HideEmptyInReadonly = false;

    /// <inheritdoc cref="UICOptions.HideReadonlyProperties" />
    public static bool HideReadonlyProperties = false;

    /// <inheritdoc cref="UICOptions.HideId" />
    public static bool HideId = true;

    ///<inheritdoc cref="UICOptions.CheckReadPermissions" />
    public static string ExcludedProperties = string.Empty;

    /// <inheritdoc cref="UICOptions.IncludedUndefinedProperties" />
    public static bool IncludedUndefinedProperties = false;

    /// <inheritdoc cref="UICOptions.InputGroupSingleRow" />
    public static bool InputGroupSingleRow = true;

    /// <inheritdoc cref="UICOptions.MarkLabelsAsRequired" />
    public static bool MarkLabelsAsRequired = true;

    ///<inheritdoc cref="UICOptions.NoForm" />
    public static bool NoForm = false;

    ///<inheritdoc cref="UICOptions.PostIdAsFixed" />
    public static bool PostIdAsFixed = false;

    ///<inheritdoc cref="UICOptions.PostObjectAsDefault" />
    public static bool PostObjectAsDefault = false;

    ///<inheritdoc cref="UICOptions.ReplaceSaveButtonWithCreateButton" />
    public static bool ReplaceSaveButtonWithCreateButton = false;

    ///<inheritdoc cref="UICOptions.SelectlistAddEmptyItem" />
    public static bool SelectlistAddEmptyItem = false;

    ///<inheritdoc cref="UICOptions.SelectListShowAddButtonIfAllowed" />
    public static bool SelectListShowAddButtonIfAllowed = false;

    ///<inheritdoc cref="UICOptions.SelectlistSearchableForItems" />
    public static int SelectlistSearchableForItems = 10;

    ///<inheritdoc cref="UICOptions.ShowCancelButton" />
    public static bool ShowCancelButton = false;

    ///<inheritdoc cref="UICOptions.ShowCardHeaders" />
    public static bool ShowCardHeaders = false;

    ///<inheritdoc cref="UICOptions.ShowDeleteButton" />
    public static bool ShowDeleteButton = true;

    ///<inheritdoc cref="UICOptions.ShowEditButton" />
    public static bool ShowEditButton = true;

    ///<inheritdoc cref="UICOptions.ButtonDistance" />
    public static ButtonDistance ButtonDistance = ButtonDistance.Medium;

    ///<inheritdoc cref="UICOptions.ButtonOrder"/>
    public static string ButtonOrder;

    public static Dictionary<string, Func<UICButtonToolbar, UICPropertyArgs, Task>> ButtonGenerators = new();
    ///<inheritdoc cref="UICOptions.ButtonPosition" />
    public static ButtonPosition? ButtonPosition = null;
    public static ButtonPosition? EditButtonPosition = null;
    public static ButtonPosition? DeleteButtonPosition = null;
    public static ButtonPosition? CancelButtonPosition = null;
    public static ButtonPosition? SaveButtonPosition = null;

    public static bool PostFullModelOnDelete = false;
    ///<inheritdoc cref="UICOptions.ToolbarPosition" />
    public static ToolbarPosition ToolbarPosition = ToolbarPosition.BelowForm;

    ///<inheritdoc cref="UICOptions.CheckboxColor" />
    public static Func<IColor?> CheckboxColor = null;

    ///<inheritdoc cref="UICOptions.OnSuccessfullSubmit" />
    public static Func<IUICAction> OnSuccessfullSubmit = ()=> new UICCustom();

    ///<inheritdoc cref="UICOptions.StartInCard" />
    public static UICCard? StartInCard = null;

    ///<inheritdoc cref="UICOptions.SubClassesInCard" />
    public static UICCard? SubClassesInCard = new();

}
