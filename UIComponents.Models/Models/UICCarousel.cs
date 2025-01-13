using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Reflection.Metadata;

namespace UIComponents.Models.Models
{
    public class UICCarousel : UIComponent, IUICHasAttributesAndChildren, IUICSupportsTaghelperContent
    {
        #region Ctor
        public UICCarousel()
        {
            RenderConditions.Add((self) =>((UICCarousel)self).RenderEmpty || ((UICCarousel)self).Children.Where(x => x.HasValue()).Any());
        }
        #endregion

        #region Properties
        /// <summary>
        /// The pages used by the carousel
        /// </summary>
        public List<IUIComponent> Children { get; set; } = new();

        /// <summary>
        /// Show arrows on the left and right of the carousel
        /// </summary>
        public UICHoverVisibility ArrowIndicatorsVisibility { get; set; } = Defaults.Models.UICCarousel.ArrowIndicatorsVisibility;

        /// <summary>
        /// Show the tab icons at the bottom, showing the tabs
        /// </summary>
        public UICHoverVisibility TabIndicatorsVisibility { get; set; } = Defaults.Models.UICCarousel.TabIndicatorsVisibility;

        /// <summary>
        /// If a <see cref="Children"/> inherits <see cref="IUICHasColor"/>, the indicator will be in this color.
        /// <br></br> Cards and tabs already inherit this interface. Use <see cref="UICHasColor"/> for any other component without a color
        /// </summary>
        public bool ColorTabIndicators { get; set; } = Defaults.Models.UICCarousel.ColorTabIndicators;

        public TimeSpan? NextPageInterval { get; set; } = Defaults.Models.UICCarousel.NextPageInterval?.Invoke();

        /// <summary>
        /// Render the carousel if there are no <see cref="Children"/> to be rendered
        /// </summary>
        public bool RenderEmpty { get; set; } = Defaults.Models.UICCarousel.RenderEmpty;

        /// <summary>
        /// Only render the content of a single child if only one is available.
        /// </summary>
        public bool OnlyRenderSingleContent { get; set; } = Defaults.Models.UICCarousel.OnlyRenderSingleContent;

        /// <summary>
        /// After the last page, restart with the first page
        /// </summary>
        public bool Loop { get; set; } = Defaults.Models.UICCarousel.Loop;

        /// <summary>
        /// Instead of sliding from one card to another, use a fading animation
        /// </summary>
        public bool FadeAnimation { get; set; } = Defaults.Models.UICCarousel.FadeAnimation;

        /// <summary>
        /// Triggered when the sliding starts.
        /// <br></br>args.direction: The direction in which the carousel is sliding (either "left" or "right").
        /// <br></br>args.relatedTarget: The DOM element that is being slid into place as the active item.
        /// <br></br>args.from: The index of the current item
        /// <br></br>args.to: The index of the next item
        /// </summary>
        public IUICAction OnSlideStart { get; set; } = new UICCustom();

        /// <summary>
        /// Triggered when the sliding animation is finished
        /// <br></br>args.direction: The direction in which the carousel is sliding (either "left" or "right").
        /// <br></br>args.relatedTarget: The DOM element that is being slid into place as the active item.
        /// <br></br>args.from: The index of the current item
        /// <br></br>args.to: The index of the next item
        /// </summary>
        public IUICAction OnSlideFinished { get; set; } = new UICCustom();
        #endregion

        #region Triggers

        /// <summary>
        /// $('#Id').carousel('pauze');
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerPause()
        {
            return new UICCustom($"$('#{this.GetId()}').carousel('pauze');");
        }

        /// <summary>
        /// $('#Id').carousel('cycle');
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerResume()
        {
            return new UICCustom($"$('#{this.GetId()}').carousel('cycle');");
        }

        /// <summary>
        /// $('#Id').carousel([pageIndex]);
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IUICAction TriggerGoToPage(int pageIndex)
        {
            return new UICCustom($"$('#{this.GetId()}').carousel({pageIndex});");
        }

        /// <summary>
        /// $('#Id').carousel('prev');
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerPrevious()
        {
            return new UICCustom($"$('#{this.GetId()}').carousel('prev');");
        }

        /// <summary>
        /// $('#Id').carousel('next');
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerNext()
        {
            return new UICCustom($"$('#{this.GetId()}').carousel('next');");
        }

        #endregion

        bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;
        /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
        protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
        {
            var child = new UICCustom(taghelperContent);
            this.Add(child);
            return Task.CompletedTask;
        }
        Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
    }
}
namespace UIComponents.Defaults.Models
{
    public static class UICCarousel
    {
        public static UICHoverVisibility ArrowIndicatorsVisibility { get; set; } = UICHoverVisibility.Visible;
                
        public static UICHoverVisibility TabIndicatorsVisibility { get; set; } = UICHoverVisibility.Visible;
                
        public static bool ColorTabIndicators { get; set; } = true;
                
        public static Func<TimeSpan?> NextPageInterval { get; set; } = ()=>TimeSpan.FromSeconds(5);
                
        public static bool RenderEmpty { get; set; }
                
        public static bool OnlyRenderSingleContent { get; set; } = true;
                
        public static bool Loop { get; set; } = true;
                
        public static bool FadeAnimation { get; set; }
    }
}
