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
    public static class Addons
    {
        public static UICFileExplorer AddAllAddons(UICFileExplorer fileExplorer)
        {
            fileExplorer.Left.Add(JsTree());
            fileExplorer.Right.Add(Preview());
            fileExplorer.TopContainer.Add(Toolbar(fileExplorer));
            return fileExplorer;
        }
        public static UICGroup JsTree() => new UICGroup() { RenderWithoutContent = true }.AddClass("explorer-tree");
        public static UICGroup Preview() => new UICGroup() { RenderWithoutContent = true }.AddClass("explorer-preview");
        public static UICButtonToolbar Toolbar(UICFileExplorer fileExplorer)
        {
            var toolbar = new UICButtonToolbar() { Distance = ButtonDistance.None };
            toolbar.AddLeft(new UICInputText().AddClass("explorer-path"));
            toolbar.AddLeft(ToggleJsTreeButton(fileExplorer));
            toolbar.AddLeft(TogglePreviewButton(fileExplorer));

            return toolbar;
        }
        public static UICToggleButton ToggleJsTreeButton(UICFileExplorer fileExplorer)
        {
            var click = new UICCustom($"uic.fileExplorer.showhide.jstree($('#{fileExplorer.GetId()}'));");
            var toggle = new UICToggleButton()
            {
                ButtonTrue = new UICButton()
                {
                    PrependButtonIcon = new UICIcon("fas fa-folder-tree"),
                    Tooltip = TranslatableSaver.Save("Button.FileExplorer.HideTree.Tooltip", "Click to hide the tree structure"),
                    OnClick = click
                },
                ButtonFalse = new UICButton()
                {
                    PrependButtonIcon = new UICIcon("far fa-folder-tree"),
                    Tooltip = TranslatableSaver.Save("Button.FileExplorer.ShowTree.Tooltip", "Click to show the tree structure"),
                    OnClick= click
                },
                Value = true
            };
            fileExplorer.AddScript(new UICCustom()
                .AddLine($"$('#{fileExplorer.GetId()}').on('uic-showhide-jstree', (ev, visible)=>{{")
                .AddLine($"     uic.setValue('#{toggle.GetId()}', visible);")
                .AddLine($"}});"));
            return toggle;
        }
        public static UICToggleButton TogglePreviewButton(UICFileExplorer fileExplorer)
        {
            var click = new UICCustom($"uic.fileExplorer.showhide.preview($('#{fileExplorer.GetId()}'));");
            var toggle = new UICToggleButton()
            {
                ButtonTrue = new UICButton()
                {
                    PrependButtonIcon = new UICIcon("fas fa-image"),
                    Tooltip = TranslatableSaver.Save("Button.FileExplorer.HidePreview.Tooltip", "Click to hide the file preview"),
                    OnClick = click
                },
                ButtonFalse = new UICButton()
                {
                    PrependButtonIcon = new UICIcon("far fa-image"),
                    Tooltip = TranslatableSaver.Save("Button.FileExplorer.ShowPreview.Tooltip", "Click to show the file preview"),
                    OnClick = click
                },
                Value = true
            };
            fileExplorer.AddScript(new UICCustom()
                .AddLine($"$('#{fileExplorer.GetId()}').on('uic-showhide-preview', (ev, visible)=>{{")
                .AddLine($"     uic.setValue('#{toggle.GetId()}', visible);")
                .AddLine($"}});"));
            return toggle;
        }
    }
}
