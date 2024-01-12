using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Extensions;

namespace UIComponents.Models.Models;

/// <summary>
/// This is just a list of multiple components
/// </summary>
public class UICGroup : UIComponent, IUIAction
{
    #region Fields
    public override string RenderLocation => "/UIComponents/Group/Div";//this.CreateDefaultIdentifier(Renderer);
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

    public override bool Render
    {
        get => Components.Where(x =>
        {
            if (x == null)
                return false;
            if (x is IConditionalRender cr)
                return cr.Render;
            return true;
        }).Any();
    }

    #endregion


    #region Methods
    /// <summary>
    /// Add a item to the collection and return the <paramref name="item"/>
    /// </summary>
    /// <returns><paramref name="item"/></returns>
    public T Add<T>(T item) where T : class, IUIComponent
    {
        Components.Add(item);
        return item;
    }

    /// <summary>
    /// Add a item to the collection and return the current <see cref="UICGroup"/>
    /// </summary>
    /// <returns>This <see cref="UICGroup"/></returns>
    public UICGroup Add2(IUIComponent item)
    {
        Components.Add(item);
        return this;
    }
    #endregion

}
public enum UICGroupRenderer
{
    Div,
    ContentOnly
}
