using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Web.Models;

public class ImageViewerViewModel
{
    public string ExplorerContainerId { get; set; }
    public string ControllerName { get; set; }
    public RelativePathModel FilePath { get; set; }
}
