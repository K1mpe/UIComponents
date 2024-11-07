using System;
using System.Reflection;
using UIComponents.Defaults;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IUICPropertyValidationRule
{
    /// <summary>
    /// Only properties that are assignable to this type will use this validator. You can use Null or typeof(Object) if you want to use all properties
    /// </summary>
    public Type? PropertyType { get; }
}
public interface IUICPropertyValidationValidationResultsImplementation
{
    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj);
}

public interface IUICPropertyValidationRule<T> : IUICPropertyValidationRule
{
    public Type PropertyType => typeof(T);
}

public interface IUICPropertyValidationRuleRequired : IUICPropertyValidationRule
{
    public Task<bool> IsRequired(PropertyInfo propertyInfo, object obj);

}

public interface IUICPropertyValidationRuleMinValue<TValueType> : IUICPropertyValidationRule<TValueType> where TValueType : struct, IComparable
{
    Task<Nullable<TValueType>> MinValue(PropertyInfo propertyInfo, object obj);

}

public interface IUICPropertyValidationRuleMaxValue<TValueType> : IUICPropertyValidationRule<TValueType> where TValueType : struct, IComparable
{
    Task<Nullable<TValueType>> MaxValue(PropertyInfo propertyInfo, object obj);

}

public interface IUICPropertyValidationRuleMinLength : IUICPropertyValidationRule<string>
{
    Task<int?> MinLength(PropertyInfo propertyInfo, object obj);

}

public interface IUICPropertyValidationRuleMaxLength : IUICPropertyValidationRule<string>
{
    Task<int?> MaxLength(PropertyInfo propertyInfo, object obj);

}

public interface IUICPropertyValidationRuleReadonly : IUICPropertyValidationRule
{
    Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj);
}