using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Configuration;

namespace UIComponents.Generators.Validators
{
    public class UICValidatorEditPermission : IUICPropertyValidationRuleReadonly
    {
        private readonly ILogger _logger;
        private readonly UICConfig _config;
        public UICValidatorEditPermission(ILogger<UICValidatorEditPermission> logger, UICConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public Type? PropertyType => typeof(object);

        public async Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj)
        {
            bool readOnly = false;
            if (_config.TryGetPermissionService(out var permissionService))
            {
                readOnly = !await permissionService.CanEditProperty(obj, propertyInfo.Name);
                if(readOnly)
                    _logger.LogDebug($"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name} is readonly => There is no edit permission");
                
                
                if (!readOnly && UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
                {
                    var inheritInstance = Activator.CreateInstance(inherit.DeclaringType);
                    foreach (var property in propertyInfo.DeclaringType.GetProperties())
                    {
                        if (UICInheritAttribute.TryGetInheritPropertyInfo(property, out var x) && x.DeclaringType == inherit.DeclaringType)
                            x.SetValue(inheritInstance, property.GetValue(obj));
                    }

                    readOnly = !await permissionService!.CanEditProperty(inheritInstance, inherit.Name);
                    if(readOnly)
                        _logger.LogDebug($"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name} is readonly => There is no edit permission for inherit property ({inherit.DeclaringType.Name}.{inherit.Name})");
                }
            }
            return readOnly;
        }
    }
}
