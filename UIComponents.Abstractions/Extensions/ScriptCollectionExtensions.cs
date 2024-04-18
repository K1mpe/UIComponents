using UIComponents.Abstractions.Varia;

namespace UIComponents.Abstractions.Extensions;

public static class ScriptCollectionExtensions
{

    /// <summary>
    /// Add a script that will be rendered by the script collection. This scriptcollection is rendered inside a $(document).ready
    /// </summary>
    /// <param name="hasScriptCollection"></param>
    /// <param name="razerCode"></param>
    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, RazerBlock razerCode)
    {
        hasScriptCollection.ScriptCollection.AddToScripts(new UICCustom(razerCode));
    }

    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, IUICAction scriptComponent)
    {
        hasScriptCollection.ScriptCollection.AddToScripts(scriptComponent);
    }

    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, out UICCustom customForTaghelper)
    {
        customForTaghelper = new UICCustom();
        hasScriptCollection.AddScript(customForTaghelper);
    }

    public static void AddStyle(this IUICHasScriptCollection hasScriptCollection, IUIComponent styleComponent)
    {
        hasScriptCollection.ScriptCollection.AddToStyles(styleComponent);
    }
    public static void AddStyle(this IUICHasScriptCollection hasScriptCollection, RazerBlock razerCode)
    {
        hasScriptCollection.ScriptCollection.AddToStyles(new UICCustom(razerCode));
    }
}
