using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models
{


    /// <summary>
    /// This will set the input groups on the same row
    /// </summary>
    /// <remarks>
    /// To make this work for custom <see cref="IUIComponent"/>, implement the <see cref="IUISingleRowSupport"/> interface 
    /// </remarks>
    public class UICSingleRow : UIComponent, IUICHasChildren<IUIComponent>, IUICHasAttributesAndChildren
    {
        #region Ctor
        public UICSingleRow()
        {

        }

        public UICSingleRow(List<IUIComponent> components)
        {
            Components = components;
        }
        #endregion

        #region Properties

        /// <summary>
        /// These are the affected components
        /// </summary>
        public List<IUIComponent> Components { get; set; } = new();


        /// <summary>
        /// The minimum width of labels
        /// </summary>
        public string MinLabelWidth { get; set; } = Defaults.Models.UICSingleRow.MinLabelWidth;

        /// <summary>
        /// The minimum width of inputs
        /// </summary>
        public string MinInputWidth { get; set; } = Defaults.Models.UICSingleRow.MinInputWidth;

        /// <summary>
        /// The margin between diffrent property rows (SingleRow Only)
        /// </summary>
        public string MarginBetweenRows { get; set; } = Defaults.Models.UICSingleRow.MarginBetweenRows;

        /// <summary>
        /// The margin between diffrent label and input
        /// </summary>
        public string MarginBetweenColumns { get; set; } = Defaults.Models.UICSingleRow.MarginBetweenColumns;

        public List<IUIComponent> Children => Components;
        #endregion

        #region Methods

        #endregion

        #region Converters
        public UICGroup ConvertToGroup()
        {
            var group = new UICGroup(Components) { RenderSingleItem = true };
            return group;
        }

        /// <summary>
        /// Put a list of items inside a singlerow group
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static List<IUIComponent> ConvertFromList(List<IUIComponent> components)
        {
            var result = new UICSingleRow(components);
            return new() { result };
        }
        #endregion

    }
}

namespace UIComponents.Defaults.Models
{


    /// <summary>
    /// This will set the input groups on the same row
    /// </summary>
    /// <remarks>
    /// To make this work for custom <see cref="IUIComponent"/>, implement the <see cref="IUISingleRowSupport"/> interface 
    /// </remarks>
    public static class UICSingleRow
    {
        /// <summary>
        /// The minimum width of labels
        /// </summary>
        public static string MinLabelWidth { get; set; }

        /// <summary>
        /// The minimum width of inputs
        /// </summary>
        public static string MinInputWidth { get; set; }

        /// <summary>
        /// The margin between diffrent property rows (SingleRow Only)
        /// </summary>
        public static string MarginBetweenRows { get; set; }

        /// <summary>
        /// The margin between diffrent label and input
        /// </summary>
        public static string MarginBetweenColumns { get; set; }


    }
}

