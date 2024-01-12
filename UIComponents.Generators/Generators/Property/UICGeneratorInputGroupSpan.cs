using System.Reflection;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property;

/// <summary>
/// Create a span field based on the <see cref="UICSpanAttribute"/>
/// </summary>
public class UICGeneratorInputGroupSpan : UICGeneratorProperty
{
    public UICGeneratorInputGroupSpan()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyGroupSpan;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyInfo == null)
            return await Task.FromResult(GeneratorHelper.Next<IUIComponent>());

        var spanAttr = args.PropertyInfo.GetCustomAttribute<UICSpanAttribute>();
        if (spanAttr == null)
            return await Task.FromResult(GeneratorHelper.Next<IUIComponent>());

        var span = new UICSpan(spanAttr.TranslationModel);
        return await Task.FromResult(GeneratorHelper.Success<IUIComponent>(span, true));
    }
}
