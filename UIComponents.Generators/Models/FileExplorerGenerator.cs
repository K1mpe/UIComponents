using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Generators.Models
{
    public class FileExplorerGenerator
    {
        public double Score { get; set; }
        public string Name { get; set; }
        public Func<UICFileInfo, GetFilesForDirectoryFilterModel, string, Task> Function { get; set; }
    }
}
