namespace UIComponents.Abstractions.Models;

public class UICScriptCollection : IUICScriptCollection
{
    #region Properties
    public string RenderParentId { get; set; }



    protected List<IUICAction> Scripts { get; set; } = new();
    protected List<IUICAction> ScriptsDocReady { get; set; } = new();

    protected List<IUIComponent> Styles { get; set; } = new();


    #endregion

    #region Methods
    public void AddToScripts(IUICAction script)
    {
        Scripts.Add(script);
    }

    public void AddToScripts(IEnumerable<IUICAction> scripts)
    {
        Scripts.AddRange(scripts);
    }

    public void AddToScriptsDocReady(IUICAction script)
    {
        ScriptsDocReady.Add(script);
    }

    public void AddToScriptsDocReady(IEnumerable<IUICAction> scripts)
    {
        ScriptsDocReady.AddRange(scripts);
    }

    public void AddToStyles(IUIComponent style)
    {
        Styles.Add(style);
    }

    public void AddToStyles(IEnumerable<IUIComponent> styles)
    {
        Styles.AddRange(styles);
    }

    /// <summary>
    /// Get all the scripts in the collection, only if the id matches
    /// </summary>
    /// <param name="renderParentId"></param>
    /// <returns></returns>
    public List<IUICAction> GetScripts(string renderParentId)
    {
        if (RenderParentId != renderParentId)
            return new();
        return Scripts;
    }

    public List<IUICAction> GetScriptsDocReady(string renderParentId)
    {
        if (RenderParentId != renderParentId)
            return new();
        return ScriptsDocReady;
    }

    /// <summary>
    /// Get all the styles in the collection, only if the id matches
    /// </summary>
    /// <param name="renderParentId"></param>
    /// <returns></returns>
    public List<IUIComponent> GetStyles(string renderParentId)
    {
        if (RenderParentId != renderParentId)
            return new();
        return Styles;
    }

    public void MergeIntoOtherColllection(ref IUICScriptCollection otherCollection)
    {
        foreach (var script in Scripts)
        {
            otherCollection.AddToScripts(script);
        }
        foreach (var script in ScriptsDocReady)
        {
            otherCollection.AddToScriptsDocReady(script);
        }
        foreach (var style in Styles)
        {
            otherCollection.AddToStyles(style);
        }
        Scripts = new();
        ScriptsDocReady = new();
        Styles = new();
    }


    #endregion
}
