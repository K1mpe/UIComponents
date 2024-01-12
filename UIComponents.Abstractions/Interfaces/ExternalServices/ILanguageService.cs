namespace UIComponents.Abstractions.Interfaces.ExternalServices;

public interface ILanguageService
{
    Task<string> Translate(ITranslationModel translationModel);
}
