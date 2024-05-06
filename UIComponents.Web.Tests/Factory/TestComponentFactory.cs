using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Generators.Configuration;

namespace UIComponents.Web.Tests.Factory;

public class TestComponentFactory
{
    private readonly IUICLanguageService _languageService;
    private readonly UicConfigOptions _uiConfigOptions;
    public TestComponentFactory(IUICLanguageService languageService, UicConfigOptions uiConfigOptions)
    {
        _languageService = languageService;
        _uiConfigOptions = uiConfigOptions;
    }

    public async Task<IUIComponent> CreateComponentAsync(int arg)
    {
        await Task.Delay(0);
        var vm = await CreateViewModelAsync(arg);

        return new UICViewModel("/Views/Home/TestViewModelFromFactory", vm);
    }
    public Task<TestViewModelFromFactory> CreateViewModelAsync(int arg)
    {
        var vm = new TestViewModelFromFactory();
        vm.UsePermissionService = _uiConfigOptions.CheckPermissionServiceType;
        vm.UseLanguageService = _uiConfigOptions.CheckLanguageServiceType;
        return Task.FromResult(vm);
    }
}
public class TestViewModelFromFactory
{
    public bool UsePermissionService { get; set; }
    public bool UseLanguageService { get; set; }
}
