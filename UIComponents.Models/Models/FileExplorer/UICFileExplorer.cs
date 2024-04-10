using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Models.Models.FileExplorer;

public class UICFileExplorer :UIComponent
{
    #region Ctor
    public UICFileExplorer()
    {
        
    }
    #endregion

    /// <summary>
    /// The Base directory that is used to browse files.
    /// </summary>
    public string RootDirectory { get; set; }

    /// <summary>
    /// The starting directory on page load. If null, <see cref="RootDirectory"/> is used.
    /// </summary>
    /// <remarks>
    /// Example: c:\Test is root, c:\Test\MyFolder is startDirectory, c:\Test\blub is accessable
    /// </remarks>
    public string StartDirectory { get; set; }

    /// <summary>
    /// Name of the used controller. This controller requires the 'IFileExplorerController' interface/>
    /// </summary>
    public string ControllerName { get; set; } = "UICFileExplorer";

    public string AbsolutePathReference { get; set; }
    /// <summary>
    /// A function that takes a absolute filepath and returns a relative file path with a identifier. This is to prevent the clientside from receiving the full file path, but serverside can reconstruct the path with the absolute key
    /// </summary>
    public Func<string, string> CreateRelativePath { get; set; } = (absolutePath) => absolutePath;

    public bool ShowSideBar { get; set; } = true;

    public string RenderMethod { get; set; } = Renderers.Details;

    #region Additional content
    /// <summary>
    /// Displayed above the container
    /// </summary>
    public UICGroup TopContainer { get; set; } = new();

    /// <summary>
    /// Displayed above the main window, between <see cref="Left"/> and <see cref="Right"/>
    /// </summary>
    public UICGroup TopMain { get; set; } = new();

    /// <summary>
    /// Displayed Left of the Main window, behind the sidebar
    /// </summary>
    public UICGroup Left { get; set; } = new();

    /// <summary>
    /// Displayed on the right of the Main window
    /// </summary>
    public UICGroup Right { get; set; } = new();

    /// <summary>
    /// Displayed below the main window, between <see cref="Left"/> and <see cref="Right"/>
    /// </summary>
    public UICGroup BottomMain { get; set; } = new();

    /// <summary>
    /// Displayed below the container
    /// </summary>
    public UICGroup BottomContainer { get; set; } = new();
    #endregion

    public bool CanCreate { get; set; }
    public bool CanMove { get; set; }
    public bool CanCopy { get; set; }
    public bool CanDelete { get; set; }

    public static class Renderers{
        public static string Details => "details";
    }
}
