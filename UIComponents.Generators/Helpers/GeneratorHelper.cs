using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Varia;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models.UICGeneratorResponses;
using UIComponents.Models.Extensions;
using UIComponents.Abstractions.Helpers;
using UIComponents.Models.Models.Texts;
using System.Diagnostics;

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
        var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);

        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (!args.ClassObject.GetType().IsAssignableTo(typeof(T)))
                    return Next<IUIComponent>();

                if (args.PropertyInfo == null || args.PropertyName != propertyInfo.Name ||args.PropertyType.DeclaringType != propertyInfo.DeclaringType)
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

    /// <inheritdoc cref="PropertyGeneratorForPropType(Type, string, double, Func{UICPropertyArgs, IUIComponent?, Task{IUICGeneratorResponse{IUIComponent}}})"/>>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGeneratorForPropType<T>(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        return PropertyGeneratorForPropType(typeof(T), name, priority, func);
    }

    /// <summary>
    /// A generator that only works for properties for this type
    /// </summary>
    /// <remarks>
    /// <see href="args.PropertyType"/>.IsAssignableTo(<see href="propertyType"/>)
    /// </remarks>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyGeneratorForPropType(Type propertyType, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.PropertyInfo == null || !args.PropertyType!.IsAssignableTo(propertyType))
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
                if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyGroup)
                    return Next<IUIComponent>();
                return await func(args, existing);
            }
        };
        return generator;
    }

    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> PropertyInputGenerator<T>(Expression<Func<T, object>> expression, string name, double priority, Func<UICPropertyArgs, UICInput?, Task<IUICGeneratorResponse<UICInput>>> func)
    {
        var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(expression);
        return new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyInput)
                    return Next<UICInput>();
                if (args.PropertyName != propertyInfo.Name || args.PropertyInfo.DeclaringType != propertyInfo.DeclaringType)
                    return Next<UICInput>();

                return await func(args, existing as UICInput);
            }
        };
    }
    
    /// <summary>
    /// A generator for that only works for a <see cref="UICInput"/> property with this name and 
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static UICCustomGenerator<UICPropertyArgs, UICInput> PropertyInputGenerator(string propertyName, string name, double priority, Func<UICPropertyArgs, UICInput?, Task<IUICGeneratorResponse<UICInput>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, UICInput>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if(args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyInput)
                    return Next<UICInput>();
                if (args.PropertyName != propertyName)
                    return Next<UICInput>();

                return await func(args, existing);
            }
        };
    }

    /// <summary>
    /// A generator that can be used to overwrite a specific button request
    /// </summary>
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

    /// <summary>
    /// <inheritdoc cref="ObjectGenerator(Type, string, double, Func{UICPropertyArgs, IUIComponent?, Task{IUICGeneratorResponse{IUIComponent}}})"/>
    /// </summary>
    public static UICCustomGenerator<UICPropertyArgs, IUIComponent> ObjectGenerator<T>(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> func)
    {
        return ObjectGenerator(typeof(T), name, priority, func);
    }

    /// <summary>
    /// This generator is only used when <see cref="UICPropertyArgs.ClassObject"/> matches the given type. This can be used to overwrite a specific object.
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="func"></param>
    /// <returns></returns>
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


    public static UICCustomGenerator<PropertyInfo, UICPropertyType?> PropertyTypeGenerator(string name, double priority, Func<PropertyInfo, UICPropertyType?, Task<IUICGeneratorResponse<UICPropertyType?>>> func) 
    {
        return new UICCustomGenerator<PropertyInfo, UICPropertyType?>()
        {
            Name = name,
            Priority = priority,
            GetResult = func
        };
    }

    public static UICCustomGenerator<PropertyInfo, Type?> ForeignKeyTypeGenerator(string name, double priority, Func<PropertyInfo, Type?, Task<IUICGeneratorResponse<Type?>>> func)
    {
        return new UICCustomGenerator<PropertyInfo, Type?>()
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


    #region SelectListItems

    #region GlobalConfigure
    /// <summary>
    /// Used for generating selectlist items for foreign keys or enums.
    /// </summary>
    /// <param name="name">Name that is used in debug</param>
    /// <param name="priority">Lowest priority comes first</param>
    /// <param name="func">Generator method</param>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string name, double priority, Func<UICPropertyArgs, List<UICSelectListItem>?, Task<IUICGeneratorResponse<List<UICSelectListItem>>>> func)
    {
        return new UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.SelectListItems)
                    return Next<List<UICSelectListItem>>();
                return await func(args, existing);
            }
        };
    }


    #endregion

    #region Property name specific

    /// <summary>
    /// Get selectlistItems for a specific property. This has priority 0 and should come before default generators.
    /// <br>The default selectlistgenerators should be configured to first check if there are already selectlistitems. and not do anything if the list is already populated.</br>
    /// </summary>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string propertyName, Func<Task<List<UICSelectListItem>>> getSelectListItems, Translatable placeholderText = null)
    {
        return SelectListItems("CustomSelectListItems", 0, async (args, items) =>
        {
            if (args.PropertyName != propertyName)
                return Next<List<UICSelectListItem>>();

            if (args.CallCollection.Caller is UICInput input)
            {
                if (placeholderText != null)
                    input.Placeholder = placeholderText;

                else
                {
                    var translatedProperty = UIComponents.Defaults.TranslationDefaults.TranslateProperty(args.PropertyInfo, args.UICPropertyType);
                    if (input is UICInputMultiSelect multiSelect)
                    {
                        multiSelect.Placeholder = TranslatableSaver.Save("Select.SelectOneOrMore", "Select one or more {0}", translatedProperty);
                    }
                    else
                    {
                        input.Placeholder = TranslatableSaver.Save("Select.Placeholder", "Select a {0}", translatedProperty);
                    }
                }
            }


            var selectListItems = await getSelectListItems();
            return Success(selectListItems, true);
        });
    }

    /// <inheritdoc cref="SelectListItems(string, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string propertyName, Func<Task<List<SelectListItem>>> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyName, async () => (await getSelectListItems()).ToUIC(), placeholderText);
    }

    /// <inheritdoc cref="SelectListItems(string, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string propertyName, Func<List<UICSelectListItem>> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyName, () => Task.FromResult(getSelectListItems()), placeholderText);
    }

    /// <inheritdoc cref="SelectListItems(string, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string propertyName, Func<List<SelectListItem>> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyName, () => Task.FromResult(getSelectListItems()), placeholderText);
    }

    /// <inheritdoc cref="SelectListItems(string, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string propertyName, List<UICSelectListItem> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyName, () => getSelectListItems, placeholderText);
    }

    /// <inheritdoc cref="SelectListItems(string, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems(string propertyName, List<SelectListItem> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyName, () => getSelectListItems, placeholderText);
    }

    #endregion

    #region Property Expression specific

    /// <summary>
    /// Get selectlistItems for a specific property. This has priority 0 and should come before default generators.
    /// <br>The default selectlistgenerators should be configured to first check if there are already selectlistitems. and not do anything if the list is already populated.</br>
    /// </summary>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems<T>(Expression<Func<T, object>> propertyExpression, Func<Task<List<UICSelectListItem>>> getSelectListItems, Translatable placeholderText = null)
    {
        var propertyInfo = InternalHelper.GetPropertyInfoFromExpression(propertyExpression);
        return SelectListItems("CustomSelectListItems", 0, async (args, items) =>
        {
            
            if (args.PropertyName != propertyInfo.Name || args.PropertyInfo!.DeclaringType?.FullName != propertyInfo.DeclaringType?.FullName)
                return Next<List<UICSelectListItem>>();

            if (args.CallCollection.Caller is UICInput input)
            {
                if(placeholderText != null)
                    input.Placeholder = placeholderText;

                else
                {
                    var translatedProperty = UIComponents.Defaults.TranslationDefaults.TranslateProperty(args.PropertyInfo, args.UICPropertyType);
                    if (input is UICInputMultiSelect multiSelect)
                    {
                        multiSelect.Placeholder = TranslatableSaver.Save("SelectList.SelectOneOrMore", "Select one or more {0}", translatedProperty);
                    }
                    else
                    {
                        input.Placeholder = TranslatableSaver.Save("SelectList.SelectOne", "Select a {0}", translatedProperty);
                    }
                }
                
            }

            var selectListItems = await getSelectListItems();
            return Success(selectListItems, true);
        });
    }

    ///<inheritdoc cref=" SelectListItems{T}(Expression{Func{T, object}}, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems<T>(Expression<Func<T, object>> propertyExpression, Func<Task<List<SelectListItem>>> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyExpression, async () => (await getSelectListItems()).ToUIC(), placeholderText);
    }

    ///<inheritdoc cref=" SelectListItems{T}(Expression{Func{T, object}}, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems<T>(Expression<Func<T, object>> propertyExpression, Func<List<UICSelectListItem>> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyExpression, () => Task.FromResult(getSelectListItems()), placeholderText);
    }

    ///<inheritdoc cref=" SelectListItems{T}(Expression{Func{T, object}}, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems<T>(Expression<Func<T, object>> propertyExpression, Func<List<SelectListItem>> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyExpression, () => Task.FromResult(getSelectListItems()), placeholderText);
    }

    ///<inheritdoc cref=" SelectListItems{T}(Expression{Func{T, object}}, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems<T>(Expression<Func<T, object>> propertyExpression, List<UICSelectListItem> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyExpression, () => getSelectListItems, placeholderText);
    }

    ///<inheritdoc cref=" SelectListItems{T}(Expression{Func{T, object}}, Func{Task{List{UICSelectListItem}}}, Translatable)"/>
    public static UICCustomGenerator<UICPropertyArgs, List<UICSelectListItem>> SelectListItems<T>(Expression<Func<T, object>> propertyExpression, List<SelectListItem> getSelectListItems, Translatable placeholderText)
    {
        return SelectListItems(propertyExpression, () => getSelectListItems, placeholderText);
    }
    #endregion

    #endregion

    /// <summary>
    /// A generator response that is used when the current generator could not give a result and the next generator should be called.
    /// </summary>
    public static IUICGeneratorResponse<T> Next<T>()
    {
        return new UICGeneratorResponseNext<T>();
    }
    public static IUICGeneratorResponse<IUIComponent> Next()
    {
        return Next<IUIComponent>();
    }
    public static Task<IUICGeneratorResponse<IUIComponent>> NextAsync()
    {
        return Task.FromResult(Next<IUIComponent>());
    }

    /// <summary>
    /// A generator response that has successfully generated a response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">Result from the success</param>
    /// <param name="allowContinue">Allow other generators to manipulate this result</param>
    public static IUICGeneratorResponse<T> Success<T>(T result, bool allowContinue)
    {
        return new UICGeneratorResponseSuccess<T>(result, allowContinue);
    }
    public static IUICGeneratorResponse<IUIComponent> Success(IUIComponent result, bool allowContinue)
    {
        return Success<IUIComponent>(result, allowContinue);
    }
    public static Task<IUICGeneratorResponse<IUIComponent>> SuccessAsync(IUIComponent result, bool allowContinue)
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
