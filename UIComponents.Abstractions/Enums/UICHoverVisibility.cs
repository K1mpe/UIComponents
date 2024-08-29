using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Enums
{
    public enum UICHoverVisibility
    {
        /// <summary>
        /// Always visible
        /// </summary>
        Visible =1,

        /// <summary>
        /// Only visible when the mouse is hovering
        /// </summary>
        VisibleOnHover=2,

        /// <summary>
        /// Always hidden
        /// </summary>
        Hidden = 3
    }
}
