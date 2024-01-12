using UIComponents.Models.Abstract;
using UIComponents.Models.Defaults;
using UIComponents.Models.Extensions;

namespace UIComponents.Models.Models.Buttons;

public class UICButtonEdit : UIComponent
{
    #region Ctor
    public UICButtonEdit() : base()
    {
        ButtonSetEdit = new()
        {
            ButtonText = TranslationDefaults.ButtonEdit,
            OnClick = new UICActionSetEdit(),
        };
        ButtonSetReadonly = new()
        {
            ButtonText = TranslationDefaults.ButtonReadonly,
            OnClick = new UICActionSetReadonly(),
        };
    }

    /// <summary>
    /// Create a button to make a form readonly
    /// </summary>
    /// <param name="formId">example: #my-form</param>
    public UICButtonEdit(string formId) : this()
    {
        this.AddAttribute("formidentifier", formId);
    }
    #endregion

    #region Properties

    public bool ReadonlyOnLoad { get; set; } = true;

    public UICButton ButtonSetEdit { get; set; }
    public UICButton ButtonSetReadonly { get; set; }

    #endregion
}
