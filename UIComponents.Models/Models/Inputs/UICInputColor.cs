using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Inputs;

/// <summary>
/// This is a input for a <see cref="string"/> that stores a hex-color or colorname
/// </summary>
public class UICInputColor : UICInput<string>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired || ValidationValidColor;
    #endregion

    #region Ctor
    public UICInputColor() : this("") { }
    public UICInputColor(string propertyName) : base(propertyName)
    {
    }
    #endregion

    #region Properties
    public UICInputColorRenderer Renderer { get; set; } = UICInputColorRenderer.Coloris;

    public bool AllowAlpha { get; set; } = true;

    /// <summary>
    /// Only show the colors from <see cref="Constants.Colors"/>
    /// </summary>
    public bool OnlySystemColors { get; set; }

    public bool ValidationRequired { get; set; }
    public bool ValidationValidColor { get; set; } = true;
    #endregion

}

public enum UICInputColorRenderer
{
    Coloris,
}
