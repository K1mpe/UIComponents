using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMaxValue<TValue> : IUICPropertyValidationRuleMaxValue<TValue> where TValue : struct, IComparable
{
    public Func<PropertyInfo, object, Task<Nullable<TValue>>> MaxValueFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<Nullable<TValue>> MaxValue(PropertyInfo propertyInfo, object obj)
    {
        if (MaxValueFunc == null)
            throw new ArgumentNullException(nameof(MaxValueFunc));

        return MaxValueFunc(propertyInfo, obj);
    }

}