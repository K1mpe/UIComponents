namespace UIComponents.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FakeForeignKeyAttribute : Attribute
{
    public FakeForeignKeyAttribute(Type type, bool isRequired = true)
    {
        Type = type;
        IsRequired = isRequired;
    }

    public Type Type { get; }
    public bool IsRequired { get; }
}
