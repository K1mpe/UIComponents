using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Abstractions.Models.HtmlResponse;

namespace UIComponents.Web.Extensions;

public static class IUICValidatorExtensions
{
    /// <summary>
    /// Adds all validation rules from <see cref="IUICPropertyValidationRule"/>
    /// </summary>
    /// <typeparam name="T">The model of the abstract validator</typeparam>
    /// <param name="validationService">the injected IUICValidationService</param>
    /// <param name="validator">the current abstractValidator</param>
    /// <param name="languageService">the injected IUICLanguageService</param>
    /// <remarks>
    /// UseCase: _validationService.ValidateModel(this, _languageService);
    /// </remarks>
    public static void ValidateModel<T>(this IUICValidationService validationService, AbstractValidator<T> validator, IUICLanguageService languageService)
    {
        var properties = typeof(T).GetProperties();
        foreach(var property in properties)
        {
            var expression = DynamicExpressionParser.ParseLambda<T, object>(null, false, "@" + property.Name);
            ValidationRuleResultError errorResult = null;
            validator.RuleFor(expression)
                .Must((model, value) => 
                {
                    var result = validationService.ValidateObjectProperty(property, model).Result;
                    errorResult = result.ValidationErrors.FirstOrDefault();
                    return !result.HasValidationErrors;
                }).WithMessage((model)=> TranslationDefaults.TranslateWithPlaceholders(errorResult.ErrorMessage, errorResult.Arguments, languageService).Result);
        }
    }


    public static IActionResult ValidationErrors(this Controller controller, FluentValidation.Results.ValidationResult validationResult=null)
    {
        UICValidationErrors errors = null;
        if (validationResult == null)
            errors = controller.ModelState.ValidationErrors();
        else
            errors = validationResult.ValidationErrors();

        errors.Url = controller.Request.Path.Value;
        return controller.Json(errors);
    }

    public static UICValidationErrors ValidationErrors(this ModelStateDictionary ModelState)
    {
        var response = new UICValidationErrors();
        foreach (var item in ModelState.Where(x => x.Value.Errors.Any()))
        {
            var messages = item.Value.Errors.Select(x => x.ErrorMessage);
            response.Errors.Add(new()
            {
                PropertyName = item.Key,
                Error = string.Join("<br />", messages)
            });
        }
        return response;
    }

    public static UICValidationErrors ValidationErrors(this FluentValidation.Results.ValidationResult ModelState)
    {
        var response = new UICValidationErrors();
        ModelState.Errors.ForEach(x =>
        {
            response.Errors.Add(new()
            {
                PropertyName = x.PropertyName,
                Error = x.ErrorMessage
            });
        });
        return response;
    }
}
