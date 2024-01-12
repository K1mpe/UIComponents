using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models;
using UIComponents.Generators.Models.Arguments;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;

public abstract class UICGeneratorProperty : UICGeneratorBase<UICPropertyArgs, IUIComponent>
{
    public virtual Type? RequiredPropertyType { get; set; } = null;

    public virtual string? RequiredName { get; set; } = null;

    public virtual UICPropertyType? UICPropertyType { get; set; } = null;

    public virtual UICGeneratorPropertyCallType? RequiredCaller { get; set; } = null;

    public virtual bool? HasExistingResult { get; set; } = null;

    protected override async Task<IUICGeneratorResponse<IUIComponent>> CallResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {

        if (RequiredPropertyType != null)
        {
            if (!args.PropertyInfo?.PropertyType.IsAssignableTo(RequiredPropertyType) ?? false)
                return new UICGeneratorResponseNext<IUIComponent>();
        }

        if (!string.IsNullOrEmpty(RequiredName))
        {
            if (args.PropertyName != RequiredName)
                return new UICGeneratorResponseNext<IUIComponent>();
        }

        if (UICPropertyType != null)
        {
            if (args.UICPropertyType != UICPropertyType)
                return new UICGeneratorResponseNext<IUIComponent>();
        }

        if (RequiredCaller != null)
        {
            if (args.CallCollection.CurrentCallType != RequiredCaller)
                return new UICGeneratorResponseNext<IUIComponent>();

        }

        if(HasExistingResult!= null)
        {
            if (HasExistingResult != (existingResult != null))
                return GeneratorHelper.Next<IUIComponent>();
        }

        return await base.CallResponseAsync(args, existingResult);
    }
}
