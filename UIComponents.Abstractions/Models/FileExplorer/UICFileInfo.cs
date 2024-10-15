using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Abstractions.Models.FileExplorer
{

    public class UICFileInfo : IRelativePath
    {
        public string Thumbnail { get; set; }
        public string Icon { get; set; }

        public string FileName
        {
            get
            {
                if (Extension == "folder")
                {
                    var parts = RelativePath.Split("/");
                    return parts[parts.Length - 2];
                }
                return RelativePath.Split("/").Last();
            }
        }

        public string FileType { get; set; }

        public string Extension
        {
            get
            {
                if (RelativePath.EndsWith("/"))
                    return "folder";
                return RelativePath.Split(".").Last();
            }
        }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

        public string Size
        {
            get
            {
                if (SizeValue == null)
                    return string.Empty;
                return Defaults.FormatDefaults.FormatFileSize(SizeValue.Value);
            }
        }
        public long? SizeValue { get; set; }


        public bool CanOpen { get; set; }
        public bool CanMove { get; set; }
        public bool CanDelete { get; set; }
        public bool CanRename { get; set; }

        public bool IsFolder => Extension == "folder";

        public string AbsolutePathReference { get; set; }

        public string RelativePath { get; set; }

        /// <summary>
        /// If this item is a directory, check if there are subDictories. This is used by to prevent another ajax request from jsTree to get subfolders
        /// </summary>
        public bool DirectoryHasSubdirectories { get; set; } = true;

        /// <summary>
        /// Additional data added here will be added as data- attribute in the html element.
        /// </summary>
        /// <remarks>
        /// AbsolutePath and RelativePath is already used and will be overwritten</remarks>
        public Dictionary<string, string> Data { get; set; } = new();

        /// <summary>
        /// You can pass additional properties here. These are not mapped to data- properties
        /// </summary>
        public Dictionary<string, string> Options { get; set; } = new();
    }
}


