namespace UIComponents.Generators.Models;

internal class StoredEventTrigger
{
    public Func<Dictionary<string, string>, Task> Func { get; set; }
    public bool SingleUse { get; set; }
    public DateTime MaxLifetime { get; set; }

}
