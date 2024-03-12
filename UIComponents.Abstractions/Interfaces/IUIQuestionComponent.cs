namespace UIComponents.Abstractions.Interfaces;

public interface IUIQuestionComponent: IUIComponent
{
    public string Id { get; set; }
}

public interface IUIQuestionComponent<T> : IUIQuestionComponent
{
    public T MapResponse(string response);
}
