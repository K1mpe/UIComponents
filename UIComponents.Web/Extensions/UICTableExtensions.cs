using System.Linq.Expressions;
using UIComponents.Generators.Interfaces;
using UIComponents.Abstractions.Helpers;
using UIComponents.Models.Models.Tables;

namespace UIComponents.Web.Extensions;

public static class UICTableExtensions
{
    public static async Task<UICTable> AddGeneratedColumn(this UICTable table, PropertyInfo propertyInfo, IUIComponentGenerator componentGenerator, Action<UICTableColumn> config = null)
    {
        var column = await componentGenerator.CreateTableColumnFromProperty(propertyInfo);
        return table.AddColumn(column, config);
    }

    public static async Task<UICTable<T>> AddGeneratedColumn<T>(this UICTable<T> table, Expression<Func<T, object>> expression, IUIComponentGenerator componentGenerator, Action<UICTableColumn> config = null) where T : class
    {
        var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
        await AddGeneratedColumn(table, propertyInfo, componentGenerator, config);
        return table;
    } 


}
