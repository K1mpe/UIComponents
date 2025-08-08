using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Models.Models.FileExplorer
{

    public class UICFileExplorer : UIComponent
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

        public static class Renderers
        {
            public static string Details => "details";
        }

    }
    public static class UICFileExplorerAddons
    {
        public static UICFileExplorer AddAllAddons(this UICFileExplorer fileExplorer)
        {
            fileExplorer.Left.Add(JsTree());
            fileExplorer.Right.Add(Preview());
            fileExplorer.TopContainer.Add(Toolbar(fileExplorer));
            ContextMenu(fileExplorer).ForEach(x => fileExplorer.BottomContainer.Add(x));
            return fileExplorer;
        }
        public static UICGroup JsTree() => new UICGroup() { RenderWithoutContent = true }.AddClass("explorer-tree");
        public static UICGroup Preview() => new UICGroup() { RenderWithoutContent = true }.AddClass("explorer-preview");
        public static UICButtonToolbar Toolbar(UICFileExplorer fileExplorer)
        {
            var toolbar = new UICButtonToolbar() { Distance = ButtonDistance.None };
            toolbar.AddLeft(GoDirectoryUp(fileExplorer));
            toolbar.AddLeft(new UICInputText().AddClass("explorer-path"));
            toolbar.AddLeft(ToggleJsTreeButton(fileExplorer));
            toolbar.AddLeft(TogglePreviewButton(fileExplorer));

            return toolbar;
        }
        public static UICButton GoDirectoryUp(UICFileExplorer fileExplorer)
        {
            return new UICButton()
            {
                AppendButtonIcon = IconDefaults.DirectoryUp?.Invoke(),
                Tooltip = TranslatableSaver.Save("Button.FileExplorer.DirectoryUp.Tooltip", "Click to move the directory up"),
                OnClick = new UICCustom($"uic.fileExplorer.directoryGoUp($('#{fileExplorer.GetId()}'));")
            };
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
                    OnClick = click
                },
                Value = true
            };
            fileExplorer.AddScriptDocReady(new UICCustom()
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
            fileExplorer.AddScriptDocReady(new UICCustom()
                .AddLine($"$('#{fileExplorer.GetId()}').on('uic-showhide-preview', (ev, visible)=>{{")
                .AddLine($"     uic.setValue('#{toggle.GetId()}', visible);")
                .AddLine($"}});"));
            return toggle;
        }
        public static List<IUIComponent> ContextMenu(UICFileExplorer fileExplorer)
        {
            var contextMenuItems = new List<IUIComponent>();

            var folderGroup = new UICContextMenuCategory()
            {
                CategoryId = "FileExplorer.FolderActions",
                CategoryRenderer = UICContextMenuCategory.CategoryRendererGroup(true)
            };
            contextMenuItems.Add(folderGroup);


            var createDirectory = new UICContextMenuItem($"#{fileExplorer.GetId()}:has(.can-create)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.CreateDirectory")) { Icon = IconDefaults.CreateFolder?.Invoke() }, new UICCustom("uic.fileExplorer.createDirectory(target);"))
            {
                Id = "FileExplorer.Actions.CreateDirectory",
                Category = folderGroup.CategoryId
            };
            contextMenuItems.Add(createDirectory);

            var upload = new UICContextMenuItem($"#{fileExplorer.GetId()}:has(.can-create)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Upload")) { Icon = IconDefaults.Upload?.Invoke() }, new UICCustom("uic.fileExplorer.uploadPage(target);"))
            {

                Id = "FileExplorer.Actions.Upload",
                Category = folderGroup.CategoryId
            };
            contextMenuItems.Add(upload);

            var fileGroup = new UICContextMenuCategory()
            {
                CategoryId = "FileExplorer.FileActions",
                CategoryRenderer = UICContextMenuCategory.CategoryRendererGroup(true)
            };
            contextMenuItems.Add(fileGroup);

            var openFile = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item:not(.explorer-folder):not(.cannot-open)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Open")) { Icon = IconDefaults.OpenFile?.Invoke() }, new UICCustom("uic.fileExplorer.openItem(target);"))
            {
                Id = "FileExplorer.Actions.OpenFile",
                Category = fileGroup.CategoryId
            };
            contextMenuItems.Add(openFile);

            var openFolder = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item.explorer-folder:not(.cannot-open)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Open")) { Icon = IconDefaults.OpenFolder?.Invoke() }, new UICCustom("uic.fileExplorer.openItem(target);"))
            {
                Id = "FileExplorer.Actions.OpenFolder",
                Category = fileGroup.CategoryId
            };
            contextMenuItems.Add(openFolder);
            var download = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item:not(.cannot-open)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Download")) { Icon = IconDefaults.Download?.Invoke() }, new UICCustom("uic.fileExplorer.downloadSelected(target);"))
            {
                Id = "FileExplorer.Actions.Download",
                Category = fileGroup.CategoryId
            };
            contextMenuItems.Add(download);
            var cut = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item.can-move", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Cut")) { Icon = IconDefaults.Cut?.Invoke() }, new UICCustom("uic.fileExplorer.cutSelected(target);"))
            {
                Id = "FileExplorer.Actions.Cut",
                Category = fileGroup.CategoryId
            };
            contextMenuItems.Add(cut);
            var copy = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item:not(.cannot-open)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Copy")) { Icon = IconDefaults.Copy?.Invoke() }, new UICCustom("uic.fileExplorer.copySelected(target);"))
            {
                Id = "FileExplorer.Actions.Copy",
                Category = fileGroup.CategoryId
            };
            contextMenuItems.Add(copy);

            var paste = new UICContextMenuItem($"#{fileExplorer.GetId()}:has(.can-create)", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Paste")) { Icon = IconDefaults.Paste?.Invoke() }, new UICCustom("uic.fileExplorer.pasteSelected(clickedElement);"))
            {
                Id = "FileExplorer.Actions.Paste",
                Category = fileGroup.CategoryId,
                Attributes = new UICCustom()
                    .AddLine("function(){")
                    .AddLine("let attr= {};")
                    .AddLine("if(!uic.fileExplorer._copiedFiles.length)")
                    .AddLine("  attr.disabled = true;")
                    .AddLine("return attr;")
                    .AddLine("}")
            };
            contextMenuItems.Add(paste);

            var delete = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item.can-delete", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Delete")) { Icon = IconDefaults.Delete?.Invoke() }, new UICCustom("uic.fileExplorer.deleteSelected(target);"))
            {
                Id = "FileExplorer.Actions.Delete",
                Category = fileGroup.CategoryId,
            };
            contextMenuItems.Add(delete);
            var rename = new UICContextMenuItem($"#{fileExplorer.GetId()} .explorer-item.can-rename", new UICDropdownItem(TranslatableSaver.Save("FileExplorer.Rename")) { Icon = IconDefaults.Rename?.Invoke() }, new UICCustom("uic.fileExplorer.rename(target);"))
            {
                Id = "FileExplorer.Actions.Rename",
                Category = fileGroup.CategoryId
            };
            contextMenuItems.Add(rename);
            return contextMenuItems;
        }
    }
}

