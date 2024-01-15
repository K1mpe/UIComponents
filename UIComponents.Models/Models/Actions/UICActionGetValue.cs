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

    #endregion

    #region Properties
    [IgnoreGetChildrenFunction]
    public IUIComponent Component { get; set; }


    #endregion
}
