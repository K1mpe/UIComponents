using System.ComponentModel;
using System.Reflection;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Varia;

namespace UIComponents.Defaults;

public static class TranslationDefaults
{

    public static Translatable ButtonEdit = TranslatableSaver.Save("Button.Edit");
    public static Translatable ButtonReadonly = TranslatableSaver.Save("Button.Readonly");

    public static Translatable ButtonCreate = TranslatableSaver.Save("Button.Create");
    public static Func<Type, Translatable> ButtonCreateTooltip = (type) => TranslatableSaver.Save("Button.Create.Tooltip", "Create a new {0}", TranslateType(type));

    public static Translatable ButtonSave = TranslatableSaver.Save("Button.Save");

    public static Translatable ButtonCancel = TranslatableSaver.Save("Button.Cancel");

    public static Translatable ButtonDelete = TranslatableSaver.Save("Button.Delete");

    public static Translatable ButtonRefresh = TranslatableSaver.Save("Button.Refresh");

    public static Translatable ButtonCardExpand = TranslatableSaver.Save("Button.Expand");
    public static Translatable ButtonCardCollapse = TranslatableSaver.Save("Button.Collapse");


    public static Func<object, Translatable> ButtonDeleteToolTip = (obj) => TranslatableSaver.Save("Button.Delete.Tooltip", "Delete this {0}", TranslateObject(obj));


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
    /// A function simular to <see cref="TranslateType"/>, but this one is used for multiple types
    /// </summary>
    public static Func<Type, Translatable> TranslateTypeMultiple = (type) =>
    {
        string key = type.Name+"+";
        List<Translatable> args = new();
        string defaultValue = type.Name+"s";

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
            return TranslatableSaver.Save("DbEntity", "{0} {1}", translatedType, dbEntity.Id);

        return translatedType;
    };

    public static Func<PropertyInfo, UICPropertyType?, Translatable> TranslateProperty = (prop, uicPropType) =>
    {
        var translateAttr = prop.GetInheritAttribute<DisplayNameAttribute>();
        if (translateAttr != null)
            return translateAttr.DisplayName;

        UICInheritAttribute.TryGetInheritPropertyInfo(prop, out var inheritProp);
        
        if (uicPropType != null && uicPropType == UICPropertyType.SelectList && inheritProp.Name.EndsWith("Id") && inheritProp.Name != "Id")
            return new Translatable($"{inheritProp.DeclaringType!.Name}.Field.{inheritProp.Name.Substring(0, inheritProp.Name.Length - 2)}");

        return new Translatable($"{inheritProp.DeclaringType!.Name}.Field.{inheritProp.Name}");
    };




    /// <summary>
    /// Function takes a already translated propertyName and creates a validation message
    /// </summary>
    public static Func<Translatable, Translatable> ValidationIsRequired = (translatedPropertyName) => TranslatableSaver.Save("Validation.Required", "{0} is required", translatedPropertyName);

    public static Func<Translatable, int, Translatable> ValidateMinLength = (translatedPropertyName, minLenght) => TranslatableSaver.Save("Validation.MinLength", "The length of {0} must be longer or equal to {1}", translatedPropertyName, minLenght);
    public static Func<Translatable, int, Translatable> ValidateMaxLength = (translatedPropertyName, maxLength) => TranslatableSaver.Save("Validation.MaxLength", "The length of {0} must be shorter or equal to {1}", translatedPropertyName, maxLength);
    public static Func<Translatable, object, Translatable> ValidateMinValue = (translatedPropertyName, minValue) => TranslatableSaver.Save("Validation.MinValue", "The value of {0} must be higher than or equal to {1}", translatedPropertyName, minValue);
    public static Func<Translatable, object, Translatable> ValidateMaxValue = (translatedPropertyName, maxValue) => TranslatableSaver.Save("Validation.MaxValue", "The value of {0} must be lower or equal to {1}", translatedPropertyName, maxValue);
    public static Func<Translatable, Translatable> ValidateColor = (translatedPropertyName) => TranslatableSaver.Save("Validation.Color.Invalid", "{0} has a invalid color", translatedPropertyName);

    public static Func<int, Translatable> FileUploadMaxFiles = (fileCount) =>
    {
        if (fileCount == 1)
            return TranslatableSaver.Save("Fileupload.OneFile", "Upload only one file");
        else return TranslatableSaver.Save("FileUpload.MaxCountFiles", "Upload up to {0} files", fileCount);
    };
    public static Translatable SelectListNoItems = TranslatableSaver.Save("SelectList.NoItemsAvailable", "No items available");


    /// <summary>
    /// When using <see cref="UICTooltipAttribute"/> without a key, this is the key that will be used
    /// </summary>
    public static Func<PropertyInfo, UICPropertyType, string> DefaultTooltipKey = (propertyInfo, propertyType) =>
    {
        var translateProperty = TranslateProperty(propertyInfo, propertyType);
        if (translateProperty.ResourceKey == (new Untranslated("")).ResourceKey)
            return $"{propertyInfo.DeclaringType.Name}.ToolTip.{propertyInfo.Name}";
        return translateProperty.ResourceKey.Replace(".Field.", ".Tooltip.");
    };

    /// <summary>
    /// When using <see cref="UICSpanAttribute"/> without a key, this is the key that will be used
    /// </summary>
    public static Func<PropertyInfo, UICPropertyType, string> DefaultInfoSpanKey = (propertyInfo, propertyType) =>
    {
        var translateProperty = TranslateProperty(propertyInfo, propertyType);
        if (translateProperty.ResourceKey == (new Untranslated("").ResourceKey))
            return $"{propertyInfo.DeclaringType.Name}.Info.{propertyInfo.Name}";
        return translateProperty.ResourceKey.Replace(".Field.", ".Info.");
    };
}
