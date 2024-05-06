using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Configuration;

namespace UIComponents.Web.Components;

[ViewComponent]
public class UICViewComponent : ViewComponent
{
    private readonly UICConfig _uicConfig;
    private readonly IServiceProvider _serviceProvider;
    public UICViewComponent(UICConfig uicConfig, IServiceProvider serviceProvider)
    {
        _uicConfig = uicConfig;
        _serviceProvider = serviceProvider;
    }

    public async Task<IViewComponentResult> InvokeAsync(IUIComponent element)
    {
        await Task.Delay(0);

        if (element is UICFactory factoryUser)
            element = await factoryUser.CreateComponentFromFactoryAsync(_serviceProvider);

        var renderProperty = element.GetType().GetProperties().Where(x => x.Name == nameof(UIComponent.Render)).FirstOrDefault();
        if (element is IUICConditionalRender conditionalRender && !conditionalRender.Render)
        {
            return View($"/UIComponents/ComponentViews/NoRender.cshtml", element);
        }

        if(element is IUICInitializeAsync initialisable)
        {
            await initialisable.InitializeAsync();

            //Check again if the rendercondition has changed
            if (element is IUICConditionalRender conditionalRender2 && !conditionalRender2.Render)
            {
                return View($"/UIComponents/ComponentViews/NoRender.cshtml", element);
            }
        }

        if (element is IUICHasAttributes UIC)
        {
            if (_uicConfig.TryGetLanguageService(out var languageService))
            {
                if (UIC.TryGetPropertyValue<Translatable>("Tooltip", out Translatable tooltip) && tooltip != null)
                    UIC.AddAttribute("title", await languageService.Translate(tooltip));

                if (UIC.TryGetPropertyValue<Translatable>(nameof(UICInput<string>.Placeholder), out Translatable placeholder) && placeholder != null)
                    UIC.AddAttribute("placeholder", await languageService.Translate(placeholder));
            }
        }
        string renderLocation = element.RenderLocation;
        if (element.RenderLocation.Length < 7)
        {
            renderLocation += ".cshtml";
        }
        else 
        { 
            var last7CharOfRenderLocation = element.RenderLocation.Substring(element.RenderLocation.Length - 7);
            if (!last7CharOfRenderLocation.Contains("."))
                renderLocation += ".cshtml";
        }
        ViewData["UIC"] += $" => {element.RenderLocation}";

        if (element is IUICViewModel viewModelComponent)
            return View(renderLocation, viewModelComponent.ViewModel);

        return View(renderLocation, element);
    }
}
