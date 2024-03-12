namespace UIComponents.Models.Models.Actions;

public class UICActionGetValue : UIComponent, IUIAction
{
    #region ctor
    public UICActionGetValue()
	{

	}

    public UICActionGetValue(IUIComponent component)
    {
        Component = component;
    }
    public UICActionGetValue(string selector)
    {
        Selector = selector;
    }
    #endregion

    #region Properties
    [IgnoreGetChildrenFunction]
    public IUIComponent Component { get; set; }

    public string Selector { get; set; }

    #endregion
}
