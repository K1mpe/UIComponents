using UIComponents.Models.Models.Card;
using UIComponents.Models.Models.Tables;

namespace UIComponents.Models.Models.Inputs;

/// <summary>
/// Creates a input that has a table as value
/// </summary>
public class UICInputTable : UICInput<object[]>
{
    public UICInputTable(string propertyName, UICTable table) : base(propertyName)
    {
        Table = table;
    }

    public UICInputTable() : base(null)
    {

    }
    /// <summary>
    /// The table that is used as the input
    /// </summary>
    public UICTable Table { get; set; }

    /// <summary>
    /// The type of the item in the table
    /// </summary>
    public Type ItemType { get; set; }

    public override bool HasClientSideValidation => false;
}
