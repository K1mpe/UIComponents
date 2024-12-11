using UIComponents.Abstractions.Varia;

namespace UIComponents.Abstractions.Extensions;

public static class ScriptCollectionExtensions
{
    /// <summary>
    /// Add a script that will be rendered by the script collection. 
    /// </summary>
    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, bool inDocReady, RazerBlock razerCode)
    {
        hasScriptCollection.AddScript(inDocReady, new UICCustom(razerCode));
    }

    ///<inheritdoc cref="AddScript(IUICHasScriptCollection, RazerBlock)"/>
    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, bool inDocReady, IUICAction scriptComponent)
    {
        if(inDocReady) 
            hasScriptCollection.ScriptCollection.AddToScriptsDocReady(scriptComponent);
        else
            hasScriptCollection.ScriptCollection.AddToScripts(scriptComponent);
    }

    ///<inheritdoc cref="AddScript(IUICHasScriptCollection, RazerBlock)"/>
    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, bool inDocReady, out UICCustom customForTaghelper)
    {
        customForTaghelper = new UICCustom();
        hasScriptCollection.AddScript(inDocReady, customForTaghelper);
    }


    /// <summary>
    /// Add a script that will be rendered by the script collection. This scriptcollection is rendered inside a $(document).ready
    /// </summary>
    /// <param name="hasScriptCollection"></param>
    /// <param name="razerCode"></param>
    public static void AddScriptDocReady(this IUICHasScriptCollection hasScriptCollection, RazerBlock razerCode)
    {
        hasScriptCollection.ScriptCollection.AddToScriptsDocReady(new UICCustom(razerCode));
    }

    ///<inheritdoc cref="AddScriptDocReady(IUICHasScriptCollection, RazerBlock)"/>
    public static void AddScriptDocReady(this IUICHasScriptCollection hasScriptCollection, IUICAction scriptComponent)
    {
        hasScriptCollection.ScriptCollection.AddToScriptsDocReady(scriptComponent);
    }

    ///<inheritdoc cref="AddScriptDocReady(IUICHasScriptCollection, RazerBlock)"/>
    public static void AddScriptDocReady(this IUICHasScriptCollection hasScriptCollection, out UICCustom customForTaghelper)
    {
        customForTaghelper = new UICCustom();
        hasScriptCollection.AddScriptDocReady(customForTaghelper);
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
