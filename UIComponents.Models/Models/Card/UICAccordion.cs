namespace UIComponents.Models.Models.Card;

public class UICAccordion : UIComponent, IUICHasChildren<UICCard>
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


}
