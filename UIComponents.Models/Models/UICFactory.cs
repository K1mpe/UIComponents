using Microsoft.Extensions.DependencyInjection;

namespace UIComponents.Models.Models;

public abstract class UICFactory : IUIComponent
{
    public string RenderLocation => throw new NotImplementedException();


    public Type FactoryType { get; protected set; }
    public abstract Task<IUIComponent> CreateComponentFromFactoryAsync(IServiceProvider provider);
}

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

    public override Task<IUIComponent> CreateComponentFromFactoryAsync(IServiceProvider provider)
    {
        using (var scope = provider.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService(FactoryType) as TFactory;
            return Function(service);
        }
    }
}
