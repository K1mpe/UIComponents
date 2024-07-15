namespace UIComponents.Abstractions.DataTypes;


public interface IValueRange
{
    public object From { get; }
    public object To { get; }
}
public interface IValueRange<T> : IValueRange 
{
    public T From { get; }
    public T To { get; }

    public T Start => From;
    public T End => To;

    object IValueRange.From => From;
    object IValueRange.To => To;
}

public class ValueRange<T> : IValueRange<T> where T : IComparable
{
    #region Ctor
    public ValueRange()
    {

    }
    public ValueRange(T from, T to)
    {
        From = from;
        To = to;
    }

    #endregion

    #region Properties
    /// <summary>
    /// The lowest value from the range, same as <see cref="Start"/>
    /// </summary>
    public T From { get; set; }

    /// <summary>
    /// The highest value from the range, same as <see cref="End"/>
    /// </summary>
    public T To { get; set; }

    /// <summary>
    /// The lowest value from the range, same as <see cref="From"/>
    /// </summary>
    public T Start
    {
        get { return From; }
        set { From = value; }
    }

    /// <summary>
    /// The highest value from the range, same as <see cref="To"/>
    /// </summary>
    public T End
    {
        get { return To; }
        set { To = value; }
    }
    #endregion


    #region Methods
    public override string ToString() => this.AsString();
    #endregion

}
