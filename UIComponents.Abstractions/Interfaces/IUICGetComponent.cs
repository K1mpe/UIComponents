namespace UIComponents.Abstractions.Interfaces;


/// <summary>
/// A class with this interface contains a way to get a <see cref="IUIComponent"/> and will not be rendered by themself 
/// </summary>
public interface IUICGetComponent : IUIComponent
{
    string IUIComponent.RenderLocation => throw new NotImplementedException();

    /// <summary>
    /// Provide a IUIComponent that is used in for rendering
    /// </summary>
    public Task<IUIComponent> GetComponentAsync(IServiceProvider serviceProvider);
}

