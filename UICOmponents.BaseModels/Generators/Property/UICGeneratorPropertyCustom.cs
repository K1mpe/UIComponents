using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models.Arguments;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorPropertyCustom : IUICGeneratorProperty
{
    public Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> GetResult { get; set; }

    public double Priority { get; set; }

    public string Name { get; set; }
}
