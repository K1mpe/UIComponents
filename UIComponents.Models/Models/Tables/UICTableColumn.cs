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
        public string Type { get; set; }

        //Additional options
        public Dictionary<string, object> Options { get; } = new();


        public string Sorter { get; set; }
        public UICIcon Icon { get; set; }

        public string Width { get; set; }
        public string MaxWidth { get; set; }
        public string Css { get; set; }

        public bool Editing { get; set; } = true;

        public bool Render { get; set; } = true;

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

        public string DefaultFilter { get; set; }

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
                    return "d-block d-sm-none";
                case UICTableColumnVisibility.VisibleSmallerThenMd:
                    return "d-none d-sm-block d-md-none";
                case UICTableColumnVisibility.VisibleSmallerThenLg:
                    return "d-none d-sm-block d-md-block d-lg-none";
                case UICTableColumnVisibility.VisibleSmallerThenXl:
                    return "d-none d-sm-block d-md-block d-lg-block d-xl-none";
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }


}
