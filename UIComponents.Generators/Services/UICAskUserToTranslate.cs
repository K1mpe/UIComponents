
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Varia;
using UIComponents.Models.Extensions;
using UIComponents.Models.Models.Questions;

namespace UIComponents.Generators.Services;

public class UICAskUserToTranslate : IUICAskUserToTranslate
{
    private readonly IUICQuestionService _questionService;
    private readonly ILogger<UICAskUserToTranslate> _logger;

    public UICAskUserToTranslate(IUICQuestionService questionService, ILogger<UICAskUserToTranslate> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    public async virtual Task AskCurrentUserToTranslate(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);
        var translations = await TranslatableSaver.LoadFromFileAsync(filePath);


        var languages = translations.SelectMany(x => x.TranslationsList.Select(y => y.Code)).Distinct().ToList();
        _logger.LogInformation("There are {0} languages detected in the file", languages.Count);
        if (!languages.Any())
            return;
        var language = languages.First();
        if (languages.Count > 1)
        {
            var question = UICQuestionSelectList.Create(
                TranslatableSaver.Save("UICAskUserToTranslate.SelectLanguage.Title", "Select a language"),
                null,
                languages.Select(x => new SelectListItem(x, x)).ToList(), _questionService);
            var result = await _questionService.TryAskQuestionToCurrentUser(question, TimeSpan.FromMinutes(1));
            if (result.IsValid)
            {
                language = result.Result;
            }
            else
            {
                return;
            }
        }

        await translations.AskCurrentUserToTranslate(_questionService, language);
    }
}
