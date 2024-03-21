using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators;

public abstract class UICValidatorRangeAttribute<T>: IUICPropertyValidationRuleMinValue<T>, IUICPropertyValidationRuleMaxValue<T> where T : struct, IComparable
{

    public Type? PropertyType => typeof(T);

    public async Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        var result = new ValidationRuleResult();

        var minResult = await IUICPropertyValidationRuleMinValue<T>.DefaultValidationErrors(this, propertyInfo, obj);
        result.ImportErrors(minResult);
        var maxResult = await IUICPropertyValidationRuleMaxValue<T>.DefaultValidationErrors(this, propertyInfo, obj);
        result.ImportErrors(maxResult);
        return result;
            
    }

    public Task<T?> MaxValue(PropertyInfo propertyInfo, object obj)
    {
        var rangeAttr = propertyInfo.GetCustomAttribute<RangeAttribute>();
        if (rangeAttr != null)
            return Task.FromResult((T?)rangeAttr.Minimum);
        return Task.FromResult(default(T?));
    }

    public Task<T?> MinValue(PropertyInfo propertyInfo, object obj)
    {
        var rangeAttr = propertyInfo.GetCustomAttribute<RangeAttribute>();
        if (rangeAttr != null)
            return Task.FromResult((T?)rangeAttr.Minimum);
        return Task.FromResult(default(T?));
    }
}

public class UICValidatorRangeAttributeInt : UICValidatorRangeAttribute<int> { }
public class UICValidatorRangeAttributeFloat : UICValidatorRangeAttribute<float> { }
public class UICValidatorRangeAttributeLong : UICValidatorRangeAttribute<long> { }
public class UICValidatorRangeAttributeDouble : UICValidatorRangeAttribute<double> { }
public class UICValidatorRangeAttributeDecimal : UICValidatorRangeAttribute<decimal> { }
public class UICValidatorRangeAttributeShort : UICValidatorRangeAttribute<short> { }
public class UICValidatorRangeAttributeDate : UICValidatorRangeAttribute<DateOnly> { }
public class UICValidatorRangeAttributeDateTime : UICValidatorRangeAttribute<DateTime> { }
public class UICValidatorRangeAttributeTimeOnly : UICValidatorRangeAttribute<TimeOnly> { }
public class UICValidatorRangeAttributeTimeSpan : UICValidatorRangeAttribute<TimeSpan> { }
