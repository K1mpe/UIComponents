using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Buttons;

namespace UIComponents.Models.Models.Card;

public class UICCard : UIComponent, IUICCardLike
{

    #region Ctor
    public UICCard(Translatable title) : this(new UICCardHeader() { Title = title})
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
    /// <summary>
    /// The header of the card, <see cref="UICCardHeader"/> is most used for this.
    /// </summary>
    public IHeader Header { get; set; }

    /// <summary>
    /// These are all the elements displayed in this card
    /// </summary>
    public UICGroup Body { get; set; } = new();

    public UICGroup Footer { get; set; } = new();


    /// <summary>
    /// Do not display the header for this card. Header can still be used for tabs
    /// </summary>
    public bool HideHeader { get; set; }    

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


    IUICHasAttributesAndChildren IUICCardLike.Content => Body;

    IUICHasAttributesAndChildren IUICCardLike.Footer => Footer;
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
        Body.Add(out added, component);
        return this;
    }

    public UICCard Add<T>(T component, Action<T> configure) where T: IUIComponent
    {
        Body.Add(component, configure);
        return this;
    }

    #region AddHeader
    private T CreateHeader<T>(T header = null) where T: class, IHeader
    {
        if (Header != null)
        {
            if (Header is T)
                return (T)Header;
            throw new Exception($"Header already exists and is not assignable to {typeof(T).Name}");
        }

        header = header ?? Activator.CreateInstance<T>();
        Header = header;
        return header;
    }

    /// <summary>
    /// Get the current header or create a new header, ouput this header
    /// </summary>
    public UICCard AddHeader<T>(out T addedHeader, T header = null) where T : class, IHeader
    {
        addedHeader = CreateHeader(header);
        return this;
    }


    /// <summary>
    /// If the header does not exist yet, create this header. also configure the header
    /// </summary>
    public UICCard AddHeader<T>(T header, Action<T> configure) where T : class, IHeader
    {
        var created = CreateHeader(header);
        configure(created);
        return this;
    }

    /// <inheritdoc cref="AddHeader{T}(out T, T)"/>
    public UICCard AddHeader(out UICCardHeader addedHeader)
    {
        return AddHeader(out addedHeader, null);
    }

    /// <inheritdoc cref="AddHeader{T}(T, Action{T})"/>
    public UICCard AddHeader(Action<UICCardHeader> configure)
    {
        return AddHeader<UICCardHeader>(null, configure);
    }

    #endregion

    #region AddFooter

    /// <summary>
    /// If the footer does not exist, create the footer
    /// </summary>
    private UICGroup CreateFooter(UICGroup? footer = null)
    {
        if (Footer != null)
            return Footer;

        if (footer == null)
            footer = new UICGroup();

        Footer = footer;
        return Footer;
    }

    public UICCard AddFooter(IUIComponent component)
    {
        CreateFooter().Add(component);
        return this;
    }

    /// <summary>
    /// Add a item to the footer and ouput this item
    /// </summary>
    public UICCard AddFooter<T>(out T addedToFooter, T element) where T : IUIComponent
    {
        CreateFooter().Add(out addedToFooter, element);
        return this;
    }

    /// <summary>
    /// Add a item to the footer and configure the added item
    /// </summary>
    public UICCard AddFooter<T>(T addingToFooterElement, Action<T> configure) where T : IUIComponent
    {
        CreateFooter().Add(addingToFooterElement, configure);
        return this;
    }

    #endregion


    #region AddPartial
    /// <summary>
    /// Add a partial to the card and also add a refresh button to the header
    /// </summary>
    public UICCard AddPartial(UICPartial partial)
    {
        Body.Add(partial);

        try
        {
            AddHeader(h => h.AddButton(new UICButtonRefreshPartial(partial)));
        }
        catch{ }

        return this;
    }

    public UICCard AddPartial(string controller, string action, object data = null)
    {
        return AddPartial(new(controller, action, data));
    }

    /// <summary>
    /// Add a partial to the card and also add a refresh button to the header
    /// </summary>
    public UICCard AddPartial(out UICPartial addedPartial, UICPartial partial)
    {
        addedPartial = partial;
        return AddPartial(partial);
    }
    #endregion
    #endregion

}


