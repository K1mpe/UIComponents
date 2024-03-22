using System.Reflection;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IUICDefaultCheckValidationErrors<TValidator> where TValidator: IUICPropertyValidationRule
{
    Task<ValidationRuleResult> DefaultValidationErrors(TValidator validator, PropertyInfo propertyInfo, object obj);
}
