using UIComponents.Abstractions.DataTypes.RecurringDates;
using UIComponents.Abstractions.DataTypes.RecurringDates.Selectors;

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
    public List<Type> AllowedTypes { get; set; } = new List<Type>() { typeof(RecurringWeekly), typeof(RecurringMonthly), typeof(RecurringCustomDate) };


    /// <summary>
    /// Requires at least 1 included selector
    /// </summary>
    public bool ValidationRequired { get; set; }

    /// <summary>
    /// The endtime is required for selectors
    /// </summary>
    public bool ValidationEndRequired { get; set; }
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
