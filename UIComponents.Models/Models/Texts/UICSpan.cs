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
    public UICSpan(Translatable text) : this()
    {
        Text = text;
    }
    #endregion
    public Translatable Text { get; set; }

}
