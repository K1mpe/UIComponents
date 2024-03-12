namespace UIComponents.Models.Models.Actions;

public class UICActionServerResponse : IUIAction
{
    #region Field
    public string RenderLocation => this.CreateDefaultIdentifier();
    #endregion

    #region Ctor
    public UICActionServerResponse()
    {

    }
    public UICActionServerResponse(bool singleUse, Func<Dictionary<string, string>, Task> func)
    {
        SingleUse = singleUse;
        Function = func;
    }

    public UICActionServerResponse(bool singleUse, Action<Dictionary<string, string>> action) : this(singleUse, (data) => { action(data); return Task.CompletedTask; }) { }
    #endregion

    #region Properties

    public Func<Dictionary<string, string>, Task> Function { get; set; }
    public bool SingleUse { get; set; }

    public object Data { get; set; }

    /// <summary>
    /// Before sending the request, this action is called client side to get additional properties.
    /// </summary>
    /// If this result has the same properties as <see cref="Data"/>, the <see cref="Data"/> takes priority.
    public IUIAction? GetVariableData { get; set; } = null;
    #endregion
}
