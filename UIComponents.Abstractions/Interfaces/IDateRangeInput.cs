namespace UIComponents.Abstractions.Interfaces;

public interface IDateRangeInput : IValueRange<DateTime?>
{

    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    /// <summary>
    /// Duration can only return a value if both <see cref="Start"/> and <see cref="End"/> are provided
    /// </summary>
    public TimeSpan? Duration
        {
            get
            {
                if (From.HasValue && To.HasValue)
                    return To.Value - From.Value;
                return null;
            }
        }


    DateTime? IValueRange<DateTime?>.From => From;
    DateTime? IValueRange<DateTime?>.To => To;

}
