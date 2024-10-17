namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICQuestionService
{

    #region Ask question

    /// <summary>
    /// Ask a question to a User and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userId">The id of the user that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, object userId, out TResponse response) where TQuestion : IUIQuestionComponent<TResponse>;

    /// <summary>
    /// Ask a question to multiple Users and await the first response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userIds">The ids of the users that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, List<object> userIds, out TResponse response) where TQuestion : IUIQuestionComponent<TResponse>;

    /// <summary>
    /// Ask a question to the current user and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// <br>This method will instantly return false if no user is found</br>
    /// </summary>
    /// <typeparam name="TQuestion"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="question"></param>
    /// <param name="timeout"></param>
    /// <param name="response"></param>
    /// <remarks>This method requires you have a implementation of <see cref="IUICGetCurrentUserId"/></remarks>
    /// <returns></returns>
    public bool TryAskQuestionToCurrentUser<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, out TResponse response) where TQuestion : IUIQuestionComponent<TResponse>;

    /// <summary>
    /// Ask a question to a User and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userId">The id of the user that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion(IUIQuestionComponent question, TimeSpan timeout, object userId, out string response);

    /// <summary>
    /// Ask a question to multiple Users and await the first response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userIds">The ids of the users that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion(IUIQuestionComponent question, TimeSpan timeout, List<object> userIds, out string response);

    /// <summary>
    /// Ask a question to the current User. This method will fail if there is no current user available
    /// <br>This method will instantly return false if no user is found</br>
    /// </summary>
    /// <param name="question"></param>
    /// <param name="timeout"></param>
    /// <param name="response"></param>
    /// <remarks>This method requires you have a implementation of <see cref="IUICGetCurrentUserId"/></remarks>
    /// <returns></returns>
    public bool TryAskQuestionToCurrentUser(IUIQuestionComponent question, TimeSpan timeout,  out string response);

    #endregion

    #region Respond to question
    /// <summary>
    /// Answer the question and set the response. This will also trigger <see cref="RemoveQuestion(string)"/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="response"></param>
    void AnswerQuestion(string key, string response);

    /// <summary>
    /// Cancel a question, result the <see cref="TryAskQuestion(IUIQuestionComponent, TimeSpan, List{object}, out string)"/> methods to fail. This will also trigger <see cref="RemoveQuestion(string)"/>
    /// </summary>
    /// <param name="key"></param>
    void CancelQuestion(string key);

    /// <summary>
    /// Remove the question from users that still might have it open
    /// </summary>
    /// <param name="questionId">The Id of the question</param>
    /// <returns></returns>
    Task RemoveQuestion(string questionId);
    #endregion
}
