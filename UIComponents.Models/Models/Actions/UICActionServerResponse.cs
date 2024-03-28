namespace UIComponents.Models.Models.Actions;

public class UICActionServerResponse : IUICAction
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

    /// <summary>
    /// This is the maximum time this connection can exist. Cleaning up the connection after this time.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(1);

    public object Data { get; set; }

    /// <summary>
    /// Before sending the request, this action is called client side to get additional properties.
    /// </summary>
    /// If this result has the same properties as <see cref="Data"/>, the <see cref="Data"/> takes priority.
    public IUICAction? GetVariableData { get; set; } = null;
    #endregion
}
