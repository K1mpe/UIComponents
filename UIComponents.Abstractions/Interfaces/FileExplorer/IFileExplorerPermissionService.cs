using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces.FileExplorer
{
    public interface IFileExplorerPermissionService
    {
        /// <summary>
        /// Check if this file can be shown in the explorer. This does not mean the user can open this file.
        /// </summary>
        Task<bool> CurrentUserCanViewFileOrDirectory(string path);

        /// <summary>
        /// Check if the user can open or download this file
        /// </summary>
        Task<bool> CurrentUserCanOpenFileOrDirectory(string path);

        /// <summary>
        /// Check if the current user can upload, move or copy files in this directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> CurrentUserCanCreateInThisDirectory(string path);
        Task<bool> CurrentUserCanMoveFileOrDirectory(string path);
        Task<bool> CurrentUserCanRenameFileOrDirectory(string path);
        Task<bool> CurrentUserCanDeleteFileOrDirectory(string path); 

    }
}
