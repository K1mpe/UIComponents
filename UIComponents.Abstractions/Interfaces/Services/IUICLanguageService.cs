namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICLanguageService
{
    Task<string> Translate(Translatable translationModel);
}
