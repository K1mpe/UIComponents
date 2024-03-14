namespace UIComponents.Abstractions.Interfaces;

public interface IUICHasAttributes : IUIComponent
{
    Dictionary<string, string> Attributes { get; set; }
}

public interface IUICHasAttributesAndChildren : IUICHasAttributes, IUICHasChildren<IUIComponent>
{

}
