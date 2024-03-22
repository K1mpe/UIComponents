using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMinLength : IUICPropertyValidationRuleMinLength
{
    
    public Func<PropertyInfo, object, Task<int?>> MinLengthFunc { get; set; }

    public Type? PropertyType => typeof(object);


    public Task<int?> MinLength(PropertyInfo propertyInfo, object obj)
    {
        if (MinLengthFunc == null)
            throw new ArgumentNullException(nameof(MinLengthFunc));

        return MinLengthFunc(propertyInfo, obj);
    }
}