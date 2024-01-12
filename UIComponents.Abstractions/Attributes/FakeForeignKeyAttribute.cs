using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Attributes
{
    /// <summary>
    /// Used for cdc-Grid to replace id with items
    /// </summary>
    public class FakeForeignKeyAttribute : Attribute
    {
        public FakeForeignKeyAttribute(Type type, bool isRequired = true)
        {
            Type = type;
            IsRequired = isRequired;
        }

        public Type Type { get; }
        public bool IsRequired { get; }
    }

}
