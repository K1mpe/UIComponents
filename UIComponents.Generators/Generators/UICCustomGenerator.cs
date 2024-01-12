using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators
{
    public class UICCustomGenerator<TArgs, TResult> : IUICGenerator<TArgs, TResult>
    {
        public Func<TArgs, TResult?, Task<IUICGeneratorResponse<TResult>>> GetResult { get; set; }

        public double Priority { get; set; }

        public string Name { get; set; }
    }
}
