namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICLanguageService
{
    Task<string> Translate(Translatable translationModel);

    /// <summary>
    /// This function can translate any kind of object.
    /// You can set the default to translate the type or return .ToString();
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    Task<string> TranslateObject(object obj);
}
