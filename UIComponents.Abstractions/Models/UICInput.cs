namespace UIComponents.Abstractions.Models;


public abstract class UICInput : UIComponent
{
    #region Fields
    public abstract bool HasClientSideValidation { get; }
    #endregion

    public UICInput(string propertyName) : base()
    {
        PropertyName = propertyName;
        Actions = new(() => this.GetId());
    }

    #region Properties
    /// <summary>
    /// The name of the property as used in the C# class
    /// </summary>
    /// <remarks>
    /// This is not the same as the DisplayName
    /// </remarks>
    public string PropertyName { get; set; }

    /// <summary>
    /// A placeholder for when the input is empty
    /// </summary>
    public Translatable Placeholder { get; set; }

    /// <summary>
    /// The popup when hovering over the input
    /// </summary>
    public Translatable Tooltip { get; set; }

    /// <summary>
    /// This is the translated name. This can be used for validation messages etc. (Should be the same as the label text in a input group)
    /// </summary>
    public Translatable DisplayName { get; set; }


    //public UIComponent PrependInput { get; set; }
    //public UIComponent AppendInput { get; set; }

    /// <summary>
    /// The actions that can be triggered on certain events
    /// </summary>
    public UICActions Actions { get; set; }

    public bool Readonly { get; set; }
    public bool Disabled { get; set; }

    public object ValueObject { get; set; }
    #endregion

    #region Methods

    public override string ToString()
    {
        return $"{PropertyName} - {base.ToString()}";
    }

    #endregion



}
public abstract class UICInput<T> : UICInput
{
    public UICInput(string propertyName) : base(propertyName)
    {

    }
    public T Value {
        get { return (T)ValueObject; }
        set { ValueObject = value; }
    }
}
