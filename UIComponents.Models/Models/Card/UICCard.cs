using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Buttons;

namespace UIComponents.Models.Models.Card;

public class UICCard : UIComponent
{

    #region Ctor
    public UICCard(ITranslateable title) : this(new UICCardHeader() { Title = title})
    {
    }

    public UICCard(IHeader header) : this()
    {
        Header = header;
    }
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

    public UICGroup? Footer { get; set; }



    /// <summary>
    /// If this card has a title, this property can set a card as closed by default.
    /// </summary>
    public bool DefaultClosed { get; set; }

    public bool DisableClosing { get; set; }

    /// <summary>
    /// Store the collapsed state of a card in local storage. Next time the user visits this page it will remember if the card was collapsed or not
    /// </summary>
    /// <remarks>
    /// This only works if the card has a Id Assigned => card.AddAttribute("id", "myId")
    /// </remarks>
    public bool RememberCollapsedState { get; set; } = true;

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

    /// <summary>
    /// If the footer does not exist, create the footer
    /// </summary>
    public UICGroup AddFooter(UICGroup? footer = null) 
    {
        if (Footer != null)
            return Footer;

        if(footer == null)
            footer = new UICGroup();

        Footer = footer;
        return Footer;
    }

    public T AddHeader<T>(T header = null) where T: class, IHeader
    {
        if(Header != null)
        {
            if(Header is T)
                return (T)Header;
            throw new Exception($"Header already exists and is not assignable to {typeof(T).Name}");
        }

        header = header ?? default(T);
        Header = header;
        return (T)Header;
    }
    public UICCardHeader AddHeader(UICCardHeader? header = null)
    {
        return AddHeader<UICCardHeader>(header);
    }
    #endregion

}


