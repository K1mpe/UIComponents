using FluentValidation;
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
            Translatable message = "IUICValidator";
            validator.RuleFor(expression)
                .Must((model, value) => 
                {
                    var result = validationService.ValidateObjectProperty(property, model).Result;
                    message = result.ValidationErrors.Select(x=>x.ErrorMessage).FirstOrDefault();
                    return !result.HasValidationErrors;
                }).WithMessage((model)=> languageService.Translate(message).Result);
        }
    }



    public static ValidationErrors ToValidationErrors(this FluentValidation.Results.ValidationResult ModelState)
    {
        var response = new ValidationErrors();
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
