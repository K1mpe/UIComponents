using Microsoft.Extensions.Logging;
namespace UIComponents.Abstractions;

public static class UICEventIds
{
    public static EventId GeneratorSuccessfullResult = new(1001, nameof(GeneratorSuccessfullResult));
    public static EventId GeneratorBreakGenerators = new(1002, nameof(GeneratorBreakGenerators));
    public static EventId GeneratorNoResultForGenerator = new(1003, nameof(GeneratorNoResultForGenerator));
    public static EventId GeneratorNoResultForRequest = new(1004, nameof(GeneratorNoResultForRequest));
}
