using Microsoft.AspNetCore.Mvc.Rendering;
using UIComponents.Abstractions.Interfaces.Inputs;
using static UIComponents.Models.Models.Inputs.UICInputSelectList;

namespace UIComponents.Models.Models.Inputs
{
    public class UICInputSelectList : UICInput<string>, IUICInputSelectList
    {
        #region Fields
        public override bool HasClientSideValidation => ValidationRequired;
        public override string RenderLocation
        {
            get
            {
                var renderer = Renderer;
                if (SearchableIfMinimimResults <= SelectListItems.Count)
                    renderer = SelectListRenderer.Select2;
                return this.CreateDefaultIdentifier(renderer);
            }
        }
        #endregion

        #region Ctor
        public UICInputSelectList() : base(null)
        {

        }
        public UICInputSelectList(string propertyName, List<SelectListItem> selectListItems) : base(propertyName)
        {
            SelectListItems = selectListItems.ToUIC();
        }
        public UICInputSelectList(string propertyName, List<UICSelectListItem> selectListItems = null) : base(propertyName)
        {
            SelectListItems = selectListItems;
        }
        #endregion

        #region Properties

        public IColor? Color { get; set; } = Defaults.Models.Inputs.UICInputSelectList.Color;
        /// <summary>
        /// Get a searchfield inside the if there are at least this numer of selectlistitems.
        /// </summary>
        /// <remarks>
        /// 0 => always on
        /// <br> -1 => never on</br></remarks>
        public int SearchableIfMinimimResults { get; set; } = Defaults.Models.Inputs.UICInputSelectList.SearchableIfMinimimResults;


        public List<UICSelectListItem> SelectListItems { get; set; } = new();

        public bool ValidationRequired { get; set; } = Defaults.Models.Inputs.UICInputSelectList.ValidationRequired;

        /// <summary>
        /// If no <see cref="SelectListItems"/> are available, A item is generated with this text
        /// </summary>
        public Translatable NoItemsText { get; set; } = Defaults.Models.Inputs.UICInputSelectList.NoItemsText;



        /// <summary>
        /// If true, the user can create new options by typing
        /// </summary>
        public bool AllowDynamicOptions { get; set; } = Defaults.Models.Inputs.UICInputSelectList.AllowDynamicOptions;

        /// <summary>
        /// Action that is triggered on opening the selectlist
        /// </summary>
        public IUICAction? OnListOpen { get; set; } = Defaults.Models.Inputs.UICInputSelectList.OnListOpen;

        public SelectListRenderer Renderer { get; set; } = Defaults.Models.Inputs.UICInputSelectList.Renderer;

        object IUICInputSelectList.Value => Value;


        #endregion

        #region Methods
        public UICInputSelectList AddSource(UICActionGetPost source)
        {
            var selectSource = new UICInputSelectListSource(this, source);
            AddSource(selectSource);
            return this;
        }
        public UICInputSelectList AddSource(out UICInputSelectListSource outSource, UICActionGetPost source)
        {
            var selectSource = new UICInputSelectListSource(this, source);
            return AddSource(out outSource, selectSource);
        }
        public UICInputSelectList AddSource(UICActionGetPost source, Action<UICInputSelectListSource> action)
        {
            var selectSource = new UICInputSelectListSource(this, source);
            return AddSource(selectSource, action);
        }
        public UICInputSelectList AddSource(UICInputSelectListSource source)
        {
            source.InputSelectList = this;
            ScriptCollection.AddToScripts(source);
            return this;
        }
        public UICInputSelectList AddSource(out UICInputSelectListSource outSource, UICInputSelectListSource source)
        {
            outSource = source;
            return AddSource(source);
        }
        public UICInputSelectList AddSource(UICInputSelectListSource source, Action<UICInputSelectListSource> action)
        {
            AddSource(source);
            action(source);
            return this;
        }

        #endregion


        public enum SelectListRenderer
        {
            Default,
            Select2
        }
    }
}
namespace UIComponents.Defaults.Models.Inputs
{
    public static class UICInputSelectList
    {
        public static IColor? Color { get; set; }
        public static int SearchableIfMinimimResults { get; set; } = -1;
        public static bool ValidationRequired { get; set; }
        public static Translatable NoItemsText { get; set; } = TranslationDefaults.SelectListNoItems;
        public static bool AllowDynamicOptions { get; set; }
        public static IUICAction? OnListOpen { get; set; }
        public static SelectListRenderer Renderer { get; set; } = SelectListRenderer.Select2;
    }
}


