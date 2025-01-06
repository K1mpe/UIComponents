using UIComponents.Models.Models.Icons;

namespace UIComponents.Defaults;

public partial class IconDefaults
{
    /// <summary>
    /// This icon is displayed only when the tooltip is not empty
    /// </summary>
    public static UICIcon TooltipIcon = new UICIcon("fas fa-circle-info text-info");

    public static UICIcon RefreshIcon = new UICIcon("fas fa-sync");

    public static UICIcon Add = new UICIcon("fas fa-plus");

    /// <summary>
    /// Used for the <see cref="UICButtonSave"/> on creating a new entity
    /// </summary>
    public static UICIcon Create = null;

    /// <summary>
    /// Used for a <see cref="UICButtonSave"/> except when using the <see cref="Create"/>
    /// </summary>
    public static UICIcon Save = null;
    public static UICIcon Edit = null;
    public static UICIcon CancelEdit = null;
    public static UICIcon Delete = null;

    public static UICIcon ButtonCardCollapse = new UICIcon("fas fa-minus");
    public static UICIcon ButtonCardExpend = new UICIcon("fas fa-plus");
    public static UICIcon ButtonClose = new UICIcon("fas fa-xmark");

    public static UICIcon Upload = new UICIcon("fas fa-upload");
    public static UICIcon Download = new UICIcon("fas fa-download");
    public static UICIcon Rename = new UICIcon("fas fa-input-text");
    public static UICIcon CreateFolder = new UICIcon("fas fa-folder-plus");
    public static UICIcon Cut = new UICIcon("fas fa-cut");
    public static UICIcon Copy = new UICIcon("fas fa-copy");
    public static UICIcon Paste = new UICIcon("fas fa-paste");
    public static UICIcon OpenFolder = new UICIcon("fas fa-folder-open");
    public static UICIcon OpenFile = new UICIcon("fas fa-eye");
    public static UICIcon DirectoryUp = new UICIcon("fas fa-folder-arrow-up");
}
