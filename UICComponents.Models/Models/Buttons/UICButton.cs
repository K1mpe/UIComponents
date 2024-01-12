namespace UIComponents.ComponentModels.Models.Buttons;

[PrependAppendInputGroupClass("")]
public class UICButton : UIComponent
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICButton(ITranslationModel text) : this()
    {
        ButtonText = text;
    }

    public UICButton() : base()
    {

    }
    #endregion

    #region Properties

    public ITranslationModel ButtonText { get; set; }

    public ITranslationModel Tooltip { get; set; }

    public IColor? Color { get; set; } = Colors.ButtonDefault;

    /// <summary>
    /// Function triggered when clicking the button
    /// </summary>
    /// <remarks>
    /// args => ClickEventArgs
    /// </remarks>
    public IUIAction OnClick { get; set; } = new UICCustom();

    public UICIcon PrependButtonIcon { get; set; }
    public UICIcon AppendButtonIcon { get; set; }

    public ButtonRenderer Renderer { get; set; } = ButtonRenderer.Default;
    public bool Disabled { get; set; }
    #endregion



    public enum ButtonRenderer
    {
        Default,
        CardButton
    }
}

