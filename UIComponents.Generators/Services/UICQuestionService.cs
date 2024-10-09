using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Models.HtmlResponse;
using UIComponents.Models.Models.Questions;

namespace UIComponents.Generators.Services;


public class UICQuestionService : IUICQuestionService
{
    private readonly IUICSignalRService _signalRService;
    private readonly IUICStoredComponents _storedComponents;
    private static readonly Dictionary<string, QuestionPersistance> _questionPersistance = new();
    private readonly ILogger _logger;

    public UICQuestionService(ILogger<UICQuestionService> logger, IUICStoredComponents storedComponents, IUICSignalRService signalRService = null)
    {
        _storedComponents = storedComponents;
        _logger = logger;
        _signalRService = signalRService;
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
        if (_signalRService == null)
            throw new Exception($"There is no implementation for {nameof(IUICSignalRService)} registrated.");
        result = string.Empty;
        _logger.BeginScopeKvp(
            new("UICQuestionIdentifier", question.DebugIdentifier),
            new("UICQuestionUserIds", string.Join(", ", userIds))
            );
        try
        {
            var key = _storedComponents.StoreComponentForUsers(question, userIds, userIds.Count == 1);
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
                    DebugIdentifier =  question.DebugIdentifier
                };
            }
            _logger.LogInformation("Asking question {0} to users: {1}", question.DebugIdentifier, string.Join(", ", userIds));
            foreach (var userId in userIds)
            {
                
                if (userId == null)
                    continue;
                _= _signalRService.SendUIComponentToUser(fetchComponent, userId.ToString());
            }
            Console.WriteLine("blub");
            _questionPersistance[key].AutoResetEvent.WaitOne(timeout);
            Console.WriteLine("blub2");
            result = _questionPersistance[key].Response;
            if(result == null)
            {
                _logger.LogError("No response was received for {0}", question.DebugIdentifier);
                return false;
            }
                
            Console.WriteLine($"Response {result}");
            return _questionPersistance[key].Answered;
        }
        catch(NullReferenceException)
        {
            Console.WriteLine("nullReference");
            //No response received within timespan
            _logger.LogError("No response was received for {0}", question.DebugIdentifier);
            return false;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Exception");
            _logger.LogError(ex, "Error for question {0}", question.DebugIdentifier);
            return false;
        }
        finally
        {
            lock (_questionPersistance)
            {
                _questionPersistance.Remove(question.Id);
            }
            RemoveQuestion(question.Id);
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
                _logger.BeginScopeKvp("UICQuestionIdentifier", question.DebugIdentifier);
                _logger.LogInformation("Answered question {0} with '{1}'", question.DebugIdentifier, response);
                question.Response = response;
                question.Answered = true;
                question.AutoResetEvent.Set();
            }
            else
            {
                _logger.LogWarning("Tried to answer question that does not exist.");
            }
        }
    }
    public void CancelQuestion(string key)
    {
        lock (_questionPersistance)
        {
            if (_questionPersistance.TryGetValue(key, out var question))
            {
                _logger.BeginScopeKvp("UICQuestionIdentifier", question.DebugIdentifier);
                _logger.LogInformation("Question {0} was cancelled", question.DebugIdentifier);
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
