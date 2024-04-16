
using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Generators.Services;

public class UICFileExplorerPathMapper : IFileExplorerPathMapper
{
    protected Dictionary<string, string> PathMapper { get; set; } = new();

    public string RegisterPath(string basePath)
    {
        lock (PathMapper)
        {
            if (PathMapper.TryGetValue(basePath, out var result))
                return result;

            bool keyexists = true;
            string key = string.Empty;
            while (keyexists)
            {
                key = Guid.NewGuid().ToString("N").Substring(0, 6);
                keyexists = PathMapper.ContainsKey(key);
            }
            PathMapper[key] = basePath;
            return key;
        }

    }

    public string GetAbsolutePath(IRelativePath relativePath)
    {
        lock (PathMapper)
        {
            if (string.IsNullOrWhiteSpace(relativePath.AbsolutePathReference))
                return relativePath.RelativePath?.Replace("/", "\\");
            if (PathMapper.TryGetValue(relativePath.AbsolutePathReference, out var path))
            {
                var fullpath = path + relativePath.RelativePath.Substring(1);
                return fullpath.Replace("/","\\");
            }
        }

        return relativePath.RelativePath.Replace("/", "\\");
    }


    public T GetRelativePath<T>(string absolutePath) where T : class, IRelativePath
    {
        lock (PathMapper)
        {
            var dictValues = PathMapper.Where(x => absolutePath.StartsWith(x.Key));
            var instance = Activator.CreateInstance<T>();
            if (!dictValues.Any())
            {
                instance.RelativePath = absolutePath.Replace("\\","/");
                instance.AbsolutePathReference = string.Empty;
                return instance;
            }

            //Take the longest matching path
            var longest = dictValues.OrderByDescending(x => x.Key.Length).First();
            instance.AbsolutePathReference = longest.Key;
            instance.RelativePath = "~" + absolutePath.Substring(longest.Value.Length).Replace("\\", "/");
            return instance;
        }
    }
}
