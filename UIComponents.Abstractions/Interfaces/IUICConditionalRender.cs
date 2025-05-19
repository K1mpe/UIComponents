namespace UIComponents.Abstractions.Interfaces;

public interface IUICConditionalRender
{
    /// <summary>
    /// The component will not be rendered when this is false
    /// </summary>
    public bool Render { get; }
}
