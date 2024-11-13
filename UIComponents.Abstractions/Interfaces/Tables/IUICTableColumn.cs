using UIComponents.Abstractions.Enums;

namespace UIComponents.Abstractions.Interfaces.Tables;

public interface IUICTableColumn : IUIComponent, IUICConditionalRender
{
    public string Type { get; }
}

public interface IUICSortableTableColumn : IUICTableColumn
{
    public string ColumnName { get; }
    public SortOrder? SortOrder { get; }
}
