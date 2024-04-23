namespace UIComponents.Models.Models.Tables.TableColumns
{
    public class UICTableColumnPartial : UICTableColumn
    {
        #region Fields
        public override string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICTableColumnPartial));
        #endregion

        #region Ctor
        public UICTableColumnPartial()
        {
            Type = "uic-rowcontent";

        }
        public UICTableColumnPartial(UICActionGetPost getPost) : this()
        {
            GetPost = getPost;
        }
        #endregion

        #region Properties

        public UICActionGetPost GetPost { get; set; }

        /// <summary>
        /// Whether multiple rows can be opened at once or if the open row should be closed when opening another
        /// </summary>
        public bool Multiple { get; set; }
        #endregion
    }
}
