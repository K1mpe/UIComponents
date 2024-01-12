using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;


/// <summary>
/// This action will submit the current form.
/// If used on a button, the button will be transformed to a submit button.
/// <br>If not used on a button, place this action on a form, and use the <see cref="UICActionTriggerSubmit"/> on the trigger instead.</br>
/// </summary>
public class UICActionSubmit : UIComponent, IUIAction
{


    #region Ctor
    public UICActionSubmit() : base()
    {

    }

    public UICActionSubmit(string postLocation) : this()
    {
        PostLocation = postLocation;
    }
    #endregion

    #region Properties
    public string PostLocation { get; set; }

    /// <summary>
    /// This function is called when the post has been successfull
    /// </summary>
    /// <remarks>
    /// args: all event properties
    /// <br>result: result from post</br>
    /// </remarks>
    public IUIAction OnSuccess { get; set; }

    /// <summary>
    /// Optional: If a propertyname is defined, the form will be posted as this property.
    /// <br>This can be used in combination with <see cref="AdditionalPost"/></br>
    /// </summary>
    /// <remarks>
    /// Example:
    /// <br>PostFormAsProperty= "FormData"</br>
    /// <br>AdditionalPost= "new{ Id= 1, Name= "Test" }"</br>
    /// <br>Result = { Id= 1, Name= "Test", FormData = {...} }</br>
    /// </remarks>
    public string PostFormAsProperty { get; set; }

    /// <summary>
    /// All properties from this object will also be posted.
    /// </summary>
    /// <remarks>
    /// Be carefull, if the form has the same properties as this object, this will overwrite the form properties!
    /// </remarks>
    public object AdditionalPost { get; set; }

    #endregion
}
