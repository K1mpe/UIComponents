using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.FileExplorer.Exceptions
{
    public class UICFileExplorerCannotCreateInDirectoryException : ArgumentStringException
    {
        public UICFileExplorerCannotCreateInDirectoryException(string directoryPath) : base("Cannot create inside this directory {0}", directoryPath)
        {
        }
    }
}
