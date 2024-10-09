using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorGroup : UICGeneratorProperty
{

    public UICGeneratorGroup(ILogger<UICGeneratorGroup> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult= false;
    }
    public override double Priority { get; set; } = 1000;


    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.ClassObject == null)
            return GeneratorHelper.Next<IUIComponent>();

        var group = new UICGroup() { Parent = args.CallCollection.Caller, RenderSingleItem = true };

        if(args.ClassObject is IUIComponent component)
        {
            _logger.LogDebug("{0} is a IUIComponent, not further evaluated by generators", $"{args.ClassObject.GetType().Name}.{args.PropertyName}");
            group.Add(component);
            return GeneratorHelper.Success(group, false);
        }

        var cc = new UICCallCollection(UICGeneratorPropertyCallType.PropertyGroup, group, args.CallCollection);


        var excludedProperties = new List<string>();
        if(!string.IsNullOrEmpty(args.Options.ExcludedProperties))
            excludedProperties = args.Options.ExcludedProperties.ToLower().Split(",").Select(x=>x.Trim()).ToList();
        if (args.Options.HideId)
            excludedProperties.Add("Id");
        


        var includedProperties = new List<string>();
        if(!string.IsNullOrEmpty(args.Options.IncludedProperties))
            includedProperties = args.Options.IncludedProperties.ToLower().Split(",").Select(x=> x.Trim()).ToList();

        if(string.IsNullOrWhiteSpace(args.Options.IncludedProperties) || args.Options.IncludedUndefinedProperties)
        {
            foreach(var prop in args.ClassObject.GetType().GetProperties())
            {
                if (excludedProperties.Contains(prop.Name.ToLower()))
                    continue;
                if(includedProperties.Contains(prop.Name.ToLower())) 
                    continue;
                includedProperties.Add(prop.Name.ToLower());
            }
        }
        foreach (var propName in includedProperties)
        {
            var property = args.ClassObject.GetType().GetProperties().Where(x=>x.Name.ToLower() == propName).FirstOrDefault();
            if (property == null)
                continue;
            group.Components.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, property, args.Options, cc));
        }

        
        if (args.Options.InputGroupSingleRow)
        {
            var singleRow = new UICSingleRow(group.Components.ToList());
            group.Components.Clear();
            group.Components.Add(singleRow);
        }


        return new UICGeneratorResponseSuccess<IUIComponent>(group, true);
    }
}
