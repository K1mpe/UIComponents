using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces
{
    /// <summary>
    /// A interface used for providing the option to translate Exceptions.
    /// </summary>
    public interface IFormattedException
    {
        /// <summary>
        /// This is the unformatted text for the translation
        /// </summary>
        /// <remarks>
        /// {0} requires {1} children
        /// </remarks>
        public string UnformattedMessage { get; set; }

        /// <summary>
        /// The arguments that should be parsed in the message
        /// </summary>
        /// <remarks>
        /// [ "My list", 3]
        /// </remarks>
        public object[] Arguments { get; set; } 

        public string FormattedMessage => string.Format(UnformattedMessage, Arguments);
    }
}
