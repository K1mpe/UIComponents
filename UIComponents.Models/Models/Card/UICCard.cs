using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Buttons;

namespace UIComponents.Models.Models.Card;

public class UICCard : UIComponent
{

    #region Ctor
    public UICCard() : base()
    {

    }
    #endregion

    #region Properties
    public IHeader Header { get; set; }

    /// <summary>
    /// These are all the elements displayed in this card
    /// </summary>
    public UICGroup Body { get; set; } = new();

    public UICGroup Footer { get; set; }



    /// <summary>
    /// If this card has a title, this property can set a card as closed by default.
    /// </summary>
    public bool DefaultClosed { get; set; }

    public bool DisableClosing { get; set; }

    /// <summary>
    /// If not empty, set this as the minimum width of the card
    /// </summary>
    public string MinWidth { get; set; } = "fit-content";

    /// <summary>
    /// If not empty, set this as the maximum width of the card
    /// </summary>
    public string MaxWidth { get; set; }
    





    #endregion

    #region Methods

    /// <summary>
    /// Add a item to the body
    /// </summary>
    public UICCard Add(IUIComponent component)
    {
        Body.Add(component);
        return this;
    }

    /// <summary>
    /// Add a item to the body
    /// </summary>
    public UICCard Add<T>(out T added, T component) where T : IUIComponent
    {
        added = component;
        return Add(component);
    }



    #endregion

}


