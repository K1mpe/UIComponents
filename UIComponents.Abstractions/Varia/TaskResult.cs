

namespace UIComponents.Abstractions.Varia;

public class TaskResult
{
    public TaskResult(bool isValid = true)
    {
        IsValid = isValid;
    }
    public TaskResult(string message, params object[] args) : this(false)
    {
        AddError(message, args);
    }

    public virtual bool IsValid { get; set; }

    public virtual List<(string Message, object[] args)> Errors { get; set; } = new();

    public object AsyncState => throw new NotImplementedException();

    public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

    public bool CompletedSynchronously => throw new NotImplementedException();

    public bool IsCompleted => throw new NotImplementedException();

    public void AddError(string message, params object[] args)
    {
        IsValid = false;
        Errors.Add((message, args));
    }
}

public class TaskResult<T> : TaskResult
{
    public TaskResult(T result) : base(true)
    {
        Result = result;
    }

    public virtual T Result { get; set; }

    public static implicit operator TaskResult<T>(T result) => new(result);
    public static implicit operator Task<TaskResult<T>>(TaskResult<T> taskResult) => Task.FromResult(taskResult);
}