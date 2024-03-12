namespace UIComponents.Models.Models.Inputs;

/// <summary>
/// This is a input for a <see cref="bool?"/> can result in true, false, null
/// <br>Can be used as a checkbox or as toggleswitches</br>
/// </summary>
public class UICInputCheckboxThreeState : UICInput<bool?>
{
    #region Fields
    public override bool HasClientSideValidation => false;
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICInputCheckboxThreeState() : this("")
    {

    }
    public UICInputCheckboxThreeState(string propertyName) : base(propertyName)
    {
    }

    #endregion


    #region Properties
    public IColor Color { get; set; } = ColorDefaults.InputCheckbox;

    public CheckboxRenderer Renderer { get; set; }

    #endregion
}
