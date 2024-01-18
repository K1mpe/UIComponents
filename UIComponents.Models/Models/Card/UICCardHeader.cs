using System.Reflection;
using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models.Card
{
    public class UICCardHeader : UIComponent, IHeader
    {
        public UICCardHeader(ITranslateable title): this()
        {
            Title = title;
        }
        public UICCardHeader()
        {

        }

        public ITranslateable Title { get; set; }
        public IColor? Color { get; set; } = UIComponents.Defaults.ColorDefaults.CardHeaderDefault;

        public List<IUIComponent> PrependTitle { get; set; } = new();
        public List<IUIComponent> AppendTitle { get; set; } = new();

        public List<IUIComponent> Buttons { get; set; } = new();

        /// <summary>
        /// If the card supports collapsing, Open or close it by clicking the header.
        /// <br>Does not affect clickinig <see cref="Buttons"/></br>
        /// <br>Can be disabled with ev.stopPropagation()</br>
        /// </summary>
        public bool CollapseCardOnClick { get; set; }
        public Func<object, IHeader, Task> Transformer { get; set; } = DefaultTransformer;

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

        public static Task DefaultTransformer(object sender, IHeader iheader)
        {
            var header = iheader as UICCardHeader;
            if(sender is UICCard card)
            {
                header.Renderer = CardHeaderRenderer.CardHeader;
                header.AddAttribute("class", "card-header");
                if (card.DefaultClosed)
                    header.CollapseCardOnClick = false;
            }
            else if(sender is UICTabs tabs)
            {
                header.Renderer = CardHeaderRenderer.TabHeader;
                header
                    .AddAttribute("role", "tab")
                    .AddAttribute("data-toggle", "tab");
            }


            return Task.CompletedTask;
        }
        #endregion

        public enum CardHeaderRenderer
        {
            CardHeader,

            TabHeader
        }
    }
}
