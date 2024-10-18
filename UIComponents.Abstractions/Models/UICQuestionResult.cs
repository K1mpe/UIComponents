namespace UIComponents.Abstractions.Models;

/// <summary>
/// The result from a question
/// </summary>
/// <typeparam name="T"></typeparam>
public class UICQuestionResult<T>
{
    /// <summary>
    /// Did the question receive a valid response?
    /// <br>Only use <see cref="Result"/> after checking if the response is valid</br>
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// The user that responded to the question
    /// </summary>
    public object? AnsweredByUserId { get; init; }

    /// <summary>
    /// The response from the question
    /// </summary>
    public T? Result { get; set; }

    /// <summary>
    /// Did the user cancel the question by clicking the cancelbutton or pressing escape?
    /// </summary>
    public bool IsCanceled { get; init; }

    /// <summary>
    /// Did the timeout expire before interaction from a user?
    /// </summary>
    public bool TimeoutExpired { get; init; }

}
