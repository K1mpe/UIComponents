namespace UIComponents.Abstractions.Interfaces
{
    public interface IUICScriptCollection
    {
        string RenderParentId { get; set; }
        void AddToScripts(IUIAction script);
        void AddToScripts(IEnumerable<IUIAction> scripts);
        void AddToStyles(IUIComponent style);
        void AddToStyles(IEnumerable<IUIComponent> styles);
        List<IUIAction> GetScripts(string renderParentId);
        List<IUIComponent> GetStyles(string renderParentId);

        /// <summary>
        /// All the scripts and styles that are currently available in this collection get moved to the other collection.
        /// </summary>
        /// <param name="otherCollection"></param>
        public void MergeIntoOtherColllection(ref IUICScriptCollection otherCollection);
    }
}