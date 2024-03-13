namespace UIComponents.Abstractions.Interfaces;

public interface IUIQuestionComponent: IUIComponent
{
    /// <summary>
    /// A Id that will be assigned by the service
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// A string that is used for the logging. Can be the untranslated title
    /// </summary>
    public string DebugIdentifier { get; }
}

public interface IUIQuestionComponent<T> : IUIQuestionComponent
{
    public T MapResponse(string response);
}
