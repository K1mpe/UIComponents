
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Models.UICGeneratorResponses;

public class UICGeneratorResponseSuccess<T> : IUICGeneratorResponse<T>
{
    public UICGeneratorResponseSuccess(T? result, bool @continue = false)
    {
        Result = result;
        Success = true;
        Continue = @continue;
    }

    public T? Result { get; set; }

    public bool Success { get; }

    public bool Continue { get; }
}
