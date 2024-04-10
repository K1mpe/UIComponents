using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Web.Interfaces.FileExplorer;

namespace UIComponents.Web.Tests.Controllers
{
    public class UICFileExplorerController : Controller, IFileExplorerController
    {
        private readonly ILogger _logger;
        private readonly IFileExplorerService _fileExplorerService;

        public UICFileExplorerController(ILogger<UICFileExplorerController> logger, IFileExplorerService fileExplorerService)
        {
            _logger = logger;
            _fileExplorerService = fileExplorerService;
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
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IActionResult> GetFilesForDirectory(GetFilesForDirectoryFilterModel fm)
        {
            try
            {
                var result = await _fileExplorerService.GetFilesFromDirectoryAsync(fm, Request.HttpContext.RequestAborted);
                return Json(result);
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
    }
}
