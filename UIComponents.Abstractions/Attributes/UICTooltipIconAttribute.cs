using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Attributes
{
    /// <summary>
    /// This attribute is used as addition to the <see cref="UICTooltipAttribute"/> to overwrite the default icon
    /// </summary>
    public class UICTooltipIconAttribute : Attribute
    {
        public UICTooltipIconAttribute(string iconClass)
        {
            IconClass = iconClass;
        }
        public string IconClass { get; set; }
    }
}
