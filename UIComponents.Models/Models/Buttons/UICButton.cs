namespace UIComponents.Models.Models.Buttons;

[PrependAppendInputGroupClass("")]
public class UICButton : UIComponent
{
    #region Fields
    public override string RenderLocation => UIComponent.DefaultIdentifier("Button", Renderer);
    #endregion

    #region Ctor
    public UICButton(ITranslateable text) : this()
    {
        ButtonText = text;
    }

    public UICButton() : base()
    {

    }
    #endregion

    #region Properties

    public ITranslateable ButtonText { get; set; }

    public ITranslateable Tooltip { get; set; }

    public IColor? Color { get; set; } = ColorDefaults.ButtonDefault;

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

