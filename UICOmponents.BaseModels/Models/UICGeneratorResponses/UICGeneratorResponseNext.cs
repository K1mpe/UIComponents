using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Models.UICGeneratorResponses;

public class UICGeneratorResponseNext : IUICGeneratorResponse
{
    public UICGeneratorResponseNext()
    {
        Success = false;
        Continue = true;
    }

    public bool Success { get; init; }

    public bool Continue {get;init;}
}

public class UICGeneratorResponseNext<T> : UICGeneratorResponseNext, IUICGeneratorResponse<T>
{
    public T? Result => default(T?);

}
