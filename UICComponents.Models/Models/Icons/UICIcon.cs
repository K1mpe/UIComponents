namespace UIComponents.ComponentModels.Models.Icons;

/// <summary>
/// A icon that uses a fontawesome tag
/// </summary>
/// <remarks>
/// This has some static functions like UICIcon.Delete(), UICIcon.Edit(), etc
/// </remarks>
public class UICIcon : UIComponent
{

    #region Ctor
    public UICIcon() : base()
    {

    }

    /// <summary>
    /// Create a new icon element
    /// </summary>
    /// <param name="icon">Example: fa-solid fa-hastag</param>
    public UICIcon(string icon) : this()
    {
        Icon = icon;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Examle: "fa-solid fa-hashtag"
    /// </summary>
    public string Icon { get; set; }

    public IColor? Color { get; set; }
    #endregion

    #region Presets
    public static UICIcon Add()
    {
        return new UICIcon("fas fa-plus");
    }

    public static UICIcon Checkmark()
    {
        return new UICIcon("fas fa-check");
    }

    public static UICIcon Close()
    {
        return new UICIcon("fa-solid fa-xmark");
    }

    public static UICIcon Delete()
    {
        return new UICIcon("fas fa-trash-can");
    }

    public static UICIcon Edit()
    {
        return new UICIcon("fas fa-pen-to-square");
    }

    public static UICIcon Pin()
    {
        return new UICIcon("fas fa-thumbtack");
    }


    #endregion
}
