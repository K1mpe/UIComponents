using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyRequired : IUICPropertyValidationRuleRequired
{

    public Func<PropertyInfo,object,Task<bool>> IsRequiredFunc { get; set; }

    public Type? PropertyType => typeof(object);


    public Task<bool> IsRequired(PropertyInfo propertyInfo, object obj)
    {
        if (IsRequiredFunc == null)
            throw new ArgumentNullException(nameof(IsRequiredFunc));

        return IsRequiredFunc(propertyInfo, obj);
    }
}
