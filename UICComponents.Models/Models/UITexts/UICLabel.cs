using UIComponents.ComponentModels.Defaults;

namespace UIComponents.ComponentModels.Models.UITexts;

/// <summary>
/// A label, used in a <see cref="UICInputGroup"/>
/// </summary>
public class UICLabel : UIComponent
{
    #region Ctor
    public UICLabel() : base()
    {

    }
    public UICLabel(ITranslationModel labelText) : this()
    {
        LabelText = labelText;
    }
    #endregion


    public ITranslationModel LabelText { get; set; }
    public ITranslationModel Tooltip { get; set; }

    /// <summary>
    /// This icon is displayed only when the tooltip is not empty
    /// </summary>
    public UICIcon TooltipIcon { get; set; } = IconDefaults.TooltipIcon;

    /// <summary>
    /// Adding a required marker behind this label
    /// </summary>
    public bool Required { get; set; }
}
