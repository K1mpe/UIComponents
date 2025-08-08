using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models
{

    public class UICUpload : UIComponent
    {
        #region Fields
        public override string RenderLocation => this.CreateDefaultIdentifier();
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
        /// The MVC Controller location to post the file
        /// </summary>
        /// <remarks>
        /// Example: '/Upload/UploadFile'
        /// </remarks>
        public string PostUrl { get; set; } = Defaults.Models.UICUpload.PostUrl;

        public Dictionary<string, object> PostData { get; set; } = new();

        /* Post Example

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                string filepath= $"...\\{fileName}";
                using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
                return Json("Ok");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw;
            }
        }
        */

        /// <summary>
        /// Max fileSize in mb
        /// </summary>
        public long MaxFileSize { get; set; } = Defaults.Models.UICUpload.MaxFileSize;

        /// <summary>
        /// The maximum amount of files that can be uploaded at a time
        /// </summary>
        public int MaxFileCount { get; set; } = Defaults.Models.UICUpload.MaxFileCount;

        /// <summary>
        /// if not empty, only allow these file types.
        /// </summary>
        /// <remarks>
        /// .jpg,.png
        /// </remarks>
        public string AcceptedFiles { get; set; } = Defaults.Models.UICUpload.AcceptedFiles;

        public Translatable Text { get; set; } = Defaults.Models.UICUpload.Text;

        /// <summary>
        /// Function that is called when the upload is completed, this has the 'file' argument
        /// </summary>
        public IUICAction OnSuccess { get; set; } = new UICCustom();

        /// <summary>
        /// Function that is called when all uploads are completed
        /// </summary>
        public IUICAction OnSuccessAll { get; set; } = new UICCustom();

        /// <summary>
        /// Called when a file failed to upload.
        /// </summary>
        /// <remarks>
        /// Available args: file, message
        /// </remarks>
        public IUICAction OnError { get; set; } = new UICCustom();

        public bool DisplayFileCountMessage { get; set; } = Defaults.Models.UICUpload.DisplayFileCountMessage;

        public bool AllowChunking { get; set; } = Defaults.Models.UICUpload.AllowChunking;
        /// <summary>
        /// The size of a single chunk in MB
        /// </summary>
        public int  ChunkSizeMB { get; set; } = Defaults.Models.UICUpload.ChunkSizeMB;
        public int ParallelUploads { get; set; } = Defaults.Models.UICUpload.ParallelUploads;

        #endregion
    }
}
namespace UIComponents.Defaults.Models
{
    public static class UICUpload
    {


        /// <summary>
        /// The MVC Controller location to post the file
        /// </summary>
        /// <remarks>
        /// Example: '/Upload/UploadFile'
        /// </remarks>
        public static string PostUrl { get; set; }


        /// <summary>
        /// Max fileSize in mb
        /// </summary>
        public static long MaxFileSize { get; set; }

        /// <summary>
        /// The maximum amount of files that can be uploaded at a time
        /// </summary>
        public static int MaxFileCount { get; set; } = 1;

        /// <summary>
        /// if not empty, only allow these file types.
        /// </summary>
        /// <remarks>
        /// .jpg,.png
        /// </remarks>
        public static string AcceptedFiles { get; set; }

        public static Translatable Text { get; set; } = TranslatableSaver.Save("Upload.Text", "Drop files here or click to upload");

        public static bool DisplayFileCountMessage { get; set; } = true;



        public static bool AllowChunking { get; set; }
        public static int ChunkSizeMB { get; set; } = 2;
        public static int ParallelUploads { get; set; } = 0;


        public static string DropzoneCss { get; set; } = "/lib/dropzone/dist/dropzone.min.css";

        /// <summary>
        /// The location of the dropzone.js script file
        /// </summary>
        public static string DropzoneScript { get; set; } = "/lib/dropzone/dist/dropzone.js";
    }
}