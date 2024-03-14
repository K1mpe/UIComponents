using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Extensions;

namespace UIComponents.Models.Models;

/// <summary>
/// This is just a list of multiple components
/// </summary>
public class UICGroup : UIComponent, IUIAction, IUICHasChildren<IUIComponent>, IUICHasAttributesAndChildren
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    private bool _render = true;
    #endregion

    #region Ctor
    public UICGroup(List<IUIComponent> components) : this()
    {
        Components = components.ToList();
    }

    public UICGroup(IEnumerable<IUIComponent> components) : this(components.ToList())
    {
    }

    public UICGroup() : base()
    {

    }

    #endregion

    #region Properties

    public UICGroupRenderer Renderer { get; set; } = UICGroupRenderer.Div;

    public List<IUIComponent> Components { get; set; } = new();

    /// <summary>
    /// If no components are rendered, do not render this group (div)
    /// </summary>
    public bool RenderWithoutContent { get; set; } = false;

    /// <summary>
    /// If only 1 item will be rendered, skip the group and only render the item
    /// </summary>
    public bool RenderSingleItem { get; set; } = false;
    public override bool Render
    {
        get
        {
            if (RenderWithoutContent)
                return _render;
            return Components.Where(x =>
            {
                if (x == null)
                    return false;
                if (x is IConditionalRender cr)
                    return cr.Render;
                return _render;
            }).Any();
        }
        set
        {
            _render = value;
        }
    }
    public List<IUIComponent> Children { get => Components; set => Components = value; }

    #endregion


    #region Methods
    #endregion

}
public enum UICGroupRenderer
{
    Div,
    ContentOnly
}
