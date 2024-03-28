using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// This will disable the selected button as long as there are any validation errors on the form.
/// <br>If no form is provided, the entire page needs to be free of validation errors</br>
/// </summary>
public class UICActionDisableSaveButtonOnValidationErrors : UIComponent, IUICAction
{

    #region Ctor
    public UICActionDisableSaveButtonOnValidationErrors()
    {

    }

    public UICActionDisableSaveButtonOnValidationErrors(UIComponent saveButton, UIComponent form = null)
    {
        Form = form;
        SaveButton = saveButton;
    }
    #endregion

    #region Properties

    /// <summary>
    /// This is the form or collection of properties that contains the validation errors.
    /// </summary>
    /// <remarks>
    /// If null, the entire page cannot contain any validation errors
    /// </remarks>
    [UICIgnoreGetChildrenFunction]
    public UIComponent Form { get; set; }


    /// <summary>
    /// This is the button that gets disabled on validation errors
    /// </summary>
    [UICIgnoreGetChildrenFunction]
    public UIComponent SaveButton { get; set; }
    #endregion
}
