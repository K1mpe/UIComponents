namespace UIComponents.ComponentModels.Models.Actions;


/// <summary>
/// This action will take a clientside object <see cref="ReferenceObjectName"/> and compare it with <see cref="MatchObject"/> and <seealso cref="NotMatchObject"/>
/// <br>On valid comparison, trigger <see cref="OnMatch"/> action</br>
/// <br>On invalid comparison, trigger <see cref="OnMisMatch"/> action</br>
/// </summary>
public class UICActionValidateObject : UIComponent, IUIAction
{

    #region Ctor
    public UICActionValidateObject()
    {

    }

    public UICActionValidateObject(object matchObject, IUIAction onMatch)
    {
        MatchObject = matchObject;
        OnMatch = onMatch;
    }
    #endregion

    #region Properties
    public string ReferenceObjectName { get; set; } = "referenceObject";

    /// <summary>
    /// Optional: The compared object must match all properties from this object
    /// </summary>
    /// <remarks>
    /// foo* => objectProperty starts with foo
    /// <br>*foo => objectProperty ends with foo</br>
    /// <br>*foo* => objectProperty contains foo</br>
    /// </remarks>
    public object MatchObject { get; set; }

    /// <summary>
    /// Optional: The compared object may not match any of the properties from this object
    /// </summary>
    /// <remarks>
    /// foo* => objectProperty starts with foo
    /// <br>*foo => objectProperty ends with foo</br>
    /// <br>*foo* => objectProperty contains foo</br>
    /// </remarks>
    public object NotMatchObject { get; set; }

    /// <summary>
    /// This action will be executed when the object has a match
    /// </summary>
    public IUIAction OnMatch { get; set; }

    /// <summary>
    /// This action will be executed when the object has no match
    /// </summary>
    public IUIAction OnMisMatch { get; set; }

    #endregion
}
