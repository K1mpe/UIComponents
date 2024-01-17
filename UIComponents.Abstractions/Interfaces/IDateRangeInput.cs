namespace UIComponents.Abstractions.Interfaces;

public interface IDateRangeInput
{

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

    /// <summary>
    /// Duration can only return a value if both <see cref="Start"/> and <see cref="End"/> are provided
    /// </summary>
    public TimeSpan? Duration
        {
            get
            {
                if (Start.HasValue && End.HasValue)
                    return End.Value - Start.Value;
                return null;
            }
        }

}
