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
    private readonly IUICGetCurrentUserId _uICGetCurrentUserId;
    private static readonly Dictionary<string, QuestionPersistance> _questionPersistance = new();
    private readonly ILogger _logger;

    public UICQuestionService(ILogger<UICQuestionService> logger, IUICStoredComponents storedComponents, IUICSignalRService signalRService = null, IUICGetCurrentUserId uICGetCurrentUserId = null)
    {
        _storedComponents = storedComponents;
        _logger = logger;
        _signalRService = signalRService;
        _uICGetCurrentUserId = uICGetCurrentUserId;
    }


    #region Ask Question


    public async Task<UICQuestionResult<TResponse>> TryAskQuestionToCurrentUser<TQuestion, TResponse>(TQuestion question, TimeSpan timeout) where TQuestion : IUIQuestionComponent<TResponse>
    {
        if (_uICGetCurrentUserId == null)
            throw new Exception($"There is no implementation for {nameof(IUICGetCurrentUserId)} registrated.");
        var userId = await _uICGetCurrentUserId.GetCurrentUserId();
        if (userId == null)
        {
            return new()
            {
                IsValid = false,
                TimeoutExpired = true,
            };
        }
        return await TryAskQuestion<TQuestion, TResponse>(question, timeout, userId);
    }

    public async Task<UICQuestionResult<string>> TryAskQuestionToCurrentUser(IUIQuestionComponent question, TimeSpan timeout)
    {
        if (_uICGetCurrentUserId == null)
            throw new Exception($"There is no implementation for {nameof(IUICGetCurrentUserId)} registrated.");
        var userId = await _uICGetCurrentUserId.GetCurrentUserId();
        if (userId == null)
        {
            return new()
            {
                IsValid = false,
                TimeoutExpired = true,
            };
        }
        return await TryAskQuestion(question, timeout, userId);
    }

    /// <summary>
    /// Ask a question to a User and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userId">The id of the user that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public virtual Task<UICQuestionResult<TResponse>> TryAskQuestion<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, object userId) where TQuestion : IUIQuestionComponent<TResponse>
    {
        return TryAskQuestion<TQuestion, TResponse>(question, timeout, new() { userId });
    }

    /// <summary>
    /// Ask a question to multiple Users and await the first response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userIds">The ids of the users that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public virtual async Task<UICQuestionResult<TResponse>> TryAskQuestion<TQuestion, TResponse>(TQuestion question, TimeSpan timeout, List<object> userIds) where TQuestion : IUIQuestionComponent<TResponse>
    {
        var result = await TryAskQuestion(question, timeout, userIds);
        if (!result.IsValid)
        {
            return new()
            {
                IsValid = false,
                AnsweredByUserId = result.AnsweredByUserId,
                IsCanceled = result.IsCanceled,
                TimeoutExpired = result.TimeoutExpired
            };
        }

        var converted = question.MapResponse(result.Result);
        return new()
        {
            IsValid = true,
            AnsweredByUserId = result.AnsweredByUserId,
            IsCanceled = result.IsCanceled,
            TimeoutExpired = result.TimeoutExpired,
            Result = converted
        };
    }

    /// <summary>
    /// Ask a question to a User and await the response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userId">The id of the user that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public virtual Task<UICQuestionResult<string>> TryAskQuestion(IUIQuestionComponent question, TimeSpan timeout, object userId)
    {
        return TryAskQuestion(question, timeout, new List<object>() { userId });
    }

    /// <summary>
    /// Ask a question to multiple Users and await the first response. Returns false if the timeout expires or the user presses the cancel button
    /// </summary>
    /// <param name="question">The question to ask the user</param>
    /// <param name="timeout">The timeout before the question expires (while waiting on the response, the entire thread is blocked)</param>
    /// <param name="userIds">The ids of the users that should answer the message</param>
    /// <param name="response">The response from the user (if successfull)</param>
    /// <returns></returns>
    public virtual Task<UICQuestionResult<string>> TryAskQuestion(IUIQuestionComponent question, TimeSpan timeout, List<object> userIds)
    {
        return AskQuestion(question, timeout, userIds);
    }
    protected virtual async Task<UICQuestionResult<string>> AskQuestion(IUIQuestionComponent question, TimeSpan timeout, List<object> userIds)
    {
        if (_signalRService == null)
            throw new Exception($"There is no implementation for {nameof(IUICSignalRService)} registrated.");
        
        _logger.BeginScopeKvp(
            new("UICQuestionIdentifier", question.DebugIdentifier),
            new("UICQuestionUserIds", string.Join(", ", userIds))
            );
        try
        {
            var key = _storedComponents.StoreComponentForUsers(question, userIds, userIds.Count == 1);
            question.Id = key;

            var fetchComponent = new UICFetchComponent()
            {
                ComponentKey = key
            };
            lock (_questionPersistance)
            {
                _questionPersistance[key] = new()
                {
                    Id = key,
                    Done = new TaskCompletionSource<bool>(),
                    DebugIdentifier = question.DebugIdentifier
                };
            }
            _logger.LogInformation("Asking question {0} to users: {1}", question.DebugIdentifier, string.Join(", ", userIds));
            foreach (var userId in userIds)
            {
                
                if (userId == null)
                    continue;
                await _signalRService.SendUIComponentToUser(fetchComponent, userId.ToString());
            }
            await Task.WhenAny(_questionPersistance[key].Done.Task, Task.Delay(timeout));
            var response = _questionPersistance[key];
            if (!response.Answered)
            {
                _logger.LogError("Timeout expired for question {0}", question.DebugIdentifier);
                return new()
                {
                    IsValid = false,
                    TimeoutExpired = true
                };
            }
            if (response.Canceled)
            {
                return new()
                {
                    IsValid = false,
                    IsCanceled = true,
                    AnsweredByUserId = response.AnsweredByUserId,
                };
            }
            return new()
            {
                IsValid = true,
                AnsweredByUserId = response.AnsweredByUserId,
                Result = response.Response
            };
        }
        catch(NullReferenceException)
        {
            //No response received within timespan
            _logger.LogError("No response was received for {0}", question.DebugIdentifier);
            return new()
            {
                IsValid = false,
                TimeoutExpired = true,
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error for question {0}", question.DebugIdentifier);
            throw;
        }
        finally
        {
            lock (_questionPersistance)
            {
                _questionPersistance.Remove(question.Id);
            }
            await RemoveQuestion(question.Id);
        }

    }

    #endregion

    #region Answer Question

    public virtual async Task AnswerQuestion(string key, string response)
    {
        object userId = null;
        if (_uICGetCurrentUserId != null)
            userId = await _uICGetCurrentUserId.GetCurrentUserId();
        lock (_questionPersistance)
        {
            if (_questionPersistance.TryGetValue(key, out var question))
            {
                _logger.BeginScopeKvp("UICQuestionIdentifier", question.DebugIdentifier);
                _logger.LogInformation("Answered question {0} with '{1}'", question.DebugIdentifier, response);
                question.Response = response;
                question.Answered = true;
                question.AnsweredByUserId = userId;
                question.Done.SetResult(true);
            }
            else
            {
                _logger.LogWarning("Tried to answer question that does not exist.");
            }
        }
    }
    public virtual async Task CancelQuestion(string key)
    {
        object userId = null;
        if (_uICGetCurrentUserId != null)
            userId = await _uICGetCurrentUserId.GetCurrentUserId();
        lock (_questionPersistance)
        {
            if (_questionPersistance.TryGetValue(key, out var question))
            {
                _logger.BeginScopeKvp("UICQuestionIdentifier", question.DebugIdentifier);
                _logger.LogInformation("Question {0} was cancelled", question.DebugIdentifier);
                question.Answered = true;
                question.Canceled = true;
                question.AnsweredByUserId = userId;
                question.Done.SetResult(false);
            }
        }
    }

    #endregion


    /// <summary>
    /// Remove the question with this id
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    public virtual Task RemoveQuestion(string questionId)
    {
        return _signalRService.RemoveUIComponentWithId(questionId);
    }

}
