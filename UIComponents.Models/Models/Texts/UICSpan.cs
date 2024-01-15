using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Texts;

/// <summary>
/// A normal span text
/// </summary>
public class UICSpan : UIComponent
{

    #region Ctor
    public UICSpan() : base()
    {

    }
    public UICSpan(ITranslateable text) : this()
    {
        Text = text;
    }
    #endregion
    public ITranslateable Text { get; set; }

}
