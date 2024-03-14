
using UIComponents.Models.Models.Card;

namespace UIComponents.Abstractions.Extensions;

public static class UICCardLikeExtensions
{

    public static void MoveOnlyToolbarToFooter(this IUICCardLike cardLike)
    {
        var btnToolbars = cardLike.FindAllChildrenOfType<UICButtonToolbar>();
        if (btnToolbars.Count != 1)
            return;

        var toolbar = btnToolbars[0];
        cardLike.Footer.Add(toolbar);
    }


    public static void Test()
    {
        var card = new UICCard();
        card.Add(new UICForm()
            .Add(new UICGroup()
                .Add(new UICButtonToolbar())));

        card.MoveOnlyToolbarToFooter();
    }
}
