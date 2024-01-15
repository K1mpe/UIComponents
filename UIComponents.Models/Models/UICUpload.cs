using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models;

public class UICUpload : UIComponent
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICUpload() : base()
    {

    }
    public UICUpload(string postUrl) : this()
    {
        PostUrl = postUrl;
    }
    #endregion

    #region Properties

    /// <summary>
    /// This is the render page to process this object
    /// </summary>
    /// <remarks>
    /// /Views/Shared/Component/UIComponent/Upload/_Renderer.cshtml
    /// </remarks>
    public UploadRenderer Renderer { get; set; } = UploadRenderer.Dropzone1;

    /// <summary>
    /// The MVC Controller location to post the file
    /// </summary>
    /// <remarks>
    /// Example: '/Upload/UploadFile'
    /// </remarks>
    public string PostUrl { get; set; }

    /// <summary>
    /// Max fileSize in mb
    /// </summary>
    public long MaxFileSize { get; set; }

    /// <summary>
    /// The maximum amount of files that can be uploaded at a time
    /// </summary>
    public int MaxFileCount { get; set; } = 1;

    /// <summary>
    /// if not empty, only allow these file types.
    /// </summary>
    /// <remarks>
    /// .jpg,.png
    /// </remarks>
    public string AcceptedFiles { get; set; }

    public ITranslateable Text { get; set; } = new TranslationModel("Upload.Text", "Drop files here or click to upload");

    public bool DisplayFileCountMessage { get; set; } = true;
    #endregion

}
public enum UploadRenderer
{
    Dropzone1
}