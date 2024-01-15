namespace UIComponents.Abstractions.Interfaces.ExternalServices;

public interface UICLanguageService
{
    Task<string> Translate(ITranslateable translationModel);
}
