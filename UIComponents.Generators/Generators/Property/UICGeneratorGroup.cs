using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorGroup : UICGeneratorProperty
{

    public UICGeneratorGroup()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult= false;
    }
    public override double Priority { get; set; } = 1000;


    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.ClassObject == null)
            return GeneratorHelper.Next<IUIComponent>();

        var group = new UICGroup();
        var cc = new UICCallCollection(UICGeneratorPropertyCallType.PropertyGroup, group, args.CallCollection);
        foreach (var prop in args.ClassObject.GetType().GetProperties())
        {
            group.Components.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, prop, args.Options, cc));
        }



        return new UICGeneratorResponseSuccess<IUIComponent>(group, true);
    }
}
