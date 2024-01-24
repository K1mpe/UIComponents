using System.Reflection;

namespace UIComponents.Defaults;

public static class TranslationDefaults
{

    public static ITranslateable ButtonEdit = new Translatable("Button.Edit");
    public static ITranslateable ButtonReadonly = new Translatable("Button.Readonly");

    public static ITranslateable ButtonCreate = new Translatable("Button.Create");
    public static Func<Type, ITranslateable> ButtonCreateTooltip = (type) => new Translatable("Button.Create.Tooltip", "Create a new {0}", TranslateType(type));

    public static ITranslateable ButtonSave = new Translatable("Button.Save");

    public static ITranslateable ButtonCancel = new Translatable("Button.Cancel");

    public static ITranslateable ButtonDelete = new Translatable("Button.Delete");

    public static ITranslateable ButtonRefresh = new Translatable("Button.Refresh");

    public static ITranslateable ButtonCardExpand = new Translatable("Button.Expand");
    public static ITranslateable ButtonCardCollapse = new Translatable("Button.Collapse");


    public static Func<object, ITranslateable> ButtonDeleteToolTip = (obj) => new Translatable("Button.Delete.Tooltip", "Delete this {0}", TranslateObject(obj));


    /// <summary>
    /// Create a translationmodel for a enum
    /// </summary>
    /// <remarks>
    /// Type: the type of the enum
    /// string: the value of this enum as string
    /// </remarks>
    public static Func<Type, string, ITranslateable> TranslateEnums = (type, value) => new Translatable($"Enum.{type.Name}.{value}");


    public static Func<Type, ITranslateable> TranslateType = (type) => new Translatable(type.Name);



    /// <summary>
    /// A Function that creates a ITranslateable for a object
    /// </summary>
    public static Func<object, ITranslateable> TranslateObject = (obj) =>
    {
        string toString = obj.ToString();
        if (toString != obj.GetType().FullName)
            return new Untranslated(toString);

        var translatedType = TranslateType(obj.GetType());
        if (obj is IDbEntity dbEntity)
            return new Translatable("DbEntity", "{0} {1}", translatedType, dbEntity.Id);

        return translatedType;
    };

    public static Func<PropertyInfo, UICPropertyType, ITranslateable> TranslateProperty = (prop, uicPropType) =>
    {
        if (uicPropType == UICPropertyType.SelectList && prop.Name.EndsWith("Id") && prop.Name != "Id")
            return new Translatable($"{prop.DeclaringType!.Name}.Field.{prop.Name.Substring(0, prop.Name.Length - 2)}");

        return new Translatable($"{prop.DeclaringType!.Name}.Field.{prop.Name}");
    };



    public static Func<string, ITranslateable> ValidationIsRequired = (translatedPropertyName) => new Translatable("Validation.Required", "{0} is required", translatedPropertyName);

    public static Func<string, int, ITranslateable> ValidateMinLength = (translatedPropertyName, minLenght) => new Translatable("Validation.MinLength", "The value of {0} must be longer than {1}", translatedPropertyName, minLenght);
    public static Func<string, int, ITranslateable> ValidateMaxLength = (translatedPropertyName, maxLength) => new Translatable("Validation.MaxLength", "The value of {0} must be shorter than {1}", translatedPropertyName, maxLength);
    public static Func<string, object, ITranslateable> ValidateMinValue = (translatedPropertyName, minValue) => new Translatable("Validation.MinValue", "The value of {0} must be higher than or equal to {1}", translatedPropertyName, minValue);
    public static Func<string, object, ITranslateable> ValidateMaxValue = (translatedPropertyName, maxValue) => new Translatable("Validation.MaxValue", "The value of {0} must be lower or equal to {1}", translatedPropertyName, maxValue);
    public static Func<string, ITranslateable> ValidateColor = (translatedPropertyName) => new Translatable("Validation.Color.Invalid", "{0} has a invalid color", translatedPropertyName);

    public static Func<int, ITranslateable> FileUploadMaxFiles = (fileCount) =>
    {
        if (fileCount == 1)
            return new Translatable("Fileupload.OneFile", "Upload only one file");
        else return new Translatable("FileUpload.MaxCountFiles", "Upload up to {0} files", fileCount);
    };
    public static ITranslateable SelectListNoItems = new Translatable("SelectList.NoItemsAvailable", "No items available");
}
