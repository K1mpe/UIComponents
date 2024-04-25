using UIComponents.Abstractions.Interfaces.Tables;

namespace UIComponents.Models.Models.Tables.TableColumns
{
    public class UICTableColumnPartial : IUICTableColumn
    {
        #region Fields
        public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICTableColumnPartial));
        #endregion

        #region Ctor
        public UICTableColumnPartial()
        { 

        }
        public UICTableColumnPartial(UICActionGetPost getPost) : this()
        {
            GetPost = getPost;
            if (getPost.GetVariableData == null)
                getPost.GetVariableData = new UICCustom("item");
        }
        #endregion

        #region Properties

        public string Type => "rowcontent";
        public UICActionGetPost GetPost { get; set; }

        /// <summary>
        /// The identifier to get a unique value of the row
        /// </summary>
        public string Identifier { get; set; } = "Id";

        /// <summary>
        /// This function takes arguments 'value' and 'item'. If this function returns false, the element is not rendered. 
        /// </summary>
        public IUICAction Validation { get; set; }

        /// <summary>
        /// Whether multiple rows can be opened at once or if the open row should be closed when opening another
        /// </summary>
        public bool Multiple { get; set; }

        public bool Render { get; set; } = true;

        #endregion
    }
}
