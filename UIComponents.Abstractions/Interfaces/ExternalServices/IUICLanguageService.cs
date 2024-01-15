namespace UIComponents.Abstractions.Interfaces.ExternalServices;

public interface IUICLanguageService
{
    Task<string> Translate(ITranslateable translationModel);
}
