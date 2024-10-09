using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.FileExplorer.Exceptions
{
    public class UICFileExplorerCannotRenameException : ArgumentStringException
    {
        public UICFileExplorerCannotRenameException(string filePath, string newName) : base("Cannot rename file or directory at {0} to {1}", filePath, newName)
        {
        }
    }
}
