using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Models.Models
{
    /// <summary>
    /// This is a basic implementation of the IUICHasColor class that takes any component and adds the IColor Property.
    /// </summary>
    public class UICHasColor : IUIComponent, IUICHasColor
    {
        public string RenderLocation => this.CreateDefaultIdentifier();


        public UICHasColor()
        {
            
        }
        public UICHasColor(IUIComponent component, IColor color)
        {
            Component = component;
            Color = color;
        }

        public IColor? Color { get; set; }

        public IUIComponent Component { get; set; }
    }
}
