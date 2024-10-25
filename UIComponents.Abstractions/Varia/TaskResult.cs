using System.ComponentModel.DataAnnotations;

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

    public TaskResult(Exception ex)
    {
        AddError(ex);
    }

    public TaskResult(TaskResult otherTaskResult)
    {
        IsValid = otherTaskResult.IsValid;
        Errors = otherTaskResult.Errors;
    }

    public virtual bool IsValid { get; set; }

    public virtual List<(string Message, object[] Args, Exception? Exception)> Errors { get; set; } = new();

    public virtual Dictionary<string, object> Data { get; } = new();

    public void AddError(string message, params object[] args)
    {
        IsValid = false;
        Errors.Add((message, args, null));
    }
    public void AddError(Exception ex)
    {
        IsValid = false;

        string message = ex.Message;
        object[] args = new object[0];
        if (ex is ArgumentStringException argException)
        {
            message = argException.UnformattedMessage;
            args = argException.Arguments;
        }
        Errors.Add((message, args, ex));
    }
}

public class TaskResult<T> : TaskResult
{
    public TaskResult(T result) : base(true)
    {
        Value = result;
    }
    public TaskResult(string message, params object[] args) : base(message, args)
    {

    }
    public TaskResult(Exception ex) : base(ex)
    {

    }
    public TaskResult(TaskResult otherTaskResult) : base(otherTaskResult)
    {
        if (otherTaskResult.IsValid)
        {
            if (otherTaskResult is TaskResult<T> typeTaskResult)
                Value = typeTaskResult.Value;
            else
                throw new ArgumentException("Cannot parse other taskresult from diffrent type");
        }
    }

    protected T _value;
    public virtual T Value {
        get
        {
            if (!IsValid)
                throw new Exception("The result is invalid, cannot return a valid result");
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public TaskResult<T> AddData(string key, object value)
    {
        Data.Add(key, value);
        return this;
    }

    public static implicit operator T(TaskResult<T> result) => result.Value;
    public static implicit operator TaskResult<T>(T result) => new(result);
    public static implicit operator Task<TaskResult<T>>(TaskResult<T> taskResult) => Task.FromResult(taskResult);
    public static implicit operator Task<T>(TaskResult<T> taskResult) => Task.FromResult(taskResult.Value);
}
