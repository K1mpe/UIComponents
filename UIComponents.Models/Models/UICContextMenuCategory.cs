using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Models.Models
{
    public class UICContextMenuCategory : IUIComponent, IUICHasScriptCollection
    {
        #region Fields
        public string RenderLocation => this.CreateDefaultIdentifier();

        IUICScriptCollection IUICHasScriptCollection.ScriptCollection { get; set; } = new UICScriptCollection();
        #endregion

        #region Properties
        /// <summary>
        /// The id that is used by <see cref="UICContextMenuItem.Category"/>
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// MenuItem that is used for a sub-dropdown
        /// </summary>
        public UICContextMenuItem MenuItem { get; set; }


        /// <summary>
        /// The function that returns the Html on how to render the menuItems
        /// </summary>
        /// <remarks>
        /// (category, totalMenuItems) => {...}
        /// </remarks>
        public IUICAction CategoryRenderer { get; set; }



        #endregion

        /// <summary>
        /// Group the items with a divider before and after the group
        /// </summary>
        public static UICCustom CategoryRendererGroup(bool addDividers) => new UICCustom($"(c, t) => uic.contextMenu.default.functions.category.group(c, {addDividers.ToString().ToLower()})");

        /// <summary>
        /// Create a single item with the icons
        /// </summary>
        public static UICCustom CategoryRendererIconsOnly(bool addDividers) => new UICCustom($"(c, t) => uic.contextMenu.default.functions.category.iconsOnly(c, {addDividers.ToString().ToLower()})");

        /// <summary>
        /// Create another dropdown menu with these items
        /// </summary>
        public static UICCustom CategoryRendererSubMenu(bool addDividers) => new UICCustom($"uic.contextMenu.default.functions.category.subMenu(c, {addDividers.ToString().ToLower()})");
    }
}
