namespace UIComponents.Models.Models;

public class UICScriptCollection : IUICScriptCollection
{
    #region Properties
    public string RenderParentId { get; set; }



    protected List<IUIAction> Scripts { get; set; } = new();

    protected List<IUIComponent> Styles { get; set; } = new();
    #endregion

    #region Methods
    public void AddToScripts(IUIAction script)
    {
        Scripts.Add(script);
    }

    public void AddToScripts(IEnumerable<IUIAction> scripts)
    {
        Scripts.AddRange(scripts);
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
    public List<IUIAction> GetScripts(string renderParentId)
    {
        if (RenderParentId != renderParentId)
            return new();
        return Scripts;
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
        foreach (var style in Styles)
        {
            otherCollection.AddToStyles(style);
        }
        Scripts = new();
        Styles = new();
    }


    #endregion
}
