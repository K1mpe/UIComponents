using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Abstractions.Interfaces.FileExplorer;

public interface IFileExplorerPathMapper
{
    /// <summary>
    /// By providing a path, this will be converted to a random string. this string can be used as the <see cref="IRelativePath.AbsolutePathReference"/> to convert a relative path.
    /// </summary>
    /// <param name="basePath"></param>
    /// <returns></returns>
    public string RegisterPath(string basePath);
    
    /// <summary>
    /// Get the absolute path from a relative path, may throw exception when <see cref="IRelativePath.AbsolutePathReference"/> is not found
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public string GetAbsolutePath(IRelativePath relativePath);

    
    /// <summary>
    /// Get a relative path if the absolute path stars with a path that is already registrated in <see cref="RegisterPath(string)"/>
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <returns></returns>
    public T GetRelativePath<T>(string absolutePath) where T : class, IRelativePath;
}

public static class IFileExplorerPathMapperExtensions
{
    public static RelativePathModel GetRelativePath(this IFileExplorerPathMapper pathMapper, string absolutePath)
    {
        return pathMapper.GetRelativePath<RelativePathModel>(absolutePath);
    }
}
