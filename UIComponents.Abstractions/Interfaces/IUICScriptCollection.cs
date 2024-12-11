namespace UIComponents.Abstractions.Interfaces
{
    public interface IUICScriptCollection
    {
        string RenderParentId { get; set; }

        void AddToScripts(IUICAction script);
        void AddToScripts(IEnumerable<IUICAction> scripts);

        void AddToScriptsDocReady(IUICAction script);
        void AddToScriptsDocReady(IEnumerable<IUICAction> scripts);

        void AddToStyles(IUIComponent style);
        void AddToStyles(IEnumerable<IUIComponent> styles);


        List<IUICAction> GetScripts(string renderParentId);
        List<IUICAction> GetScriptsDocReady(string renderParentId);
        List<IUIComponent> GetStyles(string renderParentId);

        /// <summary>
        /// All the scripts and styles that are currently available in this collection get moved to the other collection.
        /// </summary>
        /// <param name="otherCollection"></param>
        public void MergeIntoOtherColllection(ref IUICScriptCollection otherCollection);
    }
}