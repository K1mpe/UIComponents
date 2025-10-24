using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMaxValue<TValue> : IUICPropertyValidationRuleMaxValue<TValue> where TValue : struct, IComparable
{
    public Func<PropertyInfo, object,CancellationToken, Task<Nullable<TValue>>> MaxValueFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<Nullable<TValue>> MaxValue(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default)
    {
        if (MaxValueFunc == null)
            throw new ArgumentNullException(nameof(MaxValueFunc));

        return MaxValueFunc(propertyInfo, obj, cancellationToken);
    }

}