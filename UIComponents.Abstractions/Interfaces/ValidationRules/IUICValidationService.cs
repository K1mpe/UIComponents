using System.Reflection;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IUICValidationService
{
    Task<ValidationRuleResult> ValidateObjectAsync(object obj);

    Task<ValidationRuleResult> ValidateObjectProperty(PropertyInfo propertyInfo, object obj);


    Task<bool> ValidatePropertyRequired(PropertyInfo propertyInfo, object obj);

    Task<Nullable<TValueType>> ValidatePropertyMinValue<TValueType>(PropertyInfo propertyInfo, object obj) where TValueType : struct, IComparable;
    Task<Nullable<TValueType>> ValidatePropertyMaxValue<TValueType>(PropertyInfo propertyInfo, object obj) where TValueType : struct, IComparable;

    Task<int?> ValidatePropertyMinLength(PropertyInfo propertyInfo, object obj);
    Task<int?> ValidatePropertyMaxLength(PropertyInfo propertyInfo, object obj);

    Task<bool> ValidatePropertyReadonly(PropertyInfo propertyInfo, object obj);

}
