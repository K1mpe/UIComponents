namespace UIComponents.Generators.Models;

internal class QuestionPersistance
{
    public string Id { get; set; }
    public TaskCompletionSource<bool> Done { get; set; }
    public string DebugIdentifier { get; set; }
    public bool Answered { get; set; }
    public string Response { get; set; }
    public bool Canceled { get; set; }
    public object? AnsweredByUserId { get; set; }
}
