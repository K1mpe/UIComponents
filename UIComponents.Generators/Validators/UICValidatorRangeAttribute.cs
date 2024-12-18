using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators;

public abstract class UICValidatorRangeAttribute<T>: IUICPropertyValidationRuleMinValue<T>, IUICPropertyValidationRuleMaxValue<T> where T : struct, IComparable
{
    private readonly ILogger _logger;

    protected UICValidatorRangeAttribute(ILogger logger, IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<T>> minValidator, IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<T>> maxValidator)
    {
        _logger = logger;
    }

    public Type? PropertyType => typeof(T);


    public Task<T?> MaxValue(PropertyInfo propertyInfo, object obj)
    {
        var rangeAttr = propertyInfo.GetInheritAttribute<RangeAttribute>();
        if (rangeAttr != null)
        {
            _logger.LogDebug($"{{0}} has maximum value by {nameof(RangeAttribute)}: {{1}}", $"{propertyInfo.DeclaringType?.Name}.{propertyInfo.Name}", rangeAttr.Maximum);
            return Task.FromResult((T?)rangeAttr.Maximum);
        }
        if(UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
        {
            rangeAttr = inherit.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                _logger.LogDebug($"{{0}} has maximum value by {nameof(RangeAttribute)}: {{1}} on Inherit property ({{2}})", $"{propertyInfo.DeclaringType?.Name}.{propertyInfo.Name}", rangeAttr.Maximum, $"{inherit.DeclaringType?.Name}.{inherit.Name}");
                return Task.FromResult((T?)rangeAttr.Maximum);
            }
        }
            
        return Task.FromResult(default(T?));
    }

    public Task<T?> MinValue(PropertyInfo propertyInfo, object obj)
    {
        var rangeAttr = propertyInfo.GetInheritAttribute<RangeAttribute>();
        if (rangeAttr != null)
        {
            _logger.LogDebug($"{{0}} has minimum value by {nameof(RangeAttribute)}: {{1}}", $"{propertyInfo.DeclaringType?.Name}.{propertyInfo.Name}", rangeAttr.Minimum);
            return Task.FromResult((T?)rangeAttr.Minimum);
        }
        if (UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
        {
            rangeAttr = inherit.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                _logger.LogDebug($"{{0}} has minimum value by {nameof(RangeAttribute)}: {{1}} on Inherit property ({{2}})", $"{propertyInfo.DeclaringType?.Name}.{propertyInfo.Name}", rangeAttr.Minimum, $"{inherit.DeclaringType?.Name}.{inherit.Name}");
                return Task.FromResult((T?)rangeAttr.Minimum);
            }
        }

        return Task.FromResult(default(T?));
    }
}

public class UICValidatorRangeAttributeByte : UICValidatorRangeAttribute<byte>
{
    public UICValidatorRangeAttributeByte(
        ILogger<UICValidatorRangeAttributeByte> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<byte>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<byte>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeInt : UICValidatorRangeAttribute<int>
{
    public UICValidatorRangeAttributeInt(
        ILogger<UICValidatorRangeAttributeInt> logger, 
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<int>> minValidator, 
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<int>> maxValidator) 
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeFloat : UICValidatorRangeAttribute<float>
{
    public UICValidatorRangeAttributeFloat(ILogger<UICValidatorRangeAttributeFloat> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<float>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<float>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeLong : UICValidatorRangeAttribute<long>
{
    public UICValidatorRangeAttributeLong(ILogger<UICValidatorRangeAttributeLong> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<long>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<long>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeDouble : UICValidatorRangeAttribute<double>
{
    public UICValidatorRangeAttributeDouble(ILogger<UICValidatorRangeAttributeDouble> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<double>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<double>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeDecimal : UICValidatorRangeAttribute<decimal>
{
    public UICValidatorRangeAttributeDecimal(ILogger<UICValidatorRangeAttributeDecimal> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<decimal>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<decimal>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeShort : UICValidatorRangeAttribute<short>
{
    public UICValidatorRangeAttributeShort(ILogger<UICValidatorRangeAttributeShort> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<short>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<short>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeDateOnly : UICValidatorRangeAttribute<DateOnly>
{
    public UICValidatorRangeAttributeDateOnly(ILogger<UICValidatorRangeAttributeDateOnly> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<DateOnly>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<DateOnly>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeDateTime : UICValidatorRangeAttribute<DateTime>
{
    public UICValidatorRangeAttributeDateTime(ILogger<UICValidatorRangeAttributeDateTime> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<DateTime>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<DateTime>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeTimeOnly : UICValidatorRangeAttribute<TimeOnly>
{
    public UICValidatorRangeAttributeTimeOnly(ILogger<UICValidatorRangeAttributeTimeOnly> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TimeOnly>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TimeOnly>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeTimeSpan : UICValidatorRangeAttribute<TimeSpan>
{
    public UICValidatorRangeAttributeTimeSpan(ILogger<UICValidatorRangeAttributeTimeSpan> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TimeSpan>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TimeSpan>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}

public class UICValidatorRangeAttributeUInt : UICValidatorRangeAttribute<uint>
{
    public UICValidatorRangeAttributeUInt(
        ILogger<UICValidatorRangeAttributeUInt> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<uint>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<uint>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeULong : UICValidatorRangeAttribute<ulong>
{
    public UICValidatorRangeAttributeULong(ILogger<UICValidatorRangeAttributeULong> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<ulong>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<ulong>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
public class UICValidatorRangeAttributeUShort : UICValidatorRangeAttribute<ushort>
{
    public UICValidatorRangeAttributeUShort(ILogger<UICValidatorRangeAttributeUShort> logger,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<ushort>> minValidator,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<ushort>> maxValidator)
        : base(logger, minValidator, maxValidator)
    {
    }
}
