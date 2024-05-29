using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Linq.Expressions;
using System.Reflection;
using UIComponents.Abstractions.Interfaces.Tables;
using UIComponents.Models.Helpers;
using UIComponents.Models.Models.Tables;
using UIComponents.Models.Models.Tables.TableColumns;
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
        public string Width { get; set; } = Defaults.Models.Table.UICTable.Width;
        public string Height { get; set; } = Defaults.Models.Table.UICTable.Height;

        /// <summary>
        /// Enable the table columns to be resized
        /// </summary>
        public bool Resizable { get; set; } = Defaults.Models.Table.UICTable.Resizable;

        public List<IUICTableColumn> Columns { get; set; } = new();

        public bool Filtering { get; set; } = Defaults.Models.Table.UICTable.Filtering;
        public bool Selecting { get; set; } = Defaults.Models.Table.UICTable.Selecting;
        public bool Sorting { get; set; } = Defaults.Models.Table.UICTable.Sorting;

        /// <summary>
        /// If <see cref="EnableInsert"/>, <see cref="EnableUpdate"/> or <see cref="EnableDelete"/> are true, add the <see cref="AddControlColumn(Action{UICTableColumnControl})"/> at the end of the columns
        /// </summary>
        public bool AutoAddControlColumn { get; set; } = Defaults.Models.Table.UICTable.AutoAddControlColumn;
        public bool EnableInsert { get; set; } = Defaults.Models.Table.UICTable.EnableInsert;
        public bool EnableUpdate { get; set; } = Defaults.Models.Table.UICTable.EnableUpdate;
        public bool EnableDelete { get; set; } = Defaults.Models.Table.UICTable.EnableDelete;

        /// <summary>
        /// The input fields will have their value as tooltip.
        /// </br>
        /// This is usefull for content that will likely be clipped.
        /// </summary>
        public bool EnableTooltip { get; set; } = Defaults.Models.Table.UICTable.EnableTooltip;


        #region Data
        /// <summary>
        /// Static data that will be used when no <see cref="LoadData"/> is provided
        /// </summary>
        public List<object> Data { get; set; } = new();

        /// <summary>
        /// When triggering 'uic-reload', this is the delay before the reload is exicuted.
        /// <br>This is usefull if multiple signalR triggers can influence the table at the same time, only one trigger will be run</br>
        /// <br>This uses the same function as <see cref="UICActionDelayedAction"/></br>
        /// </summary>
        public int ReloadDelay { get; set; } = Defaults.Models.Table.UICTable.ReloadDelay;

        /// <summary>
        /// Default sorting on this property
        /// </summary>
        public GridSorter Sorter { get; set; }
        #endregion

        /// <summary>
        /// The max size for each page
        /// </summary>
        public int PageSize { get; set; } = Defaults.Models.Table.UICTable.PageSize;

        public string PagingSelector { get; set; } = Defaults.Models.Table.UICTable.PagingSelector;

        /// <summary>
        /// This will add the jsgrid-minimal class.
        /// <br>You can write Css properties to make the grid more compact</br>
        /// </summary>
        public bool Minimal { get; set; } = Defaults.Models.Table.UICTable.Minimal;

        /// <summary>
        /// If true, replace the load indicator with uic.partial.showLoadingOverlay && uic.partial.hideLoadingOverlay
        /// </summary>
        public bool ReplaceLoadingIndicator { get; set; } = Defaults.Models.Table.UICTable.ReplaceLoadingIndicator;

        /// <summary>
        /// Save the last filters in the localstorage from that browser. This requires the Id to be set.
        /// </summary>
        public bool SaveFiltersInLocalStorage { get; set; } = Defaults.Models.Table.UICTable.SaveFiltersInLocalStorage;


        /// <summary>
        /// Always show all select filters. Even if no items are available with this filter.
        /// <br>This feature only works if all remaining items are visible on a single page</br>
        /// </summary>
        public bool ShowAllSelectFilters { get; set; } = Defaults.Models.Table.UICTable.ShowAllSelectFilters;



        /// <summary>
        /// While in edit mode, if you lose focus from the edit row, this will trigger a save instead of cancel
        /// </summary>
        public bool SaveOnBlur { get; set; } = Defaults.Models.Table.UICTable.SaveOnBlur;

        /// <summary>
        /// While in edit mode, enter will trigger a submit of the current row
        /// </summary>
        public bool SaveOnEnter { get; set; } = Defaults.Models.Table.UICTable.SaveOnEnter;

        public List<UICSignalR> SignalRRefreshTriggers { get; set; } = new();

        #region Events
        public IUICAction OnInit { get; set; } = Defaults.Models.Table.UICTable.OnInit;

        /// <summary>
        /// This function is called when data has finished loading.
        /// </summary>
        /// <remarks>
        /// Available args:
        /// <br>args.grid</br>
        /// <br>args.data</br>
        /// </remarks>
        public IUICAction OnDataLoaded { get; set; } = Defaults.Models.Table.UICTable.OnDataLoaded;
        public IUICAction OnDataEditing { get; set; } = Defaults.Models.Table.UICTable.OnDataEditing;

        /// <summary>
        /// This function is called when trying to delete a item.
        /// <br>If you want to cancel deleting, set args.cancel = true</br>
        /// </summary>
        /// <remarks>
        /// Available args:
        /// <br> args.grid</br>
        /// <br> args.row</br>
        /// <br> args.item</br>
        /// <br> args.itemIndex</br>
        /// </remarks>
        public IUICAction OnItemDeleting { get; set; } = Defaults.Models.Table.UICTable.OnItemDeleting;

        /// <summary>
        /// When no <see cref="OnRowClick"/> function is provided and editing is true, edit the row that is clicked
        /// </summary>
        public bool EditOnRowClick { get; set; } = Defaults.Models.Table.UICTable.EditOnRowClick;

        /// <summary>
        /// Available arguments => 'args'
        /// <br>args.event</br>
        /// <br>args.item</br>
        /// <br>args.itemIndex</br>
        /// </summary>
        public IUICAction OnRowClick { get; set; } = Defaults.Models.Table.UICTable.OnRowClick;

        /// <summary>
        /// The function that is used to load the data.
        /// </summary>
        /// <remarks>
        /// Available args:
        /// args => filter arguments
        /// </remarks>
        public IUICAction LoadData { get; set; } = Defaults.Models.Table.UICTable.LoadData;

        /// <summary>
        /// Called when a item is inserted.
        /// </summary>
        /// <remarks>
        /// Available args => 'item'
        /// </remarks>
        public IUICAction OnInsertItem { get; set; } = Defaults.Models.Table.UICTable.OnInsertItem;

        /// <summary>
        /// Called when a item is deleted.
        /// </summary>
        /// <remarks>
        /// Available args => 'item'
        /// </remarks>
        public IUICAction OnUpdateItem { get; set; } = Defaults.Models.Table.UICTable.OnUpdateItem;

        /// <summary>
        /// Called when a item is deleted.
        /// </summary>
        /// <remarks>
        /// Available args => 'item'
        /// </remarks>
        public IUICAction OnDeleteItem { get; set; } = Defaults.Models.Table.UICTable.OnDeleteItem;

        /// <summary>
        /// This function overwrites the default functionality for the insertbutton. This may be used to open a model for inserting instead.
        /// </summary>
        public IUICAction OnInsertButtonClick { get; set; } = Defaults.Models.Table.UICTable.OnInsertButtonClick;

        /// <summary>
        /// This runs at the end of the jsGrid configuration, and may overwrite all previous functionality
        /// </summary>
        public IUICAction AdditionalConfig { get; set; } = Defaults.Models.Table.UICTable.AdditionalConfig;

        /// <summary>
        /// Create a custom renderer for the entire row
        /// </summary>
        /// <remarks>
        /// Available args=> item, index
        /// </remarks>
        public IUICAction RowRenderer { get; set; } = Defaults.Models.Table.UICTable.RowRenderer;
        #endregion

        #endregion

        #region Methods

        #region AddColumn
        /// <summary>
        /// Add a column for this propertyname. If one already exists, use the existing one for the config
        /// </summary>
        public UICTable AddColumn(PropertyInfo propInfo, Action<UICTableColumn> config = null)
        {
            UICTableColumn column = Columns.Where(x=> x is UICTableColumn).OfType<UICTableColumn>().Where(x=> x.PropertyInfo!= null && x.PropertyInfo.Name == propInfo.Name).FirstOrDefault();
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
            column = Columns.Where(x => x is UICTableColumn).OfType<UICTableColumn>().Where(x => x.PropertyInfo != null && x.PropertyInfo.Name == propInfo.Name).FirstOrDefault();
            if (column == null)
            {
                column = new UICTableColumn(propInfo);
                Columns.Add(column);
            }

            return this;
        }
        public UICTable AddColumn<T>(T column, Action<T> config = null) where T: class, IUICTableColumn
        {
            Columns.Add(column);
            if (config != null)
                config(column);
            return this;
        }
        public UICTable AddColumn<T>(out T addedColumn, T column) where T : class, IUICTableColumn
        {
            Columns.Add(column);
            addedColumn = column;
            return this;
        }

        public UICTable AddControlColumn(out UICTableColumnControl controlColumn)
        {
            controlColumn = Columns.Where(x => x is UICTableColumnControl).OfType<UICTableColumnControl>().FirstOrDefault();
            if(controlColumn == null)
            {
                controlColumn = new UICTableColumnControl();
                Columns.Add(controlColumn);
            }
            return this;
        }
        public UICTable AddControlColumn(Action<UICTableColumnControl> config = null)
        {
            AddControlColumn(out var column);
            config?.Invoke(column);
            return this;
        }

        #endregion

        #region InsertColumn

        public UICTable InsertColumn(int index, PropertyInfo propInfo, Action<UICTableColumn> config = null)
        {
            
            var existingCol = Columns.Where(x => x is UICTableColumn tableColumn && tableColumn.PropertyInfo != null && tableColumn.PropertyInfo.Name == propInfo.Name).FirstOrDefault() as UICTableColumn;
            if(existingCol == null)
            {
                existingCol = new UICTableColumn(propInfo);
            }
            else
            {
                Columns.Remove(existingCol);
            }
            if(Columns.Count > index)
                Columns.Insert(index, existingCol);
            else
                Columns.Add(existingCol);
            config?.Invoke(existingCol);
            return this;
        }

        public UICTable InsertColumn(int index, out UICTableColumn addedColumn, PropertyInfo propInfo)
        {
            var existingCol = Columns.Where(x => x is UICTableColumn tableColumn && tableColumn.PropertyInfo != null && tableColumn.PropertyInfo.Name == propInfo.Name).FirstOrDefault() as UICTableColumn;
            if (existingCol == null)
            {
                existingCol = new UICTableColumn(propInfo);
            }
            else
            {
                Columns.Remove(existingCol);
            }
            addedColumn = existingCol;
            if (Columns.Count > index)
                Columns.Insert(index, existingCol);
            else 
                Columns.Add(existingCol);

            return this;
        }

        public UICTable InsertColumn<T>(int index, T column, Action<T> config = null) where T : class, IUICTableColumn
        {
            var existingCol = Columns.Where(x => x == column).FirstOrDefault();
            if (existingCol != null)
            {
                Columns.Remove(existingCol);
            }

            if (Columns.Count > index)
                Columns.Insert(index, column);
            else
                Columns.Add(column);

            config(column);
            return this;
        }

        public UICTable InsertColumn<T>(int index, out T addedColumn, T column) where T : class, IUICTableColumn
        {
            var existingCol = Columns.Where(x => x == column).FirstOrDefault();
            if (existingCol != null)
            {
                Columns.Remove(existingCol);
            }
            addedColumn = column;
            if (Columns.Count > index)
                Columns.Insert(index, column);
            else
                Columns.Add(column);

            return this;
        }
        #endregion

        #region RemoveColumn

        public UICTable RemoveColumn(PropertyInfo propInfo)
        {
            return AddColumn(propInfo, config => config.Render = false);
        }

        #endregion

        /// <summary>
        /// Add a signalREvent. If no action is set, this will automatically be set to reloading the table
        /// </summary>
        /// <param name="signalR"></param>
        /// <returns></returns>
        public UICTable AddSignalR(UICSignalR signalR)
        {
            if (!signalR.Action.HasValue())
                signalR.Action = TriggerReload();
            SignalRRefreshTriggers.Add(signalR);
            return this;
        }

        #endregion



        #region ClientMethods
        public IUICAction TriggerReload()
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
            OnInsertItem = new UICActionPost(typeof(T).Name, "Insert") { GetVariableData = new UICCustom("item") };
            OnUpdateItem = new UICActionPost(typeof(T).Name, "Update") { GetVariableData = new UICCustom("item") };
            OnDeleteItem = new UICActionPost(typeof(T).Name, "Delete") { GetVariableData = new UICCustom("item") };
        }
        public UICTable(string id) : this()
        {
            if(!string.IsNullOrWhiteSpace(id))
                this.SetId(id);
        }
        public UICTable(List<T> data) : this()
        {
            Data = data;
        }
        public UICTable(UICActionGetPost loadDataFunc) : this()
        {
            LoadData = loadDataFunc;
        }
        #endregion


        #region Properties
        /// <summary>
        /// The static data provided
        /// </summary>
        public new IEnumerable<T> Data
        {
            get => base.Data.OfType<T>().ToList();
            set => base.Data = value.OfType<object>().ToList();
        }
        #endregion


        #region Methods

        #region Add Column
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
        /// Get the column that matches this expression. The column needs to exist at this point!
        /// </summary>
        /// <remarks>
        /// If no col is found, a new temporary column can be returned to ensure the uic-custom taghelper does not crash</remarks>
        /// <param name="expression"></param>
        /// <param name="allowNew"></param>
        /// <returns></returns>
        public UICTableColumn GetColumn(Expression<Func<T, object>> expression, bool allowNew=true)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
            var col= Columns.Where(x => x is UICTableColumn).OfType<UICTableColumn>().Where(x=>x.PropertyInfo != null && x.PropertyInfo.Name ==propertyInfo.Name).FirstOrDefault();
            if (col == null && allowNew)
                col = new();
            return col;
        }

        /// <summary>
        /// Add multiple columns, columns should be split by ","
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public UICTable<T> AddColumns(string columnNames, bool caseSensitive = true)
        {
            if (string.IsNullOrWhiteSpace(columnNames))
                return this;

            var columns = columnNames.Split(",").Select(x => x.Trim());
            foreach(var column in columns)
            {
                var propInfo = typeof(T).GetProperty(column);
                if (propInfo == null && !caseSensitive)
                    propInfo = typeof(T).GetProperties().Where(x => x.Name.ToUpper() == column.ToUpper()).FirstOrDefault();

                if (propInfo == null) 
                    continue;
                AddColumn(propInfo);
            }
            return this;
        }

        /// <summary>
        /// Add multiple columns by expression
        /// </summary>
        /// <param name="propExpressions"></param>
        /// <returns></returns>
        public UICTable<T> AddColumns(params Expression<Func<T, object>>[] propExpressions)
        {
            foreach (var expression in propExpressions)
            {
                AddColumn(expression);
            }
            return this;
        }

        /// <summary>
        /// <inheritdoc cref=" AddColumns(Expression{Func{T, object}}[])"/>
        /// <br>This also contains a action that will be applied to all provided expressions</br>
        /// </summary>
        public UICTable<T> ConfigureColumns(Action<UICTableColumn> action, params Expression<Func<T, object>>[] propExpressions)
        {
            foreach (var propExpression in propExpressions)
                AddColumn(propExpression, action);
            return this;
        }


        /// <summary>
        /// Add all the remaining columns in the same order as they are defined in the Model
        /// </summary>
        public UICTable<T> AddAllUndefinedColumns(bool includeId = false, bool includeIsDeleted = false)
        {
            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var ignoreAttr = propertyInfo.GetCustomAttribute<UICIgnoreAttribute>();
                if (ignoreAttr != null)
                    continue;

                if (!includeId && propertyInfo.Name.ToUpper() == "ID")
                    continue;

                if (!includeIsDeleted && propertyInfo.Name.ToUpper() == "ISDELETED")
                    continue;

                if (Columns.Where(x => x is UICTableColumn).OfType<UICTableColumn>().Where(x => x.PropertyInfo != null && x.PropertyInfo.Name == propertyInfo.Name).Any())
                    continue;
                AddColumn(propertyInfo);
            }
            return this;
        }
        #endregion

        #region Insert Column

        public UICTable<T> InsertColumn(int index, Expression<Func<T, object>> propExpression, Action<UICTableColumn> action = null)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(propExpression);
            InsertColumn(index, propertyInfo, action);
            return this;
        }
        public UICTable<T> InsertColumn(int index, out UICTableColumn column, Expression<Func<T, object>> propExpression)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(propExpression);
            InsertColumn(index, out column, propertyInfo);
            return this;
        }


        #endregion

        #region RemoveColumn

        public UICTable<T> RemoveColumn(Expression<Func<T, object>> propExpression)
        {
            var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(propExpression);
            RemoveColumn(propertyInfo);
            return this;
        }


        /// <summary>
        /// Set the render property for multiple columns to false. Columns must be split by ","
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public UICTable<T> RemoveColumns(string columnNames, bool caseSensitive = false)
        {
            if (string.IsNullOrWhiteSpace(columnNames))
                return this;

            var columns = columnNames.Split(",").Select(x => x.Trim());
            foreach (var column in columns)
            {
                var propInfo = typeof(T).GetProperty(column);
                if (propInfo == null && !caseSensitive)
                    propInfo = typeof(T).GetProperties().Where(x => x.Name.ToUpper() == column.ToUpper()).FirstOrDefault();

                if (propInfo == null)
                    continue;
                RemoveColumn(propInfo);
            }
            return this;
        }
        #endregion


        /// <summary>
        /// Set the default ordering of the table
        /// </summary>
        public UICTable<T> OrderBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Asc)
        {
            var propInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
            Sorter = new(propInfo.Name, sortOrder);
            return this;
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
namespace UIComponents.Defaults.Models.Table
{
    public static class UICTable
    {
        public static string Width { get; set; } = "";
        public static string Height { get; set; } = "auto";
        public static bool Resizable { get; set; } = true;
        public static bool Filtering { get; set; } = true;
        public static bool Selecting { get; set; } = true;
        public static bool Sorting { get; set; } = true;        
        public static bool AutoAddControlColumn { get; set; } = true;
        public static bool EnableInsert { get; set; }
        public static bool EnableUpdate { get; set; }
        public static bool EnableDelete { get; set; }
        public static bool EnableTooltip { get; set; }
        public static int PageSize { get; set; } = int.MaxValue;
        public static string PagingSelector { get; set; }
        public static bool Minimal { get; set; } = true;
        public static bool ReplaceLoadingIndicator { get; set; } = true;
        public static bool SaveFiltersInLocalStorage { get; set; } = true;
        public static bool ShowAllSelectFilters { get; set; }
        public static bool SaveOnBlur { get; set; }
        public static bool SaveOnEnter { get; set; }
        public static List<UICSignalR> SignalRRefreshTriggers { get; set; } = new();
        public static int ReloadDelay { get; set; } = 250;
        public static IUICAction OnInit { get; set; } = new UICCustom();
        public static IUICAction OnDataLoaded { get; set; } = new UICCustom();
        public static IUICAction OnDataEditing { get; set; } = new UICCustom();
        public static IUICAction OnItemDeleting { get; set; } = new UICCustom();
        public static bool EditOnRowClick { get; set; } = true;
        public static IUICAction OnRowClick { get; set; } = new UICCustom();
        public static IUICAction LoadData { get; set; } = new UICCustom();
        public static IUICAction OnInsertItem { get; set; } = new UICCustom();
        public static IUICAction OnUpdateItem { get; set; } = new UICCustom();
        public static IUICAction OnDeleteItem { get; set; } = new UICCustom();
        public static IUICAction OnInsertButtonClick { get; set; } = new UICCustom();
        public static IUICAction AdditionalConfig { get; set; } = new UICCustom();
        public static IUICAction RowRenderer { get; set; } = new UICCustom();
    }
}
