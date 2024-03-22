using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMaxLength : IUICPropertyValidationRuleMaxLength
{
    public Func<PropertyInfo, object, Task<int?>> MaxLengthFunc { get; set; }

    public Type? PropertyType => typeof(object);


    public Task<int?> MaxLength(PropertyInfo propertyInfo, object obj)
    {
        if (MaxLengthFunc == null)
            throw new ArgumentNullException(nameof(MaxLengthFunc));

        return MaxLengthFunc(propertyInfo, obj);
    }
}