using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Abstractions.Models.HtmlResponses;
using UIComponents.Abstractions.Varia;
using UIComponents.Models.Models.Actions;
using UIComponents.Models.Models.Card;
using UIComponents.Web.Components;
using UIComponents.Web.Helpers;
using UIComponents.Web.Interfaces.FileExplorer;
using UIComponents.Web.Models;

namespace UIComponents.Web.Tests.Controllers;

public class UICFileExplorerController : Controller, IUICFileExplorerController
{
    private readonly ILogger _logger;
    private readonly IUICLanguageService _languageService;
    private readonly IUICFileExplorerService _fileExplorerService;
    private readonly IUICFileExplorerPathMapper _pathMapper;
    private readonly IUICFileExplorerPermissionService _permissionService;
    private readonly IUICFileExplorerExecuteActions _actions;
    public UICFileExplorerController(ILogger<UICFileExplorerController> logger, IUICFileExplorerService fileExplorerService, IUICFileExplorerPathMapper pathMapper, IUICLanguageService languageService, IUICFileExplorerExecuteActions actions, IUICFileExplorerPermissionService permissionService = null)
    {
        _logger = logger;
        _fileExplorerService = fileExplorerService;
        _pathMapper = pathMapper;
        _languageService = languageService;
        _permissionService = permissionService;
        _actions = actions;
    }

    public virtual async Task<IActionResult> CopyFiles(RelativePathModel[] FromPath, RelativePathModel ToPath)
    {
        try
        {
            await _fileExplorerService.CopyFilesAsync(FromPath.ToList(), ToPath);
            return Json(true);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> CreateDirectory(RelativePathModel pathModel)
    {
        try
        {
            await _fileExplorerService.CreateDirectory(pathModel);
            return Json(true);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> DeleteFiles(RelativePathModel[] pathModel)
    {
        try
        {
            await _fileExplorerService.DeleteFilesAsync(pathModel.ToList());
            return Json(true);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> Download(RelativePathModel[] pathModels)
    {
        try
        {
            var files = pathModels.Select(x => _pathMapper.GetAbsolutePath(x));
            return await UICFileExplorerHelper.DownloadFileOrZipStream(files, HttpContext, _logger);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetFilesForDirectoryPartial(GetFilesForDirectoryFilterModel fm)
    {
        try
        {
            var result = await _fileExplorerService.GetFilesFromDirectoryAsync(fm, Request.HttpContext.RequestAborted);
            string renderLocation = fm.RenderLocation;
            if (!renderLocation.Contains("/"))
                renderLocation = "/UIComponents/ComponentViews/FileExplorer/ExplorerViews/" + renderLocation;
            return ViewComponent(typeof(UICViewComponent), new UICViewModel(renderLocation, result));
        }
        catch (OperationCanceledException)
        {
            return Json(false);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }
    [HttpPost]
    public virtual async Task<IActionResult> GetFilesForDirectoryJson(GetFilesForDirectoryFilterModel fm)
    {
        try
        {
            var result = await _fileExplorerService.GetFilesFromDirectoryAsync(fm, Request.HttpContext.RequestAborted);
            return Json(result);
        }
        catch (OperationCanceledException)
        {
            return Json(false);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> MoveFiles(RelativePathModel[] FromPath, RelativePathModel ToPath)
    {
        try
        {
            await _fileExplorerService.MoveFilesAsync(FromPath.ToList(), ToPath);
            return Json(true);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }


    [HttpGet]
    public virtual async Task<IActionResult> OpenFile(string base64)
    {
        try
        {
            base64 = base64.Replace(" ", "+");
            var json = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var pathModel = JsonSerializer.Deserialize<RelativePathModel>(json);

            var absolutePath = _pathMapper.GetAbsolutePath(pathModel);
            if (_permissionService != null && !await _permissionService.CurrentUserCanOpenFileOrDirectory(absolutePath))
                return await Error(TranslatableSaver.Save("FileExplorer.OpenFile.AccessDenied", "You do not have access to open {0}", pathModel.RelativePath));
            var memstream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);

            return File(memstream, GetMimeType(absolutePath));
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> OpenImage(RelativePathModel pathModel, string explorerId)
    {
        try
        {
            var vm = new ImageViewerViewModel()
            {
                FilePath = pathModel,
                ExplorerContainerId = explorerId,
                ControllerName = this.GetType().Name
            };
            if (vm.ControllerName.EndsWith("Controller"))
                vm.ControllerName = vm.ControllerName.Remove(vm.ControllerName.LastIndexOf("Controller"));
            return PartialView("/UIComponents/ComponentViews/FileExplorer/ImageViewer.cshtml", vm);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> Preview(RelativePathModel pathModel)
    {
        try
        {
            var result = await _fileExplorerService.GetFilePreviewAsync(pathModel, HttpContext.RequestAborted);
            if (result == null)
                return Json(false);
            return ViewOrPartial(new UICViewModel("/UIComponents/ComponentViews/FileExplorer/FilePreview", result));
        }
        catch (OperationCanceledException)
        {
            return Json(false);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> Rename(RelativePathModel pathModel, string newName)
    {
        try
        {
            await _fileExplorerService.RenameFileOrDirectoryAsync(pathModel, newName);
            return Json(true);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }

    public virtual async Task<IActionResult> UploadPartial(RelativePathModel directoryPathModel)
    {
        try
        {
            var absolutePath = _pathMapper.GetAbsolutePath(directoryPathModel);
            if (_permissionService != null && !await _permissionService.CurrentUserCanCreateFileInThisDirectory(absolutePath))
                return await Error(TranslatableSaver.Save("FileExplorer.NoAccessToUploadFiles", "You do not have access to upload files in {0}", directoryPathModel.RelativePath));

            var modal = new UICModal(directoryPathModel.RelativePath);

            var uploader = await _fileExplorerService.GetUploadFileComponent(directoryPathModel);
            if(uploader != null)
            {
                modal.Add(uploader);
                return ViewOrPartial(modal);
            }

            modal.Add(new UICUpload(Url.Action(nameof(UploadFiles))), upload =>
            {
                upload.AllowChunking = true;
                upload.ParallelUploads = 5;
                upload.MaxFileCount = 100;
                upload.ChunkSizeMB = 25;
                upload.DisplayFileCountMessage = false;
                upload.PostData["directoryPathModel"] = directoryPathModel;
                upload.OnSuccessAll = new UICCustom()
                    .AddLine($"$('#{modal.GetId()}').trigger('uic-close');")
                    .AddLine($"$('.file-explorer-container').trigger('uic-reload');");
            });
            return ViewOrPartial(modal);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }
    public virtual async Task<IActionResult> UploadFiles(RelativePathModel directoryPathModel)
    {
        try
        {
            var files = Request.Form.Files;
            var absolutePath = _pathMapper.GetAbsolutePath(directoryPathModel);
            if (_permissionService != null && !await _permissionService.CurrentUserCanCreateFileInThisDirectory(absolutePath))
                return await Error(TranslatableSaver.Save("FileExplorer.NoAccessToUploadFiles", "You do not have access to upload files in {0}", directoryPathModel.RelativePath));

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file.FileName);
                string filepath = Path.Combine(absolutePath, fileName);

                if (_permissionService != null && !await _permissionService.CurrentUserCanCreateOrEditFile(filepath))
                {
                    var relativePath = _pathMapper.GetRelativePath(filepath);
                    return await Error(TranslatableSaver.Save("FileExplorer.NoAccessToUploadFile", "You do not have access to upload file in {0}", directoryPathModel.RelativePath));
                }
            }
            await UICFileExplorerHelper.UploadFilesFromDropzoneStream(HttpContext, absolutePath, (target, stream) => _actions.AddFile(target, stream), _logger);

            return Json(true);
        }
        catch (Exception ex)
        {
            return await Error(ex);
        }
    }




    protected virtual IActionResult ViewOrPartial(IUIComponent component)
    {
        if (IsAjaxReques(Request))
            return PartialView("/UIComponents/ComponentViews/Render.cshtml", component);
        return View("/UIComponents/ComponentViews/Render.cshtml", component);
    }
    protected bool IsAjaxReques(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Headers != null)
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";

        return false;
    }
    protected virtual Task<IActionResult> Error(Exception ex)
    {
        if (ex is ArgumentStringException stringException)
        {
            _logger.LogError(ex, stringException.UnformattedMessage, stringException.Arguments);
            return Error(new Translatable(stringException.UnformattedMessage, stringException.UnformattedMessage, stringException.Arguments));
        }

        _logger.LogError(ex, ex.Message);
        return Error(ex.Message);
    }
    protected virtual async Task<IActionResult> Error(Translatable message = null)
    {
        var translated = await _languageService.Translate(message);
        HttpContext.Response.StatusCode = 500;
        return Json(new UICToastResponse()
        {
            Notification = new UICToastRNotification(IUICToastNotification.ToastType.Error, translated)
        });

    }

    protected virtual string GetMimeType(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        string mimeType = "application/octet-stream"; // default MIME type
        RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileInfo.Extension.ToLower());

        if (key != null)
        {
            object value = key.GetValue("Content Type");
            if (value != null)
            {
                mimeType = value.ToString();
            }
        }

        return mimeType;
    }
}
