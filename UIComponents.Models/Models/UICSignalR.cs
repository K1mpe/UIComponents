using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models
{

    /// <summary>
    /// This is a signalR listener that will trigger Action when it receives something from signalR
    /// </summary>
    public class UICSignalR : UIComponent, IUICSupportsTaghelperContent
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

        /// <summary>
        /// A function that must return true or false to check if the action can be executed.
        /// </summary>
        /// <remarks>This function has access to the <see cref="SubscriptionArguments"/></remarks>
        public IUICAction Condition { get; set; } = new UICCustom();

        public bool Debug { get; set; } = UIComponents.Defaults.Models.UICSignalR.Debug;
        #endregion

        bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;
        /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
        protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
        {
            var child = new UICCustom(taghelperContent);
            Action = child;
            return Task.CompletedTask;
        }
        Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
    }
}
namespace UIComponents.Defaults.Models
{
    public static class UICSignalR
    {
        public static bool Debug { get; set; }
    }
}
