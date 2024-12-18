using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators;

public class UICValidatorMinMaxOfTypeByte : IUICPropertyValidationRuleMinValue<byte>, IUICPropertyValidationRuleMaxValue<byte>
{
    public Type? PropertyType => typeof(byte);

    public Task<byte?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(byte.MaxValue as byte?);

    public Task<byte?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(byte.MinValue as byte?);
}
public class UICValidatorMinMaxOfTypeShort : IUICPropertyValidationRuleMinValue<short>, IUICPropertyValidationRuleMaxValue<short>
{
    public Type? PropertyType => typeof(short);

    public Task<short?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(short.MaxValue as short?);

    public Task<short?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(short.MinValue as short?);
}
public class UICValidatorMinMaxOfTypeInt : IUICPropertyValidationRuleMinValue<int>, IUICPropertyValidationRuleMaxValue<int>
{
    public Type? PropertyType => typeof(int);

    public Task<int?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(int.MaxValue as int?);

    public Task<int?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(int.MinValue as int?);
}
public class UICValidatorMinMaxOfTypelong : IUICPropertyValidationRuleMinValue<long>, IUICPropertyValidationRuleMaxValue<long>
{
    public Type? PropertyType => typeof(long);

    public Task<long?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(long.MaxValue as long?);

    public Task<long?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(long.MinValue as long?);
}
public class UICValidatorMinMaxOfTypeFloat : IUICPropertyValidationRuleMinValue<float>, IUICPropertyValidationRuleMaxValue<float>
{
    public Type? PropertyType => typeof(float);

    public Task<float?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(float.MaxValue as float?);

    public Task<float?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(float.MinValue as float?);
}
public class UICValidatorMinMaxOfTypeDouble : IUICPropertyValidationRuleMinValue<double>, IUICPropertyValidationRuleMaxValue<double>
{
    public Type? PropertyType => typeof(double);

    public Task<double?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(double.MaxValue as double?);

    public Task<double?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(double.MinValue as double?);
}
public class UICValidatorMinMaxOfTypeDecimal : IUICPropertyValidationRuleMinValue<decimal>, IUICPropertyValidationRuleMaxValue<decimal>
{
    public Type? PropertyType => typeof(decimal);

    public Task<decimal?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(decimal.MaxValue as decimal?);

    public Task<decimal?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(decimal.MinValue as decimal?);
}

public class UICValidatorMinMaxOfTypeUShort : IUICPropertyValidationRuleMinValue<ushort>, IUICPropertyValidationRuleMaxValue<ushort>
{
    public Type? PropertyType => typeof(ushort);

    public Task<ushort?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(ushort.MaxValue as ushort?);

    public Task<ushort?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(ushort.MinValue as ushort?);
}
public class UICValidatorMinMaxOfTypeUInt : IUICPropertyValidationRuleMinValue<uint>, IUICPropertyValidationRuleMaxValue<uint>
{
    public Type? PropertyType => typeof(uint);

    public Task<uint?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(uint.MaxValue as uint?);

    public Task<uint?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(uint.MinValue as uint?);
}
public class UICValidatorMinMaxOfTypeUlong : IUICPropertyValidationRuleMinValue<ulong>, IUICPropertyValidationRuleMaxValue<ulong>
{
    public Type? PropertyType => typeof(ulong);

    public Task<ulong?> MaxValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(ulong.MaxValue as ulong?);

    public Task<ulong?> MinValue(PropertyInfo propertyInfo, object obj) => Task.FromResult(ulong.MinValue as ulong?);
}