namespace UIComponents.Models.Models.Buttons;

public class UICButtonEdit : UIComponent
{
    #region Ctor
    public UICButtonEdit() : base()
    {
        ButtonSetEdit = new UICButton()
        {
            ButtonText = TranslationDefaults.ButtonEdit,
            AppendButtonIcon = new UICIcon(IconDefaults.Edit?.Icon),
            OnClick = new UICActionSetEdit(),
        };
        ButtonSetReadonly = new UICButton()
        {
            ButtonText = TranslationDefaults.ButtonReadonly,
            AppendButtonIcon = new UICIcon(IconDefaults.CancelEdit?.Icon),
            OnClick = new UICActionSetReadonly(),
        };
    }

    /// <summary>
    /// Create a button to make a form readonly
    /// </summary>
    /// <param name="formId">example: #my-form</param>
    public UICButtonEdit(UICForm form) : this()
    {
        Form = form;
    }
    #endregion

    #region Properties

    public UICForm Form { get; set; }

    public bool ReadonlyOnLoad { get; set; } = true;

    public UICButton ButtonSetEdit { get; set; }
    public UICButton ButtonSetReadonly { get; set; }

    #endregion
}
