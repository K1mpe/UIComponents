using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property
{
    public class UICGeneratorInputGroup : UICGeneratorProperty
    {

        public UICGeneratorInputGroup()
        {
            RequiredCaller = UICGeneratorPropertyCallType.PropertyGroup;
        }

        public override double Priority { get; set; } = 1000;


        public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
        {
            if (args.PropertyInfo == null)
                return new UICGeneratorResponseNext<IUIComponent>();

            if (existingResult != null)
                return new UICGeneratorResponseNext<IUIComponent>();

            var ignoreAttr = args.PropertyInfo.GetCustomAttribute<UICIgnoreAttribute>();
            if (ignoreAttr != null)
            {
                return new UICGeneratorResponseNext<IUIComponent>();
            }

            var inputGroup = new UICInputGroup();
            inputGroup.Label = await args.Configuration.GetGeneratedResultAsync<IUIComponent, UICLabel>(UICGeneratorPropertyCallType.PropertyLabel, inputGroup, args);
            inputGroup.Input = await args.Configuration.GetGeneratedResultAsync<IUIComponent, UICInput>(UICGeneratorPropertyCallType.PropertyInput, inputGroup, args);
            inputGroup.Input.DisplayName = TranslationDefaults.TranslateProperty(args.PropertyInfo, args.UICPropertyType.Value);
            if (!inputGroup.Label.HasValue() && !inputGroup.Input.HasValue())
                inputGroup.Render = false;

            inputGroup.Span = await args.Configuration.GetPropertyGroupSpanAsync(args, inputGroup);

            return new UICGeneratorResponseSuccess<IUIComponent>(inputGroup, true);
        }
    }
}
