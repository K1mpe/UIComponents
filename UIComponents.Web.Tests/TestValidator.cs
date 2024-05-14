using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Web.Tests
{
    public class TestValidator : IUICPropertyValidationRuleReadonly
    {
        public Type? PropertyType => throw new NotImplementedException();

        public Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj)
        {
            throw new NotImplementedException();
        }
    }
}
