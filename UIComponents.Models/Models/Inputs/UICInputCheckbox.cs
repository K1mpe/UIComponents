using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Inputs;

/// <summary>
/// This is a input for a <see cref="bool"/>
/// <br>Can be used as a checkbox or as toggleswitches</br>
/// </summary>
public class UICInputCheckbox : UICInput<bool>
{
    #region Fields
    public override bool HasClientSideValidation => false;
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICInputCheckbox() : this("")
    {

    }
    public UICInputCheckbox(string propertyName) : base(propertyName)
    {
    }

    #endregion


    #region Properties
    public IColor Color { get; set; } = ColorDefaults.InputCheckbox;

    public CheckboxRenderer Renderer { get; set; } = CheckboxRenderer.Checkbox;

    #endregion
}
public enum CheckboxRenderer
{
    Checkbox,
    ToggleSwitch
}
