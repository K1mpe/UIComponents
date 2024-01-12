namespace UIComponents.Abstractions.Interfaces;

public interface IUICHasParent
{
    [IgnoreGetChildrenFunction]
    IUIComponent? Parent { get; set; }
}
