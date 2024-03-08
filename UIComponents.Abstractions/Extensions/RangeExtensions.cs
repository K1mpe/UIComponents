namespace UIComponents.Abstractions.Extensions;

public static class RangeExtensions
{

    /// <summary>
    /// Check if the given value falls withing this range
    /// </summary>
    public static bool Contains<T>(this IValueRange<T> range, T value) where T : IComparable, IEquatable<T>
    {
        return range.From.CompareTo(value) <= 0 && range.To.CompareTo(value) >= 0;
    }

    /// <summary>
    /// Check if the includedRange falls completely within this range
    /// </summary>
    public static bool ContainsRange<T>(this IValueRange<T> range, IValueRange<T> includedRange) where T : IComparable
    {
        if (range.From.CompareTo(includedRange.From) > 0)
            return false;
        if(range.To.CompareTo(includedRange.To) < 0)
            return false;
        return true;
    }


    /// <summary>
    /// Check if this range has a partial match with another range
    /// </summary>
    public static bool HasOverlap<T>(this IValueRange<T> range, IValueRange<T> comparingRange) where T : IComparable
    {
        if (range.From.CompareTo(comparingRange.From) > 0 && range.From.CompareTo(comparingRange.To) <0)
            return true;
        if (range.To.CompareTo(comparingRange.From) > 0 && range.To.CompareTo(comparingRange.To) < 0)
            return true;
        return false;
    }

    /// <summary>
    /// Combines <see cref="ContainsRange{T}(IValueRange{T}, IValueRange{T})"/> and <see cref="HasOverlap{T}(IValueRange{T}, IValueRange{T})"/>
    /// </summary>
    public static bool ContainsOrHasOverlap<T>(this IValueRange<T> range, IValueRange<T> comparingRange) where T: IComparable
    {
        return range.ContainsRange(comparingRange) || range.HasOverlap(comparingRange);
    }

}
