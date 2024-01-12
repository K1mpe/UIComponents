using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators;

public abstract class UICGeneratorBase<TArgs, TResult> : IUICGenerator<TArgs, TResult>
{
    public Func<TArgs, TResult?, Task<IUICGeneratorResponse<TResult>>> GetResult => (args, existing) => CallResponseAsync(args, existing);

    public abstract double Priority { get; set; }

    public virtual string Name => GetType().Name;

    public abstract Task<IUICGeneratorResponse<TResult>> GetResponseAsync(TArgs args, TResult? existingResult);

    /// <summary>
    /// This method can be overwritten as a step before the <see cref="GetResponseAsync(TArgs)"/> is called
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    protected virtual Task<IUICGeneratorResponse<TResult>> CallResponseAsync(TArgs args, TResult? existingResult) => GetResponseAsync(args, existingResult);
}
