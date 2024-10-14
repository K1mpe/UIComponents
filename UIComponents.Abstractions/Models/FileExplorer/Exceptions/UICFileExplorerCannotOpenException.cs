using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.FileExplorer.Exceptions
{
    public class UICFileExplorerCannotOpenException : ArgumentStringException
    {
        public UICFileExplorerCannotOpenException(string filepath) : base("Cannot move file or directory {0}", filepath)
        {
        }
    }
}
