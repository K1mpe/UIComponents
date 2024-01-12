namespace UIComponents.Abstractions.Interfaces;

public interface IUICGeneratorResponse
{

    /// <summary>
    /// The current handler has a successfull response
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// If true, the next hanlder will be used.
    /// </summary>
    bool Continue { get; }
}

public interface IUICGeneratorResponse<out TResult> : IUICGeneratorResponse
{
    /// <summary>
    /// The result if <see cref="IUICGeneratorResponse.Success"/> is true
    /// </summary>
    TResult? Result { get; }
}