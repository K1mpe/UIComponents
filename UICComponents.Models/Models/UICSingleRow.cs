using UIComponents.Abstractions.Interfaces;
using UIComponents.ComponentModels.Abstract;

namespace UIComponents.ComponentModels.Models;


/// <summary>
/// This will set the input groups on the same row
/// </summary>
/// <remarks>
/// To make this work for custom <see cref="IUIComponent"/>, implement the <see cref="IUISingleRowSupport"/> interface 
/// </remarks>
public class UICSingleRow : UIComponent
{
    #region Ctor
    public UICSingleRow()
    {

    }

    public UICSingleRow(List<IUIComponent> components)
    {
        Components = components;
    }
    #endregion

    #region Properties

    /// <summary>
    /// These are the affected components
    /// </summary>
    public List<IUIComponent> Components { get; set; } = new();


    /// <summary>
    /// The minimum width of labels
    /// </summary>
    public string MinLabelWidth { get; set; }

    /// <summary>
    /// The minimum width of inputs
    /// </summary>
    public string MinInputWidth { get; set; }

    /// <summary>
    /// The margin between diffrent property rows (SingleRow Only)
    /// </summary>
    public string MarginBetweenRows { get; set; }

    /// <summary>
    /// The margin between diffrent label and input
    /// </summary>
    public string MarginBetweenColumns { get; set; }
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
    /// Add a item to the collection and return the current <see cref="UICSingleRow"/>
    /// </summary>
    /// <returns>This <see cref="UICSingleRow"/></returns>
    public UICSingleRow Add2(IUIComponent item)
    {
        Components.Add(item);
        return this;
    }
    #endregion

    #region Converters
    public UICGroup ConvertToGroup()
    {
        var group = new UICGroup(Components);
        return group;
    }

    /// <summary>
    /// Put a list of items inside a singlerow group
    /// </summary>
    /// <param name="components"></param>
    /// <returns></returns>
    public static List<IUIComponent> ConvertFromList(List<IUIComponent> components)
    {
        var result = new UICSingleRow(components);
        return new() { result };
    }
    #endregion

}
