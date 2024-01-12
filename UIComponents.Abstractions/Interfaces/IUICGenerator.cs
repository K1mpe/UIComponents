namespace UIComponents.Abstractions.Interfaces;

public interface IUICGenerator
{

    /// <summary>
    /// Priority of the handler, Lower value goes first
    /// </summary>
    public double Priority { get; }

    /// <summary>
    /// A name for the handler, only used for debugging
    /// </summary>
    public string Name { get; }
}

public interface IUICGenerator<TArgs, TResult> : IUICGenerator
{

    /// <summary>
    /// The function that handles the IncommingArgs and possibly a existing result
    /// </summary>
    public Func<TArgs, TResult?, Task<IUICGeneratorResponse<TResult>>> GetResult { get; }
}
