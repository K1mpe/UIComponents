using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using UIComponents.Abstractions.Interfaces.Tables;

namespace UIComponents.Models.Models.Tables
{
    public class UICTableColumn : IUIComponent, IUICTableColumn
    {
        #region Fields
        public virtual string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICTableColumn));
        #endregion

        #region Ctor
        public UICTableColumn(PropertyInfo propInfo = null)
        {
            PropertyInfo = propInfo;
        }
        #endregion

        #region Properties
        public PropertyInfo PropertyInfo{ get; set; }
        public Translatable Title { get; set; }
        public Translatable Tooltip { get; set; }
        public SortOrder? SortOrder { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// This is the default filter that is set on load. May be overwritten by <see cref="UICTable.SaveFiltersInLocalStorage"/>
        /// </summary>
        public object DefaultFilter { get; set; }
        //Additional options
        public Dictionary<string, object> Options { get; } = new();

        /// <summary>
        /// When changing the filter, the data will automatically refresh without needing to click the submit filter button
        /// </summary>
        public bool AutoSearch { get; set; } = true;
        public UICIcon Icon { get; set; }

        public string Width { get; set; }
        public string MaxWidth { get; set; }
        public string Css { get; set; }

        public bool Editing { get; set; } = true;

        public bool Render { get; set; } = true;


        /// <summary>
        /// Give a format if supported
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Step can be used as indicator for numbers, datetime or timeonly
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// The minimum value this field can have
        /// </summary>
        /// 
        public object? MinValue { get; set; }

        /// <summary>
        /// The maximum value this field can have
        /// </summary>
        public object? MaxValue { get; set; }

        public bool CheckViewPermission { get; set; } = true;
        public bool CheckEditPermission { get; set; } = true;

        /// <summary>
        /// When this property is true, this column will not be effected by generators anymore.
        /// </summary>
        public bool IgnoreGenerators { get; set; }

        /// <summary>
        /// Selectlistitems only used for <see cref="UICPropertyType.SelectList"/>
        /// </summary>
        public List<SelectListItem> SelectListItems { get; set; }

        /// <summary>
        /// Value may be null, checkbox is not nullable, Threestate checkbox is.
        /// </summary>
        public bool Nullable { get; set; }

        public UICHorizontalAlignment? HorizontalAlignment { get; set; }
        public UICVerticalAlignment? VerticalAlignment { get; set; }


        /// <summary>
        /// A custom cell renderer for this column
        /// </summary>
        /// <remarks>
        /// Available args: value, item</remarks>
        public virtual IUICAction CellRenderer { get; set; } = new UICCustom();

        /// <summary>
        /// is a function to create cell content. It should return markup as string, DomNode or jQueryElement
        /// </summary>
        /// <remarks>
        /// Available args: value, item</remarks>
        public virtual IUICAction ItemTemplate { get; set; } = new UICCustom();

        /// <summary>
        /// is a function to create cell content of editing row. 
        /// </summary>
        /// <remarks>
        /// Available args: value, item</remarks>
        public virtual IUICAction EditTemplate { get; set; } = new UICCustom();

        /// <summary>
        /// is a function to create filter row cell content. 
        /// </summary>
        public virtual IUICAction FilterTemplate { get; set; } = new UICCustom();

        /// <summary>
        /// is a function to create column header content.
        /// </summary>
        public virtual IUIComponent HeaderTemplate { get; set; } = new UICCustom();

        public UICTableColumnVisibility ColumnVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;
        public UICTableColumnVisibility TextVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;
        public UICTableColumnVisibility IconVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;

        #endregion

        #region Methods
        public UICTableColumn OrderBy(SortOrder order)
        {
            SortOrder = order;
            return this;
        }
        public static string VisibilityClass(UICTableColumnVisibility visibility)
        {
            switch (visibility)
            {
                case UICTableColumnVisibility.VisibleOnAll:
                    return "d-table-cell";
                case UICTableColumnVisibility.HiddenOnAll:
                    return "d-none";
                case UICTableColumnVisibility.HideSmallerThanSm:
                    return "d-none d-sm-table-cell";
                case UICTableColumnVisibility.HideSmallerThanMd:
                    return "d-none d-md-table-cell";
                case UICTableColumnVisibility.HideSmallerThenLg:
                    return "d-none d-lg-table-cell";
                case UICTableColumnVisibility.HideSmallerThenXl:
                    return "d-none d-xl-table-cell";
                case UICTableColumnVisibility.VisibleSmallerThenSm:
                    return "d-table-cell d-sm-none";
                case UICTableColumnVisibility.VisibleSmallerThenMd:
                    return "d-sm-table-cell d-md-none";
                case UICTableColumnVisibility.VisibleSmallerThenLg:
                    return "d-sm-table-cell d-md-table-cell d-lg-none";
                case UICTableColumnVisibility.VisibleSmallerThenXl:
                    return "d-sm-table-cell d-md-table-cell d-lg-table-cell d-xl-none";
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }


}
