using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Models.HtmlResponse;

namespace UIComponents.Generators.Services;


public class UICQuestionService : IUICQuestionService
{
    private readonly IUICSignalRService _signalRService;
    private readonly IUICStoredComponents _storedComponents;
    private static readonly Dictionary<string, QuestionPersistance> _questionPersistance = new();
    private readonly IUICUserNotificationCollection _userNotificationCollection;
    private readonly ILogger _logger;

    public UICQuestionService(ILogger<UICQuestionService> logger, IUICStoredComponents storedComponents, IUICSignalRService signalRService, IUICUserNotificationCollection userNotificationCollection = null)
    {
        _storedComponents = storedComponents;
        _logger = logger;
        _signalRService = signalRService;
        _userNotificationCollection = userNotificationCollection;
    }


    #region Ask Question

    /// <summary>
    /// Ask a question to a User and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userId">The id of the user that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, object userId, out TResponse response) where TQuestion : IUIQuestionComponent<TResponse>
    {
        return TryAskQuestion(question, timeout, new() { userId }, out response);
    }

    /// <summary>
    /// Ask a question to multiple Users and await the first response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userIds">The ids of the users that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, List<object> userIds, out TResponse response) where TQuestion : IUIQuestionComponent<TResponse>
    {
        response = default;
        var result = TryAskQuestion(question, timeout, userIds, out var stringResponse);
        if (result)
            response = question.MapResponse(stringResponse);
        return result;
    }

    /// <summary>
    /// Ask a question to a User and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userId">The id of the user that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion(IUIQuestionComponent question, TimeSpan timeout, object userId, out string response)
    {
        return TryAskQuestion(question, timeout, new() { userId }, out response);
    }

    /// <summary>
    /// Ask a question to multiple Users and await the first response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userIds">The ids of the users that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public bool TryAskQuestion(IUIQuestionComponent question, TimeSpan timeout, List<object> userIds, out string response)
    {
        return AskQuestion(question, timeout, userIds, out response);
    }
    protected bool AskQuestion(IUIQuestionComponent question, TimeSpan timeout, List<object> userIds, out string result)
    {
        result = string.Empty;
        try
        {
            var key = _storedComponents.StoreComponent(question, userIds.Count == 1);
            question.Id = key;

            var fetchComponent = new FetchComponent()
            {
                ComponentKey = key
            };
            lock (_questionPersistance)
            {
                _questionPersistance[key] = new()
                {
                    Id = key,
                    AutoResetEvent = new(false),
                };
            }

            foreach (var userId in userIds)
            {
                if(userId == null)
                    continue;
                _= _signalRService.SendUIComponentToUser(fetchComponent, userId.ToString());
                if (_userNotificationCollection != null)
                    _userNotificationCollection.AddNotificationToUserCollection(userId.ToString(), question, timeout);
            }
            _questionPersistance[key].AutoResetEvent.WaitOne(timeout);
            result = _questionPersistance[key].Response;
            return _questionPersistance[key].Answered;
        }
        catch
        {
            //No response received within timespan
            return false;
        }
        finally
        {
            lock (_questionPersistance)
            {
                _questionPersistance.Remove(question.Id);
            }

        }

    }

    #endregion

    #region Answer Question

    public void AnswerQuestion(string key, string response)
    {
        lock(_questionPersistance)
        {
            if (_questionPersistance.TryGetValue(key, out var question))
            {
                question.Response = response;
                question.Answered = true;
                question.AutoResetEvent.Set();
            }
        }
    }
    public void CancelQuestion(string key)
    {
        lock (_questionPersistance)
        {
            if (_questionPersistance.TryGetValue(key, out var question))
            {
                question.AutoResetEvent.Set();
            }
        }
    }

    #endregion


    /// <summary>
    /// Remove the question with this id
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    public Task RemoveQuestion(string questionId)
    {
        return _signalRService.RemoveUIComponentWithId(questionId);
    }
    

}
