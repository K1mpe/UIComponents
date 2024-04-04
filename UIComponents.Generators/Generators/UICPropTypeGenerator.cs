using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators;

public class UICPropTypeGenerator : UICGeneratorBase<PropertyInfo, UICPropertyType?>
{
    private readonly UICConfig _uICConfig;

    public UICPropTypeGenerator(UICConfig uICConfig)
    {
        _uICConfig = uICConfig;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<UICPropertyType?>> GetResponseAsync(PropertyInfo propertyInfo, UICPropertyType? existingResult)
    {
        if (existingResult != null)
            GeneratorHelper.Next<UICPropertyType?>();
        UICPropertyType uicPropertyType = UICPropertyType.String;

        //Get from attribute
        var uicPropertyAttribute = propertyInfo.GetCustomAttribute<UICPropertyTypeAttribute>();
        if (uicPropertyAttribute != null)
            uicPropertyType = uicPropertyAttribute.Type;
        else
        {
            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            bool nullable = type != propertyInfo.PropertyType;

            var dataTypeAttr = propertyInfo.GetCustomAttribute<DataTypeAttribute>();
            if (dataTypeAttr != null)
            {
                switch (dataTypeAttr.DataType)
                {
                    case DataType.Custom:
                        break;
                    case DataType.DateTime:
                        return GeneratorHelper.Success<UICPropertyType?>(UICPropertyType.DateTime, true);
                    case DataType.Date:
                        return GeneratorHelper.Success<UICPropertyType?>(UICPropertyType.DateOnly, true);
                    case DataType.Time:
                        break;
                    case DataType.Duration:
                        return GeneratorHelper.Success<UICPropertyType?>(UICPropertyType.Timespan, true);
                    case DataType.PhoneNumber:
                        break;
                    case DataType.Currency:
                        break;
                    case DataType.Text:
                        break;
                    case DataType.Html:
                        break;
                    case DataType.MultilineText:
                        break;
                    case DataType.EmailAddress:
                        break;
                    case DataType.Password:
                        break;
                    case DataType.Url:
                        break;
                    case DataType.ImageUrl:
                        break;
                    case DataType.CreditCard:
                        break;
                    case DataType.PostalCode:
                        break;
                    case DataType.Upload:
                        break;
                }
            }

            var foreignKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
            if(foreignKey != null)
                return GeneratorHelper.Success<UICPropertyType?>(UICPropertyType.SelectList, true);

            var fakeForeignKey = propertyInfo.GetCustomAttribute<FakeForeignKeyAttribute>();
            if (fakeForeignKey != null)
                return GeneratorHelper.Success<UICPropertyType?>(UICPropertyType.SelectList, true);

            if(UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
                return await GetResponseAsync(inherit, existingResult);

            var foreignKeyType = await _uICConfig.GetForeignKeyTypeAsync(propertyInfo, new());
            if(foreignKeyType != null)
            {
                return GeneratorHelper.Success<UICPropertyType?>(UICPropertyType.SelectList, true);
            }


            if(propertyInfo.PropertyType.IsAssignableTo(typeof(IEnumerable)) && propertyInfo.PropertyType != typeof(string))
                type = propertyInfo.PropertyType.GetGenericArguments()[0];


            switch (type.Name.ToLower())
            {
                case "string":
                case "icomparable":
                    uicPropertyType = UICPropertyType.String;
                    if (propertyInfo.Name.ToLower().EndsWith("color"))
                        uicPropertyType = UICPropertyType.HexColor;
                    break;
                case "int32":
                case "int64":
                case "byte":
                case "long":
                    uicPropertyType = UICPropertyType.Number;
                    break;
                case "single":
                case "double":
                case "decimal":
                    uicPropertyType = UICPropertyType.Decimal;
                    break;
                case "timespan":
                    uicPropertyType = UICPropertyType.Timespan;
                    break;
                case "datetime":
                    uicPropertyType = UICPropertyType.DateTime;
                    break;
                case "dateonly":
                    uicPropertyType = UICPropertyType.DateOnly;
                    break;
                case "timeonly":
                    uicPropertyType = UICPropertyType.TimeOnly;
                    break;
                case "boolean":
                    uicPropertyType = UICPropertyType.Boolean;
                    if (nullable)
                        uicPropertyType = UICPropertyType.ThreeStateBoolean;
                    break;
                default:
                    if (type.BaseType?.Name == "Enum")
                    {
                        uicPropertyType = UICPropertyType.SelectList;
                        break;
                    }
                    break;
            }
        }
        await Task.Delay(0);
        return GeneratorHelper.Success<UICPropertyType?>(uicPropertyType, true);
    }
}
