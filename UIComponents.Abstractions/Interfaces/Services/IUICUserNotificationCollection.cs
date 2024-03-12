namespace UIComponents.Abstractions.Interfaces.Services;

public interface IUICUserNotificationCollection
{
    public void AddNotificationToUserCollection(string userId, IUIComponent component, TimeSpan lifeTime);
}
