
using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Generators.Services;

public class UICFileExplorerPathMapper : IUICFileExplorerPathMapper
{
    protected Dictionary<string, string> PathMapper { get; set; } = new();

    public virtual string RegisterPath(string basePath)
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

    public virtual string GetAbsolutePath(IRelativePath relativePath)
    {
        lock (PathMapper)
        {
            if (string.IsNullOrWhiteSpace(relativePath.AbsolutePathReference))
                return relativePath.RelativePath?.Replace("/", "\\");
            if (PathMapper.TryGetValue(relativePath.AbsolutePathReference, out var path))
            {
                var fullpath = ReplaceRoot(relativePath.RelativePath, "~", path);
                return fullpath.Replace("/","\\");
            }
        }

        return relativePath.RelativePath.Replace("/", "\\");
    }


    public virtual T GetRelativePath<T>(string absolutePath) where T : class, IRelativePath
    {
        lock (PathMapper)
        {
            absolutePath = absolutePath.Replace("/", "\\");
            var dictValues = PathMapper.Where(x => absolutePath.StartsWith(x.Value));
            var instance = Activator.CreateInstance<T>();
            if (!dictValues.Any())
            {
                instance.RelativePath = absolutePath.Replace("\\","/");
                instance.AbsolutePathReference = string.Empty;
                return instance;
            }

            //Take the longest matching path
            var longest = dictValues.OrderByDescending(x => x.Value.Length).First();
            instance.AbsolutePathReference = longest.Key;
            instance.RelativePath = ReplaceRoot(absolutePath, longest.Value, "~").Replace("\\", "/");
            return instance;
        }
    }

    public virtual string ReplaceRoot(string path, string sourceRoot, string targetRoot)
    {
        if (!path.StartsWith(sourceRoot))
            throw new ArgumentException(path);
        //Do not use the ReplaceMethod since this could replace multiple times in the directory
        return $"{targetRoot}{path.Substring(sourceRoot.Length)}";
    }
}
