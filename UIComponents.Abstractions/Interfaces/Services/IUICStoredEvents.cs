namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICStoredEvents
{
    /// <summary>
    /// Store this function to be called by a key.
    /// </summary>
    /// <param name="func">Function that should be triggered</param>
    /// <param name="singleUse">If true, this function can only be called once</param>
    /// <returns></returns>
    public string SubscribeIncommingEvent(Func<Dictionary<string, string>, Task> func, TimeSpan lifeTime, bool singleUse);

    /// <summary>
    /// Use the key given by <see cref="SubscribeIncommingEvent(Func{Dictionary{string, string}, Task}, bool)"/> to execute the function
    /// </summary>
    /// <param name="key">The unique key linked to the method</param>
    /// <param name="data">Dictionary of data that is posted</param>
    public Task IncommingSignalRTrigger(string key, Dictionary<string, string> data);


    /// <summary>
    /// Used for the UICEvent to manage and store subscriptions
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    /// <param name="subscribe"></param>
    /// <param name="unsubscribe"></param>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public string SubscribeOnEvent<TArgs>(Action<EventHandler<TArgs>> subscribe, Action<EventHandler<TArgs>> unsubscribe, string? eventId = null) where TArgs : EventArgs;


}