using System.Reflection;

namespace UIComponents.Defaults;

public static class TranslationDefaults
{

    public static ITranslateable ButtonEdit = new TranslationModel("Button.Edit");
    public static ITranslateable ButtonReadonly = new TranslationModel("Button.Readonly");

    public static ITranslateable ButtonCreate = new TranslationModel("Button.Create");
    public static Func<Type, ITranslateable> ButtonCreateTooltip = (type) => new TranslationModel("Button.Create.Tooltip", "Create a new {0}", TranslateType(type));

    public static ITranslateable ButtonSave = new TranslationModel("Button.Save");

    public static ITranslateable ButtonCancel = new TranslationModel("Button.Cancel");

    public static ITranslateable ButtonDelete = new TranslationModel("Button.Delete");
    public static Func<object, ITranslateable> ButtonDeleteToolTip = (obj) => new TranslationModel("Button.Delete.Tooltip", "Delete this {0}", TranslateObject(obj));


    /// <summary>
    /// Create a translationmodel for a enum
    /// </summary>
    /// <remarks>
    /// Type: the type of the enum
    /// string: the value of this enum as string
    /// </remarks>
    public static Func<Type, string, ITranslateable> TranslateEnums = (type, value) => new TranslationModel($"Enum.{type.Name}.{value}");

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
            return new TranslationModel("DbEntity", "{0} {1}", translatedType, dbEntity.Id);

        return translatedType;
    };

    public static Func<PropertyInfo, UICPropertyType, ITranslateable> TranslateProperty = (prop, uicPropType) =>
    {
        if (uicPropType == UICPropertyType.SelectList && prop.Name.EndsWith("Id") && prop.Name != "Id")
            return new TranslationModel($"{prop.DeclaringType!.Name}.Field.{prop.Name.Substring(0, prop.Name.Length - 2)}");

        return new TranslationModel($"{prop.DeclaringType!.Name}.Field.{prop.Name}");
    };

    public static Func<Type, ITranslateable> TranslateType = (type) => new TranslationModel(type.Name);


    public static Func<string, ITranslateable> ValidationIsRequired = (translatedPropertyName) => new TranslationModel("Validation.Required", "{0} is required", translatedPropertyName);

    public static Func<string, int, ITranslateable> ValidateMinLength = (translatedPropertyName, minLenght) => new TranslationModel("Validation.MinLength", "The value of {0} must be longer than {1}", translatedPropertyName, minLenght);
    public static Func<string, int, ITranslateable> ValidateMaxLength = (translatedPropertyName, maxLength) => new TranslationModel("Validation.MaxLength", "The value of {0} must be shorter than {1}", translatedPropertyName, maxLength);
    public static Func<string, object, ITranslateable> ValidateMinValue = (translatedPropertyName, minValue) => new TranslationModel("Validation.MinValue", "The value of {0} must be higher than or equal to {1}", translatedPropertyName, minValue);
    public static Func<string, object, ITranslateable> ValidateMaxValue = (translatedPropertyName, maxValue) => new TranslationModel("Validation.MaxValue", "The value of {0} must be lower or equal to {1}", translatedPropertyName, maxValue);
    
}
