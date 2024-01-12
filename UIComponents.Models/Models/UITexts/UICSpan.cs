using UIComponents.Models.Abstract;

namespace UIComponents.Models.Models.UITexts;

/// <summary>
/// A normal span text
/// </summary>
public class UICSpan : UIComponent
{

    #region Ctor
    public UICSpan() : base()
    {

    }
    public UICSpan(ITranslationModel text) : this()
    {
        Text = text;
    }
    #endregion
    public ITranslationModel Text { get; set; }

}
