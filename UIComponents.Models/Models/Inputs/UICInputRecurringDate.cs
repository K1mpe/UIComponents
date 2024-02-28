using UIComponents.Abstractions.Models.RecurringDates;
using UIComponents.Abstractions.Models.RecurringDates.Selectors;

namespace UIComponents.Models.Models.Inputs;

public class UICInputRecurringDate : UICInput<RecurringDate>
{

    #region Fields
    public override bool HasClientSideValidation => false;
    #endregion

    #region Ctor
    public UICInputRecurringDate(string propertyName) : base(propertyName)
    {
    }
    #endregion

    #region Properties
    public List<Type> AllowedTypes { get; set; } = new List<Type>() { typeof(RecurringWeekly), typeof(RecurringMonthly) };

    public Translatable Title { get; set; } = new("RecurringDate");

    public bool EndTimeRequired { get; set; }
    #endregion


    #region Methods
    /// <summary>
    /// Add a type to the allowed types
    /// </summary>
    public UICInputRecurringDate AddType(Type type)
    {
        if (!type.IsAssignableTo(typeof(IRecurringDateSelector)))
            throw new ArgumentException($"{type.Name} is not assignable to {nameof(IRecurringDateSelector)}");

        if(!AllowedTypes.Contains(type))
            AllowedTypes.Add(type);

        return this;
    }

    /// <summary>
    /// Remove a type from the allowed types
    /// </summary>
    public UICInputRecurringDate RemoveType(Type type)
    {
        if (!type.IsAssignableTo(typeof(IRecurringDateSelector)))
            throw new ArgumentException($"{type.Name} is not assignable to {nameof(IRecurringDateSelector)}");
        AllowedTypes.Remove(type);
        return this;
    }
    #endregion

}
