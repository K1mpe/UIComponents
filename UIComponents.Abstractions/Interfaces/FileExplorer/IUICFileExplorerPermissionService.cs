using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces.FileExplorer
{
    public interface IUICFileExplorerPermissionService
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
        /// Check if the current user can upload, move or copy files to this directory
        /// </summary>
        Task<bool> CurrentUserCanCreateInThisDirectory(string path);

        /// <summary>
        /// Check if the current user can create a file with this name. This may be a result of a upload, move or copy of a file
        /// <br>It is possible this file already exist, In that case this function should check if you can edit the existing file</br>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> CurrentUserCanCreateOrEditFile(string path);

        Task<bool> CurrentUserCanCreateDirectory(string path);


        Task<bool> CurrentUserCanMoveFileOrDirectory(string path);

        /// <summary>
        /// Check if the current user can change the name of this file or directory. If the newFileName is Null, this is a precheck to enable the rename option.
        /// <br>Before renaming the file the check will happen again with the newFileName included</br>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        Task<bool> CurrentUserCanRenameFileOrDirectory(string path, string newFileName);
        Task<bool> CurrentUserCanDeleteFileOrDirectory(string path); 

    }
}
