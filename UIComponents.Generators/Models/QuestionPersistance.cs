namespace UIComponents.Generators.Models;

internal class QuestionPersistance
{
    public string Id { get; set; }
    public AutoResetEvent AutoResetEvent { get; set; }
    public bool Answered { get; set; }
    public string Response { get; set; }
}
