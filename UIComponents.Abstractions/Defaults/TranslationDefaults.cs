using System.ComponentModel;
using System.Reflection;
using UIComponents.Abstractions;

namespace UIComponents.Defaults;

public static class TranslationDefaults
{

    public static Translatable ButtonEdit = new Translatable("Button.Edit");
    public static Translatable ButtonReadonly = new Translatable("Button.Readonly");

    public static Translatable ButtonCreate = new Translatable("Button.Create");
    public static Func<Type, Translatable> ButtonCreateTooltip = (type) => new Translatable("Button.Create.Tooltip", "Create a new {0}", TranslateType(type));

    public static Translatable ButtonSave = new Translatable("Button.Save");

    public static Translatable ButtonCancel = new Translatable("Button.Cancel");

    public static Translatable ButtonDelete = new Translatable("Button.Delete");

    public static Translatable ButtonRefresh = new Translatable("Button.Refresh");

    public static Translatable ButtonCardExpand = new Translatable("Button.Expand");
    public static Translatable ButtonCardCollapse = new Translatable("Button.Collapse");


    public static Func<object, Translatable> ButtonDeleteToolTip = (obj) => new Translatable("Button.Delete.Tooltip", "Delete this {0}", TranslateObject(obj));


    /// <summary>
    /// Create a translationmodel for a enum
    /// </summary>
    /// <remarks>
    /// Type: the type of the enum
    /// string: the value of this enum as string
    /// </remarks>
    public static Func<Type, string, Translatable> TranslateEnums = (type, value) => new Translatable($"Enum.{type.Name}.{value}");


    public static Func<Type, Translatable> TranslateType = (type) =>
    {
        string key = type.Name;
        List<Translatable> args = new();
        string defaultValue = null;

        if (type.IsGenericType)
        {

            defaultValue = key.Split("`")[0];
            key = defaultValue + ".Generic";
            defaultValue += "<";
            var genTypes = type.GetGenericArguments();
            for (var i = 0; i < genTypes.Length; i++)
            {
                defaultValue += $"{{{i}}}";
                args.Add(TranslateType(genTypes[i]));
            }

            defaultValue += ">";
        }

        return new Translatable(key, defaultValue, args.ToArray());
    };



    /// <summary>
    /// A Function that creates a Translatable for a object
    /// </summary>
    public static Func<object, Translatable> TranslateObject = (obj) =>
    {
        string toString = obj.ToString();
        if (toString != obj.GetType().FullName)
            return new Untranslated(toString);

        var translatedType = TranslateType(obj.GetType());
        if (obj is IDbEntity dbEntity)
            return new Translatable("DbEntity", "{0} {1}", translatedType, dbEntity.Id);

        return translatedType;
    };

    public static Func<PropertyInfo, UICPropertyType, Translatable> TranslateProperty = (prop, uicPropType) =>
    {
        var translateAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
        if (translateAttr != null)
            return translateAttr.DisplayName;

        if (uicPropType == UICPropertyType.SelectList && prop.Name.EndsWith("Id") && prop.Name != "Id")
            return new Translatable($"{prop.DeclaringType!.Name}.Field.{prop.Name.Substring(0, prop.Name.Length - 2)}");

        return new Translatable($"{prop.DeclaringType!.Name}.Field.{prop.Name}");
    };


    /// <summary>
    /// Function takes a already translated propertyName and creates a validation message
    /// </summary>
    public static Func<string, Translatable> ValidationIsRequired = (translatedPropertyName) => new Translatable("Validation.Required", "{0} is required", translatedPropertyName);

    public static Func<string, int, Translatable> ValidateMinLength = (translatedPropertyName, minLenght) => new Translatable("Validation.MinLength", "The value of {0} must be longer than {1}", translatedPropertyName, minLenght);
    public static Func<string, int, Translatable> ValidateMaxLength = (translatedPropertyName, maxLength) => new Translatable("Validation.MaxLength", "The value of {0} must be shorter than {1}", translatedPropertyName, maxLength);
    public static Func<string, object, Translatable> ValidateMinValue = (translatedPropertyName, minValue) => new Translatable("Validation.MinValue", "The value of {0} must be higher than or equal to {1}", translatedPropertyName, minValue);
    public static Func<string, object, Translatable> ValidateMaxValue = (translatedPropertyName, maxValue) => new Translatable("Validation.MaxValue", "The value of {0} must be lower or equal to {1}", translatedPropertyName, maxValue);
    public static Func<string, Translatable> ValidateColor = (translatedPropertyName) => new Translatable("Validation.Color.Invalid", "{0} has a invalid color", translatedPropertyName);

    public static Func<int, Translatable> FileUploadMaxFiles = (fileCount) =>
    {
        if (fileCount == 1)
            return new Translatable("Fileupload.OneFile", "Upload only one file");
        else return new Translatable("FileUpload.MaxCountFiles", "Upload up to {0} files", fileCount);
    };
    public static Translatable SelectListNoItems = new Translatable("SelectList.NoItemsAvailable", "No items available");


    /// <summary>
    /// When using <see cref="UICTooltipAttribute"/> without a key, this is the key that will be used
    /// </summary>
    public static Func<PropertyInfo, UICPropertyType, string> DefaultTooltipKey = (propertyInfo, propertyType) =>
    {
        var translateProperty = TranslateProperty(propertyInfo, propertyType);
        return translateProperty.ResourceKey.Replace(".Field.", ".Tooltip.");
    };

    /// <summary>
    /// When using <see cref="UICSpanAttribute"/> without a key, this is the key that will be used
    /// </summary>
    public static Func<PropertyInfo, UICPropertyType, string> DefaultInfoSpanKey = (propertyInfo, propertyType) =>
    {
        var translateProperty = TranslateProperty(propertyInfo, propertyType);
        return translateProperty.ResourceKey.Replace(".Field.", ".Info.");
    };
}
