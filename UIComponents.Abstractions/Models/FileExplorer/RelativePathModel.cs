using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Abstractions.Models.FileExplorer
{
    public class RelativePathModel : IRelativePath
    {
        public string AbsolutePathReference { get; set; }

        public string RelativePath { get; set; }
    }
}
