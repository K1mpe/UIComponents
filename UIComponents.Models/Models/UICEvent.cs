
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Reflection;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models;

public class UICEvent : UICEvent<EventArgs>
{

}

/// <summary>
/// Listens to a event backend and transforms it to signalR
/// </summary>
/// <typeparam name="TArgs"></typeparam>
public class UICEvent<TArgs> : IUICAction, IUICConditionalRender, IUICSupportsTaghelperContent, IUICGetComponent
    where TArgs: EventArgs
{

    public UICEvent()
    {
        
    }

    /// <summary>
    /// Create a new instance with a subscribe and unsubscribe of a event
    /// </summary>
    /// <param name="subscribeOnEvent">handler => _myService.MyEvent += handler</param>
    /// <param name="unsubscriveFromEvent">handler => _myService.MyEvent -= handler</param>
    /// <param name="action">a javascript function containing sender and args</param>
    public UICEvent(Action<EventHandler<TArgs>> subscribeOnEvent, Action<EventHandler<TArgs>> unsubscriveFromEvent, IUICAction action = null)
    {
        SubscribeOnEvent = subscribeOnEvent;
        UnsubscribeOnEvent = unsubscriveFromEvent;
        
        if (action != null) 
            Action = action;
    }

    /// <summary>
    /// Create a new instance with the <see cref="EventInfo"/> of a object
    /// </summary>
    /// <param name="service">The service containing the event</param>
    /// <param name="eventInfo">the <see cref="EventInfo"/> from the event in the service</param>
    /// <param name="action">a javascript function containing sender and args</param>
    public UICEvent(object service, EventInfo eventInfo, IUICAction action = null)
    {
        SubscribeOnEvent = handler => eventInfo.AddEventHandler(service, handler);
        UnsubscribeOnEvent = handler => eventInfo.RemoveEventHandler(service, handler);
    }

    private bool _render;
    public bool Render
    {
        get => _render && Action.HasValue();
        set => _render = value;
    }

    public Action<EventHandler<TArgs>> SubscribeOnEvent { get; set; }
    public Action<EventHandler<TArgs>> UnsubscribeOnEvent { get; set; }

    /// <summary>
    /// The action that must be executed when receiving the event
    /// </summary>
    /// <remarks>
    /// Available args: sender, args
    /// </remarks>
    public IUICAction Action { get; set; } = new UICCustom();


    #region Statics
    public static UICEvent<TArgs> Create(Action<EventHandler<TArgs>> subscribeOnEvent, Action<EventHandler<TArgs>> unsubscriveFromEvent, IUICAction action = null)
    {
        return new UICEvent<TArgs>(
            handler => subscribeOnEvent(handler),
            handler => unsubscriveFromEvent(handler),
            action);
    }
    #endregion


    #region IUICSupportsTaghelperContent
    bool IUICSupportsTaghelperContent.CallWithEmptyContent => true;

    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        if(Action == null)
            Action = new UICCustom();
        if (Action is IUICSupportsTaghelperContent supports)
        {
            return supports.SetTaghelperContent(taghelperContent, attributes);
        }
        else
        {
            throw new Exception($"Action does not support {nameof(IUICSupportsTaghelperContent)}");
        }
    }

    Task<IUIComponent> IUICGetComponent.GetComponentAsync(IServiceProvider serviceProvider)
    {
        var storedEvents = serviceProvider.GetService<IUICStoredEvents>();
        var guid = storedEvents.SubscribeOnEvent(SubscribeOnEvent, UnsubscribeOnEvent);
        var signalR = new UICSignalR()
        {
            SubscriptionName = nameof(IUICSignalRService.EventHandler),
            SubscriptionArguments = new() { "key", "sender", "args" },
            Group = guid,
            Condition = new UICCustom($"return key == '{guid}'"),
            Action = Action,
            DisableOnHidden = false
        };
        return Task.FromResult(signalR as IUIComponent);
    }

    #endregion
}
