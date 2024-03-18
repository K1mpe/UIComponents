namespace UIComponents.Abstractions.Extensions;

public static class IUICCardLikeExtensions
{
    public static void MoveFirstElementToFooter<T>(this IUICCardLike cardLike, Action<T> configureMovedElement = null) where T : IUIComponent
    {
        var children = cardLike.Content.GetAllChildren().Where(x=>x.Component is T).ToList();
        if (!children.Any())
            return;

        var child = children.First();
        cardLike.Footer.Add(child.Component);
        child.Parent.RemoveComponent(child.Component);
        if (configureMovedElement != null)
            configureMovedElement((T)child.Component);
        return;
    }

}
