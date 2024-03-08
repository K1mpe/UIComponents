namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IObjectValidationRule
{
    public Task<ValidationRuleResult> CheckValidationErrors(object obj); 
}
public interface IObjectValidationRule<T> : IObjectValidationRule where T : class
{
    public Task<ValidationRuleResult> CheckValidationErrors(T obj);

    Task<ValidationRuleResult> IObjectValidationRule.CheckValidationErrors(object obj)
    {
        if(obj is T tObj)
            return CheckValidationErrors(tObj);
        throw new ArgumentException($"{obj.GetType().Name} is not assignable to {typeof(T).Name}");
    }
}
