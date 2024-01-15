namespace UIComponents.Abstractions.Interfaces.ExternalServices;

public interface ILanguageService
{
    Task<string> Translate(ITranslateable translationModel);
}
