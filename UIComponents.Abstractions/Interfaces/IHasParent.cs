namespace UIComponents.Abstractions.Interfaces;

public interface IUICHasParent
{
    [UICIgnoreGetChildrenFunction]
    IUIComponent? Parent { get; set; }
}
