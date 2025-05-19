namespace UIComponents.Abstractions.Models;

/// <summary>
/// This is a custom exception that takes a message and arguments.
/// These can be formatted by <see cref="String.Format(string, object?[])"/>
/// </summary>
public class ArgumentStringException : Exception, IFormattedException
{
    public ArgumentStringException(string message, params object[] arguments) : base(string.Format(message, arguments))
    {
        UnformattedMessage = message;
        Arguments = arguments;
    }

    public string UnformattedMessage { get; set; }
    public object[] Arguments { get; set; }

    public override string Message => string.Format(UnformattedMessage, Arguments);
}
