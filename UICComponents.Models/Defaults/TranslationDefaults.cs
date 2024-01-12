using System.Reflection;

namespace UIComponents.ComponentModels.Defaults;

public static class TranslationDefaults
{
    public static ITranslationModel ButtonEdit = new TranslationModel("Button.Edit");
    public static ITranslationModel ButtonReadonly = new TranslationModel("Button.Readonly");

    public static ITranslationModel ButtonCreate = new TranslationModel("Button.Create");
    public static Func<Type, ITranslationModel> ButtonCreateTooltip = (type) => new TranslationModel("Button.Create.Tooltip", "Create a new {0}", TranslateType(type));

    public static ITranslationModel ButtonSave = new TranslationModel("Button.Save");

    public static ITranslationModel ButtonCancel= new TranslationModel("Button.Cancel");

    public static ITranslationModel ButtonDelete = new TranslationModel("Button.Delete");
    public static Func<object, ITranslationModel> ButtonDeleteToolTip = (obj) => new TranslationModel("Button.Delete.Tooltip", "Delete this {0}", TranslateObject(obj));


     /// <summary>
     /// Create a translationmodel for a enum
     /// </summary>
     /// <remarks>
     /// Type: the type of the enum
     /// string: the value of this enum as string
     /// </remarks>
    public static Func<Type, string, ITranslationModel> TranslateEnums= (type, value) => new TranslationModel($"Enum.{type.Name}.{value}");

    /// <summary>
    /// A Function that creates a ITranslationModel for a object
    /// </summary>
    public static Func<object, ITranslationModel> TranslateObject = (obj) =>
    {
        string toString = obj.ToString();
        if (toString != obj.GetType().FullName)
            return new Untranslated(toString);

        var translatedType = TranslateType(obj.GetType());
        if (obj is IDbEntity dbEntity)
            return new TranslationModel("DbEntity", "{0} {1}", translatedType, dbEntity.Id);

        return translatedType;
    };

    public static Func<PropertyInfo, UICPropertyType, ITranslationModel> TranslateProperty = (prop, uicPropType) =>
    {
        if (uicPropType == UICPropertyType.SelectList && prop.Name.EndsWith("Id") && prop.Name != "Id")
            return new TranslationModel($"{prop.DeclaringType!.Name}.Field.{prop.Name.Substring(0, prop.Name.Length - 2)}");

        return new TranslationModel($"{prop.DeclaringType!.Name}.Field.{prop.Name}");
    };

    public static Func<Type, ITranslationModel> TranslateType = (type) => new TranslationModel(type.Name);

    public static Func<string, ITranslationModel> ValidationIsRequired = (propertyName) => new TranslationModel("Validation.Required", "{0} is required", propertyName);
}
