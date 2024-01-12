namespace UIComponents.Models.Extensions;

public static class UICChildExtensions
{
    /// <summary>
    /// Add a child element to the parent. If possible, assign the parent to this item using <see cref="IUICHasParent"/>
    /// </summary>
    public static T Add<T, TChildItem>(this T parent, TChildItem child) where T : class, IUIComponent, IUICHasChildren<TChildItem>
    {
        if (child is IUIComponent component)
            component.AssignParent(parent);

        parent.Children.Add(child);
        return parent;
    }



    /// <summary>
    /// Add a child element to the parent and out this added element. If possible, assign the parent to this item using <see cref="IUICHasParent"/>
    /// </summary>
    public static T Add<T, TItem, TChildItem>(this T parent, out TItem addedChild, TItem child) where T : class, IUIComponent, IUICHasChildren<TChildItem> where TItem : TChildItem
    {
        addedChild = child;
        return parent.Add((TChildItem)child);
    }

    /// <summary>
    /// <inheritdoc cref="Add{T, TChildItem}(T, TChildItem)"/>
    /// </summary>
    public static T Add<T>(this T parent, IUIComponent child) where T : class, IUIComponent, IUICHasChildren<IUIComponent>
    {
        return parent.Add<T, IUIComponent>(child);
    }

    /// <summary>
    /// <inheritdoc cref="Add{T, TItem, TChildItem}(T, out TItem, TItem)"/>
    /// </summary>
    public static T Add<T, TItem>(this T parent, out TItem addedChild, TItem child) where T : class, IUIComponent, IUICHasChildren<IUIComponent> where TItem : IUIComponent
    {
        return parent.Add<T, TItem, IUIComponent>(out addedChild, child);
    }
}
