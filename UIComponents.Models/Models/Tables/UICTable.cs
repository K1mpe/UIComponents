using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Linq.Expressions;
using System.Reflection;
using UIComponents.Models.Helpers;
using static UIComponents.Models.Models.Actions.UICActionGetPost;

namespace UIComponents.Models.Models.Tables
{
    public class UICTable : UIComponent
    {
        #region Fields
        public override string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICTable));
        #endregion

        #region Ctor
        public UICTable()
        {
            
        }
        public UICTable(List<object> data) : this()
        {
            Data = data;
        }
        #endregion

        #region Properties
        public string Width { get; set; } = "100%";
        public string Height { get; set; } = string.Empty;

        public List<UICTableColumn> Columns { get; set; } = new();

        public bool Filtering { get; set; } = true;
        public bool Selecting { get; set; } = true;
        public bool Sorting { get; set; } = true;


        public bool EnableInsert { get; set; }
        public bool EnableUpdate { get; set; }
        public bool EnableDelete { get; set; }

        /// <summary>
        /// The input fields will have their value as tooltip.
        /// </br>
        /// This is usefull for content that will likely be clipped.
        /// </summary>
        public bool EnableTooltip { get; set; }


        #region Data
        public List<object> Data { get; set; }

        public string DataSource { get; set; }
        public ActionTypeEnum GetOrPost { get; set; } = ActionTypeEnum.Post;
        public object PostData { get; set; }

        public UICActionGetPost InsertItem { get; set; }

        /// <summary>
        /// If provided, there will be a expand button that gets a partial.
        /// </summary>
        public UICActionGetPost Partial { get; set; }

        /// <summary>
        /// Default sorting on this property
        /// </summary>
        public GridSorter Sorter { get; set; }
        #endregion

        /// <summary>
        /// The max size for each page
        /// </summary>
        public int PageSize { get; set; } = int.MaxValue;

        public string PagingSelector { get; set; }

        /// <summary>
        /// This will add the jsgrid-minimal class.
        /// <br>You can write Css properties to make the grid more compact</br>
        /// </summary>
        public bool Minimal { get; set; } = true;

        /// <summary>
        /// Save the last filters in the localstorage from that browser. This requires the Id to be set.
        /// </summary>
        public bool SaveFiltersInLocalStorage { get; set; } = true;


        /// <summary>
        /// Always show all select filters. Even if no items are available with this filter.
        /// <br>This feature only works if all remaining items are visible on a single page</br>
        /// </summary>
        public bool ShowAllSelectFilters { get; set; }



        /// <summary>
        /// While in edit mode, if you lose focus from the edit row, this will trigger a save instead of cancel
        /// </summary>
        public bool SaveOnBlur { get; set; }

        public List<UICSignalR> SignalRRefreshTriggers { get; set; } = new();   

        #region Events
        public IUICAction OnInit { get; set; } = new UICCustom();
        public IUICAction OnDataLoaded { get; set; } = new UICCustom();

        public IUICAction OnRowClick { get; set; } = new UICCustom();
        public IUICAction OnRowDubbleClick { get; set; } = new UICCustom();

        public IUICAction OnInsertItem { get; set; } = new UICCustom();
        public IUICAction OnUpdateItem { get; set; } = new UICCustom();
        public IUICAction OnDeleteItem { get; set; } = new UICCustom();

        public IUICAction OnInsertButtonClick { get; set; } = new UICCustom();


        public IUICAction AdditionalConfig { get; set; } = new UICCustom();
        public IUICAction RowRenderer { get; set; } = new UICCustom();
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Add a column for this propertyname. If one already exists, use the existing one for the config
        /// </summary>
        public UICTable AddColumn(PropertyInfo propInfo, Action<UICTableColumn> config = null)
        {
            UICTableColumn column = Columns.Where(x=> x.PropertyInfo!= null && x.PropertyInfo.Name == propInfo.Name).FirstOrDefault();
            if (column == null)
            {
                column = new UICTableColumn(propInfo);
                Columns.Add(column);
            }
            if(config != null)
                config(column);

            return this;                
        }

        public UICTable AddColumn(out UICTableColumn column, PropertyInfo propInfo)
        {
            column = Columns.Where(x => x.PropertyInfo != null && x.PropertyInfo.Name == propInfo.Name).FirstOrDefault();
            if (column == null)
            {
                column = new UICTableColumn(propInfo);
                Columns.Add(column);
            }

            return this;
        }
        public UICTable AddColumn(UICTableColumn column, Action<UICTableColumn> config = null)
        {
            Columns.Add(column);
            if (config != null)
                config(column);
            return this;
        }
        public UICTable AddColumn(out UICTableColumn addedColumn, UICTableColumn column)
        {
            Columns.Add(column);
            addedColumn = column;
            return this;
        }

        /// <summary>
        /// Add a signalREvent.
        /// </summary>
        /// <param name="signalR"></param>
        /// <returns></returns>
        public UICTable AddSignalR(UICSignalR signalR)
        {
            SignalRRefreshTriggers.Add(signalR);
            return this;
        }

        public string GetCondition()
        {
            throw new NotImplementedException();
        }
        #endregion



        #region ClientMethods
        public IUICAction Reload()
        {
            return new UICActionRefresh(this);
        }
        #endregion
    }

    public class UICTable<T> : UICTable where T : class
    {
        #region Ctor
        public UICTable() : base()
        {

        }
        public UICTable(string id) : this()
        {
            if(!string.IsNullOrWhiteSpace(id))
                this.SetId(id);
        }
        public UICTable(List<T> data) : base(data.OfType<object>().ToList())
        {
            
        }
        #endregion



        #region Methods
        ///<inheritdoc cref="UICTable.AddColumn(PropertyInfo, Action{UICTableColumn})"/>
        public UICTable<T> AddColumn(Expression<Func<T, object>> propExpression, Action<UICTableColumn> action = null)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(propExpression);
            AddColumn(propertyInfo, action);
            return this;
        }

        /// <inheritdoc cref="UICTable.AddColumn(out UICTableColumn, PropertyInfo)"/>
        public UICTable<T> AddColumn(out UICTableColumn column, Expression<Func<T, object>> propExpression)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(propExpression);
            AddColumn(out column, propertyInfo);
            return this;
        }

        /// <summary>
        /// Add all the remaining columns in the same order as they are defined in the Model
        /// </summary>
        public UICTable<T> AddAllUndefinedColumns(bool includeId, bool includeIsDeleted)
        {
            var properties = typeof(T).GetProperties();
            foreach(var propertyInfo in properties)
            {
                if (Columns.Where(x=>x.PropertyInfo != null && x.PropertyInfo.Name ==propertyInfo.Name).Any())
                    continue;
                AddColumn(propertyInfo);
            }
            return this;
        }
        public UICTable<T> AddColumns(params Expression<Func<T, object>>[] propExpressions)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class GridSorter
    {
        public GridSorter(string column, SortOrder sortOrder)
        {
            Column = column;
            SortOrder = sortOrder;
        }

        public string Column { get; set; }
        public SortOrder SortOrder { get; set; }

        public override string ToString()
        {
            return $"{{field: '{Column}', order: '{SortOrder.ToString().ToLower()}'}}";
        }
    }
    public enum SortOrder
    {
        Asc = 0,
        Desc = 1,
    }
}
