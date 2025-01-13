namespace UIComponents.Models.Models.Card;

public class UICCardHeader : UIComponent, IUICHeader, IUICSupportsTaghelperContent
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion


    public UICCardHeader(Translatable title): this()
    {
        Title = title;
    }
    public UICCardHeader()
    {

    }

    public Translatable Title { get; set; }
    public IColor? Color { get; set; } = ColorDefaults.CardHeaderDefault?.Invoke();

    public List<IUIComponent> PrependTitle { get; set; } = new();
    public List<IUIComponent> AppendTitle { get; set; } = new();

    public List<IUIComponent> Buttons { get; set; } = new();

    /// <summary>
    /// If the card supports collapsing, Open or close it by clicking the header.
    /// <br>Does not affect clickinig <see cref="Buttons"/></br>
    /// <br>Can be disabled with ev.stopPropagation()</br>
    /// </summary>
    public bool CollapseCardOnClick { get; set; } = true;
    public Func<object, IUICHeader, Task> Transformer { get; set; } = DefaultTransformer;

    public CardHeaderRenderer Renderer { get; set; } = CardHeaderRenderer.CardHeader;

    #region Methods

    public UICCardHeader AddButton(IUIComponent button)
    {
        Buttons.Add(button);
        return this;
    }
    public UICCardHeader AddButton<T>(out T addedButton, T button) where T : IUIComponent
    {
        addedButton = button;
        return AddButton(button);
    }

    public UICCardHeader AddPrependTitle(IUIComponent item)
    {
        PrependTitle.Add(item);
        return this;
    }
    public UICCardHeader AddPrependTitle<T>(out T addedItem, T item) where T : IUIComponent
    {
        addedItem = item;
        return AddPrependTitle(item);
    }

    public UICCardHeader AddAppendTitle(IUIComponent item)
    {
        AppendTitle.Add(item);
        return this;
    }
    public UICCardHeader AddAppendTitle<T>(out T addedItem, T item) where T : IUIComponent
    {
        addedItem = item;
        return AddAppendTitle(item);
    }

    public UICCardHeader AddCollapseButton(UICCard? card = null)
    {
        if (Buttons.Any(x => x is UICButtonCollapseCard))
            return this;

        Buttons.Add(new UICButtonCollapseCard(card));
        return this;
    }


    public static Task DefaultTransformer(object sender, IUICHeader iheader)
    {
        var header = iheader as UICCardHeader;
        if(sender is UICCard card)
        {
            header.Renderer = CardHeaderRenderer.CardHeader;
            header.AddAttribute("class", "card-header");
            if (card.DisableClosing)
                header.CollapseCardOnClick = false;
            else 
                header.Buttons.Add(new UICButtonCollapseCard(card));
        }
        else if(sender is UICTabs tabs)
        {
            header.Renderer = CardHeaderRenderer.TabHeader;
            header
                .AddAttribute("class", "nav-link")
                .AddAttribute("role", "tab")
                .AddAttribute("data-toggle", "tab");
            if (tabs.ColorTabs && header.Color != null)
                header.AddClass($"bg-{header.Color.ToLower()}");

        }
        else if(sender is UICModal modal)
        {
            header.Renderer = CardHeaderRenderer.ModalHeader;
            header.AddAttribute("class", "modal-header");
            if (modal.ShowCloseButton)
                header.Buttons.Add(new UICButton()
                {
                    PrependButtonIcon = IconDefaults.ButtonClose?.Invoke(),
                    OnClick = modal.TriggerClose()
                });
        }


        return Task.CompletedTask;
    }
    #endregion

    bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;
    /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
    protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        var child = new UICCustom(taghelperContent);
        AppendTitle.Add(child);
        return Task.CompletedTask;
    }
    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);

    public enum CardHeaderRenderer
    {
        CardHeader,
        ModalHeader,
        TabHeader
    }
}
