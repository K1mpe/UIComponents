using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Attributes
{
    public class CDCValidatorAttribute : Attribute
    {
        //This attribute only works with a EntityValidator<T>
        public CDCValidatorAttribute()
        {
        }

        public bool Unique { get; set; } = false;
        public bool SetIsUnique { get; set; } = false;
        public bool Required { get; set; } = false;
        public int MinLength { get; set; } = 0;
        public int MaxLength { get; set; } = int.MaxValue;

        public int MinValue { get; set; } = int.MinValue;
        public int MaxValue { get; set; } = int.MaxValue;

        public bool Readonly { get; set; } = false;


    }
}
