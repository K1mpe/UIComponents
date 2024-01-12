using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Attributes
{
    public class CrudTranslationAttribute : Attribute
    {
        /// <summary>
        /// Properties with name "Name" are automatically translated in js-grids. Setting this attribute false will disable this feature.
        /// </summary>
        /// <param name="translate"></param>
        public CrudTranslationAttribute(bool translate)
        {
            Translate = translate;
        }
        public bool Translate { get; set; }
    }
}
