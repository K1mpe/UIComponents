using System.Collections;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property.Inputs;

/// <summary>
/// Convert the PropertyGroup request to a Classobject Request
/// </summary>
public class UICGeneratorInputClassObject : UICGeneratorProperty
{
    public UICGeneratorInputClassObject()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult= false;
        
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyType == null)
            return GeneratorHelper.Next<IUIComponent>();

        if (args.PropertyType == typeof(string) || !args.PropertyType.IsClass || args.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            return GeneratorHelper.Next<IUIComponent>();

        if (args.PropertyValue == null)
            return GeneratorHelper.Success<IUIComponent>(new UICInputCustom() { Render = false}, true);

        

        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, null, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.PropertyValue, null, null, args.Options, cc, args.Configuration);
        
        var result =await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"Object for {args.PropertyType.Name}", newArgs, args.Options);

        var input = new UICInputObject(args.PropertyName)
        {
            Parent = args.CallCollection.Caller,
            Value = args.PropertyValue
        }.Add(result);
        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
