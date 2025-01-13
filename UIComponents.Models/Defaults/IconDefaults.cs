using UIComponents.Models.Models.Icons;

namespace UIComponents.Defaults;

public partial class IconDefaults
{
    /// <summary>
    /// This icon is displayed only when the tooltip is not empty
    /// </summary>
    public static Func<UICIcon> TooltipIcon = () => new UICIcon("fas fa-circle-info text-info");

    public static Func<UICIcon> RefreshIcon = () => new UICIcon("fas fa-sync");

    public static Func<UICIcon> Add = ()=> new UICIcon("fas fa-plus");

    /// <summary>
    /// Used for the <see cref="UICButtonSave"/> on creating a new entity
    /// </summary>
    public static Func<UICIcon> Create = null;

    /// <summary>
    /// Used for a <see cref="UICButtonSave"/> except when using the <see cref="Create"/>
    /// </summary>
    public static Func<UICIcon> Edit = null;
    public static Func<UICIcon> CancelEdit = null;
    public static Func<UICIcon> Delete = null;
    public static Func<UICIcon> Save = null;
    public static Func<UICIcon> ButtonCardCollapse = () => new UICIcon("fas fa-minus");
    public static Func<UICIcon> ButtonCardExpend = ()=>new UICIcon("fas fa-plus");
    public static Func<UICIcon> ButtonClose = () => new UICIcon("fas fa-xmark");
    public static Func<UICIcon> Upload = () => new UICIcon("fas fa-upload");
    public static Func<UICIcon> Download = () => new UICIcon("fas fa-download");
    public static Func<UICIcon> Rename = () => new UICIcon("fas fa-input-text");
    public static Func<UICIcon> CreateFolder = () => new UICIcon("fas fa-folder-plus");
    public static Func<UICIcon> Cut = () => new UICIcon("fas fa-cut");
    public static Func<UICIcon> Copy = () => new UICIcon("fas fa-copy");
    public static Func<UICIcon> Paste = () => new UICIcon("fas fa-paste");
    public static Func<UICIcon> OpenFolder = () => new UICIcon("fas fa-folder-open");
    public static Func<UICIcon> OpenFile = () => new UICIcon("fas fa-eye");
    public static Func<UICIcon> DirectoryUp = () => new UICIcon("fas fa-folder-arrow-up");
}
