namespace UIComponents.Models.Models.Card;

public class UICModal: UIComponent
{
    #region Ctor
    public UICModal(Translatable title) : this(new UICCardHeader() { Title = title })
    {
    }

    public UICModal(IHeader header) : this()
    {
        Header = header;
    }
    public UICModal():base()
    {
        
    }
    #endregion

    #region Properties
    public IHeader Header { get; set; }

    public UICGroup Body { get; set; } = new();

    public UICGroup? Footer { get; set; }

    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Move the content out of its current location and place it on the body
    /// </summary>
    public bool MoveModalToBody { get; set; }

    public bool DisableCloseOnClickout { get; set; }
    public bool DisableEscapeKeyToClickout { get; set; } 

    /// <summary>
    /// Open the modal as soon as this is loaded on the page
    /// </summary>
    public bool OpenOnLoad { get; set; } = true;

    /// <summary>
    /// When the modal is closed, remove the html from the page.
    /// </summary>
    public bool RemoveModalOnClose { get; set; } = true;

    public ModalSize Width { get; set; } = ModalSize.Auto;
    
    #endregion

    #region Methods

    /// <summary>
    /// Add a item to the body
    /// </summary>
    public UICModal Add(IUIComponent component)
    {
        Body.Add(component);
        return this;
    }

    /// <summary>
    /// Add a item to the body
    /// </summary>
    public UICModal Add<T>(out T added, T component) where T : IUIComponent
    {
        Body.Add(out added, component);
        return this;
    }

    public UICModal Add<T>(T component, Action<T> configure) where T : IUIComponent
    {
        Body.Add(component, configure);
        return this;
    }

    #region AddHeader
    private T CreateHeader<T>(T header = null) where T : class, IHeader
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
    public UICModal AddHeader<T>(out T addedHeader, T header = null) where T : class, IHeader
    {
        addedHeader = CreateHeader(header);
        return this;
    }


    /// <summary>
    /// If the header does not exist yet, create this header. also configure the header
    /// </summary>
    public UICModal AddHeader<T>(T header, Action<T> configure) where T : class, IHeader
    {
        var created = CreateHeader(header);
        configure(created);
        return this;
    }

    /// <inheritdoc cref="AddHeader{T}(out T, T)"/>
    public UICModal AddHeader(out UICCardHeader addedHeader)
    {
        return AddHeader(out addedHeader, null);
    }

    /// <inheritdoc cref="AddHeader{T}(T, Action{T})"/>
    public UICModal AddHeader(Action<UICCardHeader> configure)
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

    /// <summary>
    /// Get the current footer or create a new footer, ouput this footer
    /// </summary>
    public UICModal AddFooter(out UICGroup addedFooter, UICGroup footer = null)
    {
        addedFooter = CreateFooter(footer);
        return this;
    }

    /// <summary>
    /// If the footer does not exist yet, create this footer. also configure the footer
    /// </summary>
    public UICModal AddFooter(Action<UICGroup> configure)
    {
        var footer = CreateFooter(null);
        configure(footer);
        return this;
    }

    #endregion

    #region AddPartial
    /// <summary>
    /// Add a partial to the card and also add a refresh button to the header
    /// </summary>
    public UICModal AddPartial(UICPartial partial)
    {
        Body.Add(partial);

        try
        {
            AddHeader(h => h.AddButton(new UICButtonRefreshPartial(partial)));
        }
        catch { }

        return this;
    }

    public UICModal AddPartial(string controller, string action, object data = null)
    {
        return AddPartial(new(controller, action, data));
    }

    /// <summary>
    /// Add a partial to the card and also add a refresh button to the header
    /// </summary>
    public UICModal AddPartial(out UICPartial addedPartial, UICPartial partial)
    {
        addedPartial = partial;
        return AddPartial(partial);
    }
    #endregion
    #endregion

    #region Triggers

    public IUIAction TriggerOpen()
    {
        return new UICCustom($"$('#{this.GetId()}').trigger('uic-open');");
    }

    public IUIAction TriggerClose()
    {
        return new UICCustom($"$('#{this.GetId()}').trigger('uic-close');");
    }

    public IUIAction TriggerDestroy()
    {
        return new UICCustom($"$('#{this.GetId()}').trigger('uic-destroy');");
    }
    #endregion

    public enum ModalSize
    {
        /// <summary>
        /// Automatically set the width according to the content
        /// </summary>
        Auto, 

        /// <summary>
        /// Add class modal-sm
        /// </summary>
        Small, 

        /// <summary>
        /// Default size of modal
        /// </summary>
        Normal, 

        /// <summary>
        /// Add class modal-lg
        /// </summary>
        Large, 

        /// <summary>
        /// Add class modal-xl
        /// </summary>
        ExtraLarge
    }
}
