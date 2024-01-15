
namespace UIComponents.Generators.Models;

public class UICCallCollection
{
    public UICCallCollection(UICGeneratorPropertyCallType callType, IUIComponent caller, UICCallCollection? previousCallCollection)
    {
        CurrentCallType = callType;
        Caller = caller;
        if (previousCallCollection != null)
        {
            var previous = previousCallCollection.PreviousComponents.ToList();
            if (previousCallCollection.Caller != null)
                previous.Add(previousCallCollection.Caller);
            PreviousComponents = previous;
        }
    }

    /// <summary>
    /// This is the current parent that calls this generator
    /// </summary>
    public IUIComponent? Caller { get; }

    /// <summary>
    /// A list of all the parents this caller
    /// </summary>
    public IEnumerable<IUIComponent> PreviousComponents { get; } = new List<IUIComponent>();

    public IEnumerable<IUIComponent> Components
    {
        get
        {
            var results = new List<IUIComponent>(PreviousComponents);
            if(Caller != null)
                results.Add(Caller);
            return results;
        }
        
    }

    /// <summary>
    /// The type of response this call expects, it is recommended to honor this expectation, but not strictly required
    /// </summary>
    public UICGeneratorPropertyCallType CurrentCallType { get; }
}

/// <summary>
/// What type op response is usualy expected from this generator
/// </summary>
public enum UICGeneratorPropertyCallType
{
    ButtonCancel,
    ButtonCreate,
    ButtonSave,
    ButtonEditReadonly,
    ButtonReadonly,
    ButtonDelete,
    ButtonToolbar,

    /// <summary>
    /// When the given Property is a class (except string)
    /// </summary>
    ClassObject,

    PropertyGroup,

    /// <summary>
    /// The span field displayed under a inputfield from a input group
    /// </summary>
    PropertyGroupSpan,
    PropertyLabel,
    PropertyInput,


   /// <summary>
   /// A popover message that is show when hovering over the element
   /// </summary>
    PropertyTooltip,

    /// <summary>
    /// Span message underneath the input
    /// </summary>
    PropertyInfoSpan,

    PropertyRequired,

    /// <summary>
    /// SelectListItems for a selectlist.
    /// </summary>
    SelectListItems,
}
