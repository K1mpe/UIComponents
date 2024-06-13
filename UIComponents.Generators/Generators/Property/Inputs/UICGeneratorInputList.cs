using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputList : UICGeneratorProperty
{
    public UICGeneratorInputList(ILogger<UICGeneratorInputList> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;

    }

    public override double Priority { get; set; } = 999;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyType == null || args.PropertyType == typeof(string))
            return GeneratorHelper.Next<IUIComponent>();


        if (args.PropertyValue == null)
            return GeneratorHelper.Next<IUIComponent>();

        if(args.UICPropertyType == Abstractions.Attributes.UICPropertyType.SelectList)
            return GeneratorHelper.Next<IUIComponent>();


        if (args.PropertyValue is IEnumerable enumerable)
        {
            List<object> values= new List<object>();
            foreach (var v in enumerable)
                values.Add(v);
            var input = new UICInputList(args.PropertyName)
            {
                Parent = args.CallCollection.Caller,
                Value = values.ToArray()
            };
            input.ItemType = args.PropertyType.GetGenericArguments()[0];

            var type = input.ItemType;
            object value = null;
            try
            {
                if (type == typeof(string))
                    value = string.Empty;
                else
                    value = Activator.CreateInstance(type);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Could not create instance of {0}",type.Name);
            }
            var cc = new UICCallCollection(UICGeneratorPropertyCallType.PropertyInput, input, args.CallCollection);
            var newArgs = new UICPropertyArgs(args.ClassObject, args.PropertyInfo, args.UICPropertyType, args.Options, cc, args.Configuration);
            newArgs.SetPropertyType(type);
            newArgs.SetPropertyValue(value);

            var singleInput = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"{cc.CurrentCallType} {args.ClassObject.GetType().Name} => {args.PropertyName} List", newArgs, args.Options);
            if (singleInput is UICInput asInput)
                input.SingleInstanceInput = asInput;
            else
                throw new Exception($"Result for listItem was {singleInput.GetType().Name} and not assignable to {typeof(UICInput)}");


            if (input.SingleInstanceInput == null)
                throw new NotImplementedException($"There is no input available for {input.ItemType.Name} as a list");

            return GeneratorHelper.Success<IUIComponent>(input, true);
        }

        return GeneratorHelper.Next<IUIComponent>();
    }
}
