using UIComponents.Abstractions.DataTypes;

namespace UIComponents.Abstractions.Extensions;

public static class RangeExtensions
{

    /// <summary>
    /// Check if the given value falls withing this range
    /// </summary>
    public static bool Contains<T>(this IValueRange<T> range, T value) where T : IComparable
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

    /// <summary>
    /// [From] -> [To]
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public static string AsString(this IValueRange range)
    {
        return $"{range.From.ToString()} -> {range.To.ToString()}";
    }

    /// <summary>
    /// Take the smallest start and the largest end of these 2 ranges
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="otherRange"></param>
    /// <param name="throwIfNoOverlap">throw a exception if these 2 ranges have no overlap</param>
    /// <returns></returns>
    public static ValueRange<T> ExpandWith<T>(this IValueRange<T> source, IValueRange<T> otherRange, bool throwIfNoOverlap) where T : IComparable
    {
        if (throwIfNoOverlap && !HasOverlap(source, otherRange))
            throw new Exception($"The source range ({source.AsString()}) and other range ({otherRange.AsString()}) have no overlap");

        var newRange = new ValueRange<T>();
        if(source.From.CompareTo(otherRange.From)>0)
            newRange.From = otherRange.From;
        else 
            newRange.From = source.From;

        if (source.To.CompareTo(otherRange.To) < 0)
            newRange.To = otherRange.To;
        else
            newRange.To = source.To;

        return newRange;
    }

    /// <summary>
    /// Take the overlapping range from the 2 sources. This will throw a exception if there is no overlap!
    /// </summary>
    /// <param name="source"></param>
    /// <param name="otherRange"></param>
    /// <returns></returns>
    public static ValueRange<T> TakeOverlap<T>(this IValueRange<T> source, IValueRange<T> otherRange) where T : IComparable
    {
        if (!HasOverlap(source, otherRange))
            throw new Exception($"The source range ({source.AsString()}) and other range ({otherRange.AsString()}) have no overlap");

        var newRange = new ValueRange<T>();
        if (source.From.CompareTo(otherRange.From) < 0)
            newRange.From = otherRange.From;
        else
            newRange.From = source.From;

        if (source.To.CompareTo(otherRange.To) > 0)
            newRange.To = otherRange.To;
        else
            newRange.To = source.To;

        return newRange;
    }
}
