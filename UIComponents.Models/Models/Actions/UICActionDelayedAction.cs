using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Models.Models.Actions
{
    public class UICActionDelayedAction : IUICAction
    {
        public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICActionDelayedAction));

        public UICActionDelayedAction()
        {
            
        }
        public UICActionDelayedAction(int miliseconds, DelayedActionType delayType, IUICAction action)
        {
            Miliseconds = miliseconds;
            Action = action;
            DelayType = delayType;
        }


        /// <summary>
        /// When triggering this DelayedAction, triggering again will reset the timer. After the miliseconds the <see cref="Action"/> is run once.
        /// </summary>
        public int Miliseconds { get; set; }

        /// <summary>
        /// The type of the delay is configured here
        /// </summary>
        public DelayedActionType DelayType { get; set; }

        /// <summary>
        /// This action will trigger after the <see cref="Miliseconds"/> delay. Multiple triggers of this will only result in 1 trigger.
        /// </summary>
        public IUICAction Action { get; set; } = new UICCustom();

        public enum DelayedActionType
        {
            /// <summary>
            /// Waits for a period of inactivity, then execute the action
            /// </summary>
            Debounce,

            /// <summary>
            /// Waits for the delay, then execute once. All other triggers while waiting are ignored
            /// </summary>
            Delay,

            /// <summary>
            /// Trigger instantly, then block all triggers in the delay. After the delay is completed and one or more triggers are blocked, execute the action again one time
            /// </summary>
            Throttle
        }

    }
}
