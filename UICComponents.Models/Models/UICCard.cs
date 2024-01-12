namespace UIComponents.ComponentModels.Models;

public class UICCard : UIComponent, IUITabCard, IUICHasChildren<IUIComponent>
{

    #region Ctor
    public UICCard() : base()
    {

    }
    #endregion

    #region Properties
    /// <summary>
    /// These are all the elements displayed in this card
    /// </summary>
    public List<IUIComponent> Children { get; set; } = new();

    public List<UICButton> CardButtons { get; set; } = new();

    /// <summary>
    /// Hide this card, but still display its <see cref="Children"/>
    /// </summary>
    public bool OnlyDisplayContent { get; set; }

    /// <summary>
    /// Title for this card (hides header if no header is selected
    /// </summary>
    public ITranslationModel Title { get; set; }

    public IColor? HeaderColor { get; set; } = Colors.CardHeaderDefault;

    /// <summary>
    /// If this card has a title, this property can set a card as closed by default.
    /// </summary>
    public bool DefaultClosed { get; set; }

    /// <summary>
    /// If not empty, set this as the minimum width of the card
    /// </summary>
    public string MinWidth { get; set; } = "fit-content";

    /// <summary>
    /// If not empty, set this as the maximum width of the card
    /// </summary>
    public string MaxWidth { get; set; }

    /// <summary>
    /// If not empty, this source will replace the card elements.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Load this card when it is loaded on the page, even when closed
    /// </summary>
    public bool LoadSourceOnClosed { get; set; }

    /// <summary>
    /// When a card is open, reload the source.
    /// </summary>
    /// <remarks>
    /// if the card is empty, it should always load the source
    /// </remarks>
    public bool LoadSourceOnOpen { get; set; } = true;


    #endregion

    #region Methods


    public UICCard AddButton<T>(T button) where T : UICButton
    {
        CardButtons.Add(button);
        return this;
    }
    public UICCard AddButton<T>(out T addedButton, T button) where T: UICButton
    {
        addedButton= button;
        return AddButton(button);
    }

    #endregion

}


