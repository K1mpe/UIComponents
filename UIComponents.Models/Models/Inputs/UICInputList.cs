namespace UIComponents.Models.Models.Inputs;

public class UICInputList : UICInput<object[]>
{
    #region Fields
    public override bool HasClientSideValidation => false;
    #endregion

    #region Ctor

    public UICInputList(string propertyName) : base(propertyName)
    {
    }

    #endregion

    #region Properties
    public UICInput SingleInstanceInput { get; set; }
    public Type ItemType { get; set; }



    //public bool AllowAdd { get; set; }
    //public UICButton AddButton { get; set; }

    //public bool AllowRemove { get; set; }
    //public UICButton RemoveButton { get; set; }
    #endregion

}
