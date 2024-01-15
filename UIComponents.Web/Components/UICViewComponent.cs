using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Configuration;

namespace UIComponents.Web.Components;

[ViewComponent]
public class UICViewComponent : ViewComponent
{
    private readonly UICConfig _uicConfig;

    public UICViewComponent(IUICLanguageService languageService, UICConfig uicConfig)
    {
        _uicConfig = uicConfig;
    }

    public async Task<IViewComponentResult> InvokeAsync(IUIComponent element)
    {
        await Task.Delay(0);

        var renderProperty = element.GetType().GetProperties().Where(x => x.Name == nameof(UIComponent.Render)).FirstOrDefault();
        if (element.TryGetPropertyValue<bool>(nameof(UIComponent.Render), out bool render))
        {
            if (!render)
                return View($"/UIComponents/NoRender.cshtml", element);
        }

        if (element is UIComponent UIC)
        {
            if (UIC.Hidden)
                UIC.AddAttribute("hidden", "true");
            if(_uicConfig.TryGetLanguageService(out var languageService))
            {
                if (UIC.TryGetPropertyValue<ITranslateable>("Tooltip", out ITranslateable tooltip) && tooltip != null)
                    UIC.AddAttribute("title", await languageService.Translate(tooltip));

                if (UIC.TryGetPropertyValue<ITranslateable>(nameof(UICInput<string>.Placeholder), out ITranslateable placeholder))
                    UIC.AddAttribute("placeholder", await languageService.Translate(placeholder));
            }

            UIC.AddAttribute("class", "uic");


            if (UIC.TryGetPropertyValue<string>(nameof(UICInput.PropertyName), out string propertyName))
                UIC.AddAttribute("name", propertyName);

            if (UIC.TryGetPropertyValue<bool>(nameof(UICInput.Readonly), out bool readOnly))
                if (readOnly)
                {
                    UIC.AddAttribute("readonly", "true");
                    UIC.AddAttribute("disabled", "true");
                }
        }
        

        return View($"{element.RenderLocation}.cshtml", element);
    }
}
