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


    public static UICIcon ButtonCardCollapse = new UICIcon("fas fa-minus");
    public static UICIcon ButtonCardExpend = new UICIcon("fas fa-plus");
}
