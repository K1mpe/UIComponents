using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Web.Components;
using UIComponents.Web.Interfaces.FileExplorer;

namespace UIComponents.Web.Tests.Controllers
{
    public class UICFileExplorerController : Controller, IFileExplorerController
    {
        private readonly ILogger _logger;
        private readonly IFileExplorerService _fileExplorerService;
        private readonly IFileExplorerPermissionService _permissionService;
        private readonly IFileExplorerPathMapper _fileExplorerPathMapper;

        public UICFileExplorerController(ILogger<UICFileExplorerController> logger, IFileExplorerService fileExplorerService, IFileExplorerPathMapper fileExplorerPathMapper, IFileExplorerPermissionService permissionService = null)
        {
            _logger = logger;
            _fileExplorerService = fileExplorerService;
            _permissionService = permissionService;
            _fileExplorerPathMapper = fileExplorerPathMapper;
        }

        public async Task<IActionResult> CopyFiles((RelativePathModel FromPath, RelativePathModel ToPath)[] files)
        {
            try
            {
                await _fileExplorerService.CopyFilesAsync(files);
                return Json(true);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IActionResult> DeleteFiles(RelativePathModel[] pathModel)
        {
            try
            {
                await _fileExplorerService.DeleteFilesAsync(pathModel);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IActionResult> DownloadFile(RelativePathModel pathModel)
        {
            try
            {
                string absolutePath = _fileExplorerPathMapper.GetAbsolutePath(pathModel);
                if (_permissionService != null && !await _permissionService.CurrentUserCanOpenFileOrDirectory(absolutePath))
                    throw new AccessViolationException();

                FileInfo fileInfo = new FileInfo(absolutePath);

                var memory = new MemoryStream();
                using (var stream = new FileStream(absolutePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, fileInfo.Name);
                //return File(absolutePath, System.Net.Mime.MediaTypeNames.Application.Octet, fileInfo.Name);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public Task<IActionResult> DownloadFileTest() 
        {
            return DownloadFile(new()
            {
                RelativePath = @"C:/Jonas/Nieuw.png",
                AbsolutePathReference=""
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetFilesForDirectory(GetFilesForDirectoryFilterModel fm)
        {
            try
            {
                var result = await _fileExplorerService.GetFilesFromDirectoryAsync(fm, Request.HttpContext.RequestAborted);
                return Json(result);
                return ViewComponent(typeof(UICViewComponent), new UIComponentViewModel(fm.RenderLocation, result));
            }
            catch (OperationCanceledException)
            {
                return Json(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IActionResult> MoveFiles((RelativePathModel FromPath, RelativePathModel ToPath)[] files)
        {
            try
            {
                await _fileExplorerService.MoveFilesAsync(files);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public Task<IActionResult> OpenFiles(RelativePathModel pathModel)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Preview(RelativePathModel pathModel)
        {
            try
            {
                var result = await _fileExplorerService.GetFilePreviewAsync(pathModel, HttpContext.RequestAborted);
                if (result == null)
                    return Json(false);
                return ViewOrPartial(new UIComponentViewModel("/UIComponents/ComponentViews/FileExplorer/FilePreview",result));
            }
            catch (OperationCanceledException)
            {
                return Json(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public Task<IActionResult> UploadFiles(RelativePathModel directoryPathModel)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }




        protected IActionResult ViewOrPartial(IUIComponent component)
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
    }
}
