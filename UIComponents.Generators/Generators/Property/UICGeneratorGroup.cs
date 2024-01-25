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


        var excludedProperties = new List<string>();
        if(!string.IsNullOrEmpty(args.Options.ExcludedProperties))
            excludedProperties = args.Options.ExcludedProperties.Split(",").Select(x=>x.Trim()).ToList();
        if (args.Options.IdHidden)
            excludedProperties.Add("Id");

        var includedProperties = new List<string>();
        if(!string.IsNullOrEmpty(args.Options.IncludedProperties))
            includedProperties = args.Options.IncludedProperties.Split(",").Select(x=> x.Trim()).ToList();

        if(string.IsNullOrWhiteSpace(args.Options.IncludedProperties) || args.Options.IncludedUndefinedProperties)
        {
            foreach(var prop in args.ClassObject.GetType().GetProperties())
            {
                if (excludedProperties.Contains(prop.Name))
                    continue;
                if(includedProperties.Contains(prop.Name)) 
                    continue;
                includedProperties.Add(prop.Name);
            }
        }
        foreach (var propName in includedProperties)
        {
            var property = args.ClassObject.GetType().GetProperty(propName);
            if (property == null)
                continue;
            group.Components.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, property, args.Options, cc));
        }

        if(args.Options.InputGroupSingleRow)
        {
            var singleRow = new UICSingleRow(group.Components.ToList());
            group.Components.Clear();
            group.Components.Add(singleRow);
        }


        return new UICGeneratorResponseSuccess<IUIComponent>(group, true);
    }
}
