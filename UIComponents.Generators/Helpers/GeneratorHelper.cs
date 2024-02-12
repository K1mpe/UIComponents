using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.ExternalServices;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models.UICGeneratorResponses;
using UIComponents.Models.Models.Texts;

namespace UIComponents.Generators.Helpers;

public static class GeneratorHelper
{

    /// <summary>
    /// Add a generator that is used for each property
    /// </summary>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGenerator(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = func
        };
    }

    /// <summary>
    /// Add a generator that only works for the property assigned by the expression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">Expression that directs to property</param>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGenerator<T>(Expression<Func<T, object>> expression, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func) where T: class
    {
        var propertyInfo = InternalGeneratorHelper.GetPropertyInfoFromExpression(expression);

        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (!args.ClassObject.GetType().IsAssignableTo(typeof(T)))
                    return Next<IUIComponent>();

                if (args.PropertyInfo == null || !args.PropertyType!.IsAssignableTo(propertyInfo.PropertyType))
                    return Next<IUIComponent>();

                return await func(args, existing);
            }
        };
        return generator;

    }

    /// <summary>
    /// Add a generator that is called for <see cref="UICInputGroup"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGroupGenerator(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if(args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyGroup)
                    return Next<IUIComponent>();
                return await func(args, existing);
            }
        };
        return generator;
    }

    /// <summary>
    /// Add a generator that only works for a property with this name
    /// </summary>
    /// <param name="propertyName">Name of the matching property</param>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGenerator(string propertyName, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.PropertyName == null || args.PropertyName.ToLower() != propertyName.ToLower())
                    return Next<IUIComponent>();

                return await func(args, existing);
            }
        };
        return generator;
    }

    /// <summary>
    /// Add a generator that only works when matching the given propertyType
    /// </summary>
    /// <param name="propertyType">The type that should match the type in the arguments</param>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGenerator(UICPropertyType propertyType, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.UICPropertyType == null || args.UICPropertyType != propertyType)
                    return Next<IUIComponent>();
                return await func(args, existing);
            }
        };

        return generator;
    }

    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> ButtonGenerator(ButtonType buttonType, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                switch (buttonType)
                {
                    case ButtonType.ButtonCancel:
                        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.ButtonCancel)
                            return Next<IUIComponent>();
                        break;
                    case ButtonType.ButtonCreate:
                        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.ButtonCreate)
                            return Next<IUIComponent>();
                        break;
                    case ButtonType.ButtonSave:
                        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.ButtonSave)
                            return Next<IUIComponent>();
                        break;
                    case ButtonType.ButtonDelete:
                        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.ButtonDelete)
                            return Next<IUIComponent>();
                        break;
                    case ButtonType.ButtonEditReadonly:
                        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.ButtonEditReadonly)
                            return Next<IUIComponent>();
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return await func(args, existing);
            }
        };
    }

    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> ObjectGenerator<T>(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        return ObjectGenerator(typeof(T), name, priority, func);
    }
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> ObjectGenerator(Type objectType, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if(args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.ClassObject)
                    return Next<IUIComponent>();
                if(!args.ClassObject.GetType().IsAssignableTo(objectType))
                    return Next<IUIComponent>();

                return await func(args, existing);
            }
        };
    }

    /// <summary>
    /// Called when checking if a property is required
    /// </summary>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, bool?> RequiredCondition(string name, double priority, Func<UICPropertyArgs, bool?, Task<IUICGeneratorResponse<bool?>>> func)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, bool?>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyRequired)
                    return Next<bool?>();

                return await func(args, existing);
            }
        };
        return generator;
    }

    public static UICCustomGenerator<PropertyInfo, UICPropertyType?> PropertyTypeGenerator(string name, double priority, Func<PropertyInfo, UICPropertyType?, Task<IUICGeneratorResponse<UICPropertyType?>>> func) 
    {
        return new UICCustomGenerator<PropertyInfo, UICPropertyType?>()
        {
            Name = name,
            Priority = priority,
            GetResult = func
        };
    }

    public static UICCustomGenerator<UICPropertyArgs, Translatable> PropertyToolTip(string name, double priority, Func<UICPropertyArgs, Translatable?, Task<IUICGeneratorResponse<Translatable>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, Translatable>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if(args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyTooltip)
                    return Next<Translatable>();

                return await func(args, existing);
            }
        };
    }


    /// <summary>
    /// Used for generating a optional span field under a property
    /// </summary>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertySpanField(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<UICSpan>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyGroupSpan)
                    return Next<IUIComponent>();

                return await func(args, existing);
            }
        };
    }

    /// <summary>
    /// Used for generating selectlist items for foreign keys or enums.
    /// </summary>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, List<SelectListItem>> SelectListItems(string name, double priority, Func<UICPropertyArgs, List<SelectListItem>?, Task<IUICGeneratorResponse<List<SelectListItem>>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, List<SelectListItem>>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.SelectListItems)
                    return Next<List<SelectListItem>>();
                return await func(args, existing);
            }
        };
    }




    /// <summary>
    /// A generator response that is used when the current generator could not give a result and the next generator should be called.
    /// </summary>
    public static UICGeneratorResponseNext<T> Next<T>()
    {
        return new UICGeneratorResponseNext<T>();
    }
    public static UICGeneratorResponseNext<IUIComponent> Next()
    {
        return Next<IUIComponent>();
    }
    public static Task<UICGeneratorResponseNext<IUIComponent>> NextAsync()
    {
        return Task.FromResult(Next<IUIComponent>());
    }

    /// <summary>
    /// A generator response that has successfully generated a response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">Result from the success</param>
    /// <param name="allowContinue">Allow other generators to manipulate this result</param>
    public static UICGeneratorResponseSuccess<T> Success<T>(T result, bool allowContinue)
    {
        return new UICGeneratorResponseSuccess<T>(result, allowContinue);
    }
    public static UICGeneratorResponseSuccess<IUIComponent> Success(IUIComponent result, bool allowContinue)
    {
        return Success<IUIComponent>(result, allowContinue);
    }
    public static Task<UICGeneratorResponseSuccess<IUIComponent>> SuccessAsync(IUIComponent result, bool allowContinue)
    {
        return Task.FromResult(Success(result, allowContinue));
    }

    public enum ButtonType
    {
        ButtonCancel,
        ButtonCreate,
        ButtonSave,
        ButtonDelete,
        ButtonEditReadonly,
    }
}
