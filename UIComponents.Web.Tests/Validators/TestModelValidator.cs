using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Web.Tests.Models;

namespace UIComponents.Web.Tests.Validators;

public class TestModelValidator : AbstractValidator<TestModel>
{
    private readonly IUICValidationService _validationService;
    private readonly IUICLanguageService _languageService;

    public TestModelValidator(IUICValidationService validationService, IUICLanguageService languageService)
    {
        _validationService = validationService;
        _languageService = languageService;
        


        _validationService.ValidateModel(this, _languageService);
    }
}
