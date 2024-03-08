namespace UIComponents.Models.Models.Buttons;

[PrependAppendInputGroupClass("input-group-button")]
public class UICButton : UIComponent
{
    #region Fields
    public override string RenderLocation => UIComponent.DefaultIdentifier("Button", Renderer);
    #endregion

    #region Ctor
    public UICButton(Translatable text) : this()
    {
        ButtonText = text;
    }

    public UICButton() : base()
    {

    }
    #endregion

    #region Properties

    public Translatable ButtonText { get; set; }

    public Translatable Tooltip { get; set; }

    public IColor? Color { get; set; } = ColorDefaults.ButtonDefault;

    /// <summary>
    /// Function triggered when clicking the button
    /// </summary>
    /// <remarks>
    /// ev => ClickEventArgs
    /// </remarks>
    public IUIAction OnClick { get; set; } = new UICCustom();

    public UICIcon PrependButtonIcon { get; set; }
    public UICIcon AppendButtonIcon { get; set; }

    public ButtonRenderer Renderer { get; set; } = ButtonRenderer.Default;
    public bool Disabled { get; set; }
    #endregion

    #region Converters
    public UICDropdownItem ConvertToDropdownItem()
    {
        var dropdownItem = CommonHelper.Convert<UICDropdownItem>(this);
        dropdownItem.Content = ButtonText;
        dropdownItem.Icon = PrependButtonIcon;
        dropdownItem.OnClick= OnClick;
        return dropdownItem;
    }
    #endregion


    public enum ButtonRenderer
    {
        Default,
        CardButton
    }
}

