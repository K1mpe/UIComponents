using Microsoft.Extensions.DependencyInjection;

namespace UIComponents.Models.Models;


/// <summary>
/// This model contains a <see cref="UICFactory"/> to create a component.
/// <br>This interface inherits <see cref="IUIComponent"/> so it can be invoked and is ignored by generators.</br>
/// </summary>
public interface IUICUseFactory : IUICGetComponent
{
    Task<IUIComponent> IUICGetComponent.GetComponentAsync(IServiceProvider provider) => FactoryComponent.GetComponentAsync(provider);


    public UICFactory FactoryComponent { get; }
}

/// <summary>
/// Do not use this directly, use <see cref="UICFactory{TFactory}"/> instead
/// </summary>
public abstract class UICFactory : IUICGetComponent
{
    public Type FactoryType { get; protected set; }
    public abstract Task<IUIComponent> GetComponentAsync(IServiceProvider provider);
}


/// <summary>
/// A factory takes a type of factory and invokes a method that should return a <see cref="IUIComponent"/>
/// </summary>
/// <typeparam name="TFactory"></typeparam>
public class UICFactory<TFactory> : UICFactory where TFactory : class
{
    public UICFactory(Func<TFactory, Task<IUIComponent>>func)
    {
        FactoryType = typeof(TFactory);
        Function = func;
    }

    public UICFactory(string viewPath, Func<TFactory, Task<object>> func)
    {
        FactoryType = typeof(TFactory);
        Function = async (factory) =>
        {
            var vm = await func(factory);
            return new UICViewModel(viewPath, vm);
        };
    }

    public Func<TFactory, Task<IUIComponent>> Function { get; set; }

    public override Task<IUIComponent> GetComponentAsync(IServiceProvider provider)
    {
        var service = provider.GetRequiredService(FactoryType) as TFactory;
        return Function(service);
    }
}
