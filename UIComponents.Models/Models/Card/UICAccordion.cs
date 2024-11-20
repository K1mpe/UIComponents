namespace UIComponents.Models.Models.Card;

public class UICAccordion : UIComponent, IUICHasChildren<UICCard>, IUICSupportsTaghelperContent
{
    #region Properties

    public List<UICCard> Children { get; set; } = new List<UICCard>();

    /// <summary>
    /// Default close all cards in accordion, the only exception is if the urlHash contains the card Id
    /// </summary>
    /// <remarks>
    /// If <see cref="AllowOneCardOpen"/> is true, only first card is shown
    /// </remarks>
    public bool AllCardsClosedByDefault { get; set; }

    /// <summary>
    /// When opening a card, all other cards in the accordion will close
    /// </summary>
    public bool AllowOneCardOpen { get; set; }

    public bool RemoveMarginBetweenCards { get; set; }

    #endregion

    #region Methods
    public UICAccordion Add(out UICCard added, UICCard card)
    {
        return this.Add<UICAccordion, UICCard, UICCard>(out added, card);
    }

    public UICAccordion Add(UICCard card, Action<UICCard> configure)
    {
        return this.Add<UICAccordion, UICCard, UICCard>(card, configure);
    }
    #endregion


    bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;

    /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
    protected virtual async Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        var card = await UICCard.CreateFromContentAndAttributes(taghelperContent, attributes);
        
        this.Add(card);
    }
    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);

}
