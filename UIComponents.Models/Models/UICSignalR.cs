using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models;


/// <summary>
/// This is a signalR listener that will trigger Action when it receives something from signalR
/// </summary>
public class UICSignalR : UIComponent
{
    public override string RenderLocation => DefaultIdentifier(nameof(UICSignalR));

    #region Ctor
    public UICSignalR()
    {

    }

    public UICSignalR(string subscriptionName, params string[] incommingArgs)
    {
        SubscriptionName = subscriptionName;
        SubscriptionArguments = incommingArgs.ToList();
    }

    #endregion

    #region Properties


    /// <summary>
    /// The SignalR update you want to subscribe to
    /// </summary>
    public string SubscriptionName { get; set; }

    /// <summary>
    /// Joining groups for signalR
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// The names of the arguments that come from the SignalR call. Order is important!
    /// </summary>
    public List<string> SubscriptionArguments { get; set; } = new();

    /// <summary>
    /// SignalR will not work if the parent is hidden.
    /// </summary>
    /// <remarks>
    /// Example: Parent is hidden or parentcard is closed.
    /// </remarks>
    public bool DisableOnHidden { get; set; } = true;

    /// <summary>
    /// The action that will be executed when the signalR arrives
    /// </summary>
    /// <remarks>This action has access to the <see cref="SubscriptionArguments"/></remarks>
    public IUICAction Action { get; set; } = new UICCustom();
    #endregion

}
