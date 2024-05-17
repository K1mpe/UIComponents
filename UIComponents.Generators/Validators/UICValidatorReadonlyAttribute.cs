using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using System.ComponentModel;

namespace UIComponents.Generators.Validators
{
    public class UICValidatorReadonlyAttribute : IUICPropertyValidationRuleReadonly
    {

        private readonly ILogger _logger;
        public UICValidatorReadonlyAttribute(ILogger<UICValidatorReadonlyAttribute> logger)
        {
            _logger = logger;
        }

        public Type? PropertyType => typeof(object);

        public async Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj)
        {
            await Task.Delay(0);
            if (!propertyInfo.CanRead)
            {
                _logger.LogDebug("{0} is readonly because it cannot be set", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
                return true;
            }
            var readonlyAttr = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>();
            if (readonlyAttr != null)
            {
                _logger.LogDebug($"{{0}} is readonly because of {nameof(RequiredAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");

                return readonlyAttr.IsReadOnly;
            }


            if (UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
            {
                var readonlyAttr2 = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>();
                if (readonlyAttr2 != null)
                {
                    _logger.LogDebug($"{{0}} is readonly because of {nameof(UICInheritAttribute)} => has {nameof(RequiredAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");

                    return readonlyAttr.IsReadOnly;
                }
            }
            return false;
        }

    }
}
