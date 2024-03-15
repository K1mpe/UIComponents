namespace UIComponents.Models.Models.Inputs;


/// <summary>
/// Add custom Html in place of a UICInput. All UICInput properties will be ignored.
/// </summary>
public class UICInputCustom : UICInput
{

    #region Fields
    public override bool HasClientSideValidation => false;


    #endregion

    #region Ctor
    public UICInputCustom() : base(null)
    {
    }
    public UICInputCustom(RazerBlock razercode) : base(null)
    {
        Content = razercode.GetContent();
    }
    #endregion

    #region properties
    public string Content { get; set; }
    #endregion
}
