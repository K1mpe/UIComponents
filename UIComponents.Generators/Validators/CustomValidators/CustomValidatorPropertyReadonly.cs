using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyReadonly : IUICPropertyValidationRuleReadonly
{
    public Func<PropertyInfo, object, CancellationToken, Task<bool>> IsReadonlyFunc { get; set; }

    public Type? PropertyType => typeof(object);


    public Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj, CancellationToken cancellationToken = default)
    {
        if (IsReadonlyFunc == null)
            throw new ArgumentNullException(nameof(IsReadonlyFunc));

        return IsReadonlyFunc(propertyInfo, obj, cancellationToken);
    }

}