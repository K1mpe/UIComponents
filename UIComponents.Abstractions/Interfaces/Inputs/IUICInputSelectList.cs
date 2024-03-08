
namespace UIComponents.Abstractions.Interfaces.Inputs;

public interface IUICInputSelectList : IUICHasAttributes
{
    public object Value { get; }

    public Translatable NoItemsText { get; }
}
