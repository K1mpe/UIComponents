using System.Reflection;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IPropertyValidationRule
{
    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj);
}

public interface IPropertyValidationRule<T> : IPropertyValidationRule
{
    public Type GenericType => typeof(T);
    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj);
}

public interface IPropertyRequiredValidationRule : IPropertyValidationRule
{
    public Task<bool> IsRequired(PropertyInfo propertyInfo);
}

public interface IPropertyMinValueValidationRule<T, TValueType> : IPropertyValidationRule<T>
{
    public Task<TValueType?> MinValue(PropertyInfo propertyInfo);
}

public interface IPropertyMaxValueValidationRule<T, TValueType> : IPropertyValidationRule<T>
{
    public Task<TValueType> MaxValue(PropertyInfo propertyInfo);
}

public interface IPropertyMinLengthValidationRule : IPropertyValidationRule<string>
{
    public Task<int?> MinLenght(PropertyInfo propertyInfo);
}

public interface IPropertyMaxLengthValidationRule : IPropertyValidationRule<string>
{
    public Task<int?> MaxLength(PropertyInfo propertyInfo);
}