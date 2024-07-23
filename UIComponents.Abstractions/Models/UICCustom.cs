using UIComponents.Abstractions.Varia;

namespace UIComponents.Abstractions.Models;

/// <summary>
/// A custom component, requires <see cref="Content"/> to render. 
/// </summary>
public class UICCustom : UIComponent, IUICAction, IDropdownItem
{
    #region Fields
    protected bool _render = true;

    protected string _renderLocation;
    #endregion

    #region Ctor
    public UICCustom()
    {

    }

    public UICCustom(string content)
    {
        Content = content;
    }

    public UICCustom(RazerBlock razercode)
    {
        Content = razercode.GetContent();
    }
    #endregion

    #region Properties

    public override bool Render
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Content) && string.IsNullOrEmpty(_renderLocation))
                return false;
            return _render;
        }
        set
        {
            _render = value;
        }
    }

    public override string RenderLocation
    {
        get
        {
            if (string.IsNullOrEmpty(_renderLocation))
                return UIComponent.DefaultIdentifier("Custom");
            return _renderLocation;
        }
    }

    public string Content { get; set; }

    /// <summary>
    /// This property is used to overwrite the ToString method and give some information of what this UICCustom does.
    /// </summary>
    public string Tostring { get; set; }


    #endregion

    #region Methods

    public UICCustom AddLine(string line)
    {
        Content += line;
        Content += "\r\n";
        return this;
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(_renderLocation))
            return _renderLocation;
        if (string.IsNullOrWhiteSpace(Content))
            return "empty UICCustom";
        if (!string.IsNullOrEmpty(Tostring))
            return Tostring;
        return base.ToString();
    }

    /// <summary>
    /// Set the new renderlocation to a custom location
    /// </summary>
    /// <remarks>
    /// example: /Views/Shared/Components/UIComponent/_NoRender
    /// </remarks>
    /// <returns>
    /// this as UICCustom
    /// </returns>
    /// <param name="location"></param>
    public UICCustom ChangeRenderLocation(string location)
    {
        _renderLocation = location;
        return this;
    }

    #endregion

    public static implicit operator UICCustom(RazerBlock razerBlock) => new(razerBlock);
}
