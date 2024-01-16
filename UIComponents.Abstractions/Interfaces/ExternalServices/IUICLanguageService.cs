namespace UIComponents.Abstractions.Interfaces.ExternalServices;

public interface IUicLanguageService
{
    Task<string> Translate(ITranslateable translationModel);
}
