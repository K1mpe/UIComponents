using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UIComponents.Abstractions.Models.RecurringDates;
using UIComponents.Abstractions.Models.RecurringDates.Selectors;

namespace UIComponents.Web.ModelBinders;

public class RecurringDateModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(RecurringDate) ? new RecurringDateModelBinder() : null;
    }
}

public class RecurringDateModelBinder : IModelBinder
{

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        try
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            var model = new RecurringDate();
            var form = bindingContext.HttpContext.Request.Form;
            if (form == null)
            {
                return Task.CompletedTask;
            }

            model.Included.AddRange(GetFromForm($"{modelName}[{nameof(RecurringDate.Included)}]", form));
            model.Excluded.AddRange(GetFromForm($"{modelName}[{nameof(RecurringDate.Excluded)}]", form));

            bindingContext.Result = ModelBindingResult.Success(model);
            // Model will be null if not found, including for
            // out of range id values (0, -3, etc.)
            //var model = _context.Authors.Find(id);
            //bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace);
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
        
    }

    public List<RecurringDateItem> GetFromForm(string prefix, IFormCollection formCollection)
    {
        List<RecurringDateItem> items = new List<RecurringDateItem>();
        int counter = 0;
        while (true)
        {
            if(!formCollection.TryGetValue($"{prefix}[{counter}][{nameof(RecurringDateItem.Enabled)}]", out var enabled))
            {
                break;
            }
            formCollection.TryGetValue($"{prefix}[{counter}][PatternType]", out var patternTypeString);
            var properties = GetDictionaryFromForm(typeof(RecurringDateItem), $"{prefix}[{counter}]", formCollection);

            var type = RecurringDate.GetType(patternTypeString);
            if (type != null)
            {
                properties["PatternType"] = patternTypeString.ToString();
                var pattern= GetDictionaryFromForm(type, $"{prefix}[{counter}][{nameof(RecurringDateItem.Pattern)}]", formCollection);
                properties[nameof(RecurringDateItem.Pattern)] = RecurringDateItem.SerializeDict(pattern);
            }

            string serialized = RecurringDateItem.SerializeDict(properties);

            items.Add(RecurringDateItem.Deserialize(serialized));

            counter++;
        }
        return items;
    }
    public Dictionary<string, string> GetDictionaryFromForm(Type type, string prefix,  IFormCollection formCollection)
    {
        Dictionary<string, string> properties = new();
        foreach(var property in type.GetProperties())
        {
            if (!property.CanWrite || !property.CanRead)
                continue;
            
            if(formCollection.TryGetValue($"{prefix}[{property.Name}]", out var propValue))
            {
                properties.Add(property.Name, propValue.ToString());
            }
            if (formCollection.TryGetValue($"{prefix}[{property.Name}][]", out var propArray))
            {
                properties.Add(property.Name, $"[{propArray.ToString()}]");
            }
        }
        return properties;
    }
}
