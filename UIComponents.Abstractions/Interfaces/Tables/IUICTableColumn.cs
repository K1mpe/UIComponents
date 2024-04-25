namespace UIComponents.Abstractions.Interfaces.Tables;

public interface IUICTableColumn : IUIComponent, IUICConditionalRender
{
    public string Type { get; }
}
