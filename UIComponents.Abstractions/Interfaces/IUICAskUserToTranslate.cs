using UIComponents.Abstractions.Varia;

namespace UIComponents.Abstractions.Interfaces;

public interface IUICAskUserToTranslate
{
    /// <summary>
    /// This method asks the current user to provide translations. This directs to a filepath of a translations file that is created by <see cref="TranslatableSaver.SaveToFileAsync(List{TranslatableSaver.TranslatableXmlField}, string, bool, bool)"/>
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public Task AskCurrentUserToTranslate(string filepath);
}
