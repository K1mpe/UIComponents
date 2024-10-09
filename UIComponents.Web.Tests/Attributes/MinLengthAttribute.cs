using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Web.Tests.Attributes
{
    public class MinLengthAttribute
    {
        public MinLengthAttribute(int minLength)
        {
            MinLength = minLength;
        }
        public int MinLength { get; set; }
    }
}
