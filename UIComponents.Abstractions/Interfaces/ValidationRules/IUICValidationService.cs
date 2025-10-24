using System.Reflection;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IUICValidationService
{
    Task<ValidationRuleResult> ValidateObjectAsync(object obj, CancellationToken cancellationToken = default);

    Task<ValidationRuleResult> ValidateObjectProperty(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default);


    Task<bool> ValidatePropertyRequired(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default);

    Task<Nullable<TValueType>> ValidatePropertyMinValue<TValueType>(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default) where TValueType : struct, IComparable;
    Task<Nullable<TValueType>> ValidatePropertyMaxValue<TValueType>(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default) where TValueType : struct, IComparable;

    Task<int?> ValidatePropertyMinLength(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default);
    Task<int?> ValidatePropertyMaxLength(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default);

    Task<bool> ValidatePropertyReadonly(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default);

}
