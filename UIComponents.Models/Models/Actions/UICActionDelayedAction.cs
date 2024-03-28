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
        public UICActionDelayedAction(int miliseconds, IUICAction action)
        {
            Miliseconds = miliseconds;
            Action = action;
        }


        /// <summary>
        /// When triggering this DelayedAction, triggering again will reset the timer. After the miliseconds the <see cref="Action"/> is run once.
        /// </summary>
        public int Miliseconds { get; set; }

        /// <summary>
        /// This action will trigger after the <see cref="Miliseconds"/> delay. Multiple triggers of this will only result in 1 trigger.
        /// </summary>
        public IUICAction Action { get; set; } = new UICCustom();
    }
}
