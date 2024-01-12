using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Attributes;

/// <summary>
/// Applying this attribute to a property will ignore this property for the GetAllChildren() extention function
/// </summary>
public class IgnoreGetChildrenFunctionAttribute : Attribute
{
}
