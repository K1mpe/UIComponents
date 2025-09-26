using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Abstractions.Interfaces.FileExplorer
{

    /// <summary>
    /// After the <see cref="IUICFileExplorerService"/> finds all files and directories in the paths, all instances of this service are called.
    /// <br>First the <see cref="Initialize(GetFilesForDirectoryFilterModel, List{UICFileInfo})"/> is called with all files, allowing you to initialize data outside of the loop/></br>
    /// <br>Then it will sort the manipulators by <see cref="Priority"/> and loop through all files and directories that are found with the <see cref="ManipulateFileInfo(UICFileInfo)"/></br>
    /// </summary>
    public interface IUICFileExplorerFileInfoManipulator
    {

        /// <summary>
        /// Requires to be true to allow files to go through the <see cref="ManipulateFileInfo(UICFileInfo)"/> method
        /// </summary>
        public bool AllowFiles { get; }

        /// <summary>
        /// Requires to be true to allow directories to go through the <see cref="ManipulateFileInfo(UICFileInfo)"/> method
        /// </summary>
        public bool AllowDirectories { get; }


        /// <summary>
        /// Sorting of manipulators, lowest number goes first.
        /// </summary>
        public double Priority { get; }


        /// <summary>
        /// Initialize method ment to prepare anything that can be done outside of the loop
        /// </summary>
        /// <param name="filterModel"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public Task Initialize(GetFilesForDirectoryFilterModel filterModel, List<UICFileInfo> fileInfo);

        /// <summary>
        /// After the loop, the destroy method is called to ensure everything is ready for a next request (there may be multiple loops per request)
        /// </summary>
        /// <returns></returns>
        public Task Destroy();

        /// <summary>
        /// The looped method that may manipulate the <see cref="UICFileInfo"/>
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public Task<UICFileInfo> ManipulateFileInfo(UICFileInfo fileInfo);
    }
}
