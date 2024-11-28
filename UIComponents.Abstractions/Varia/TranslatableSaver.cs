using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Abstractions.Varia;

public static partial class TranslatableSaver
{
    /// <summary>
    /// This is the path the tranlations from UIC are located
    /// </summary>
    public static string UICTranslationFilePath => $"{Directory.GetCurrentDirectory()}\\UIComponents\\Translations.json";

    /// <summary>
    /// Using this method returns the Translatable key. But also enables <see cref="ScanSolution(string, string[])"/> to find this key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Translatable Save(string key, string defaultValue = null, params object[] args)
    {
        return new Translatable(key, defaultValue, args);
    }

    /// <summary>
    /// A merge will add all the translations from addingItems in the current list.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="addingItems"></param>
    /// <param name="replaceTranslationsWithAddedList">if true, take the translation from the adding list if it has a conflict with the list</param>
    /// <param name="addNewKeys">if true, add the addingItems to the list even if the key does not exist yet.</param>
    public static void Merge(this List<TranslatableXmlField> list, IEnumerable<TranslatableXmlField> addingItems, bool replaceTranslationsWithAddedList, bool addNewKeys)
    {
        foreach (var adding in addingItems)
        {
            var fromList = list.Where(x => x.Key == adding.Key).SingleOrDefault();
            if (fromList == null)
            {
                if (!addNewKeys)
                    continue;
                list.Add(adding);
                continue;
            }

            if (adding.LastScanned > fromList.LastScanned)
                fromList.LastScanned = adding.LastScanned;

            foreach (var translation in adding.TranslationsList)
            {
                if (fromList.TranslationsDict.ContainsKey(translation.Code))
                {
                    if (replaceTranslationsWithAddedList)
                        fromList.TranslationsDict[translation.Code] = translation.Translation;
                }
                else
                {
                    fromList.TranslationsList.Add(translation);
                }
            }
        }
    }

    /// <summary>
    /// filter the list to exclude these namespaces
    /// </summary>
    /// <param name="list"></param>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public static List<TranslatableXmlField> ExcludeNamespaces(this List<TranslatableXmlField> list, params string[] namespaces)
    {
        foreach(var namespc in namespaces)
        {
            foreach (var item in list)
            {
                foreach (var file in item.FilesAndLinesUsed.ToList())
                {
                    if (file.File.StartsWith(namespc))
                        item.FilesAndLinesUsed.Remove(file);
                }
            }
        }
        list = list.Where(x => x.FilesAndLinesUsed.Any()).ToList();
        return list;
    }

    /// <summary>
    /// This method checks all files for <see cref="TranslatableSaver.Save(string, string, object[])"/> and returns these values
    /// </summary>
    /// <param name="startDirectory"></param>
    /// <param name="blackListFolders"></param>
    /// <returns></returns>
    public static List<TranslatableXmlField> ScanSolution(string startDirectory = null, params string[] blackListFolders)
    {
        var blackListfoldersList = blackListFolders.ToList();

        var dir = $"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName}\\";
        if (!string.IsNullOrEmpty(startDirectory))
            dir = $"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName}{startDirectory}\\";

        AddToBlacklist(".git\\*");
        AddToBlacklist(".vs\\*");
        AddToBlacklist("*\\bin\\*");
        AddToBlacklist("*\\obj\\*");
        AddToBlacklist("UIComponents.Web.Tests\\*");
        var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).ToList();
        var results = new List<TranslatableXmlField>();

        var regex = new Regex(@"TranslatableSaver\.Save\(""(?<key>[^""]*)""(?:,\s*""(?<defaultValue>[^""]*)"")?(?:,\s*(?<args>.*))?\)");
        var lineToCheck = $"{nameof(TranslatableSaver)}.{nameof(TranslatableSaver.Save)}(";
        foreach (var blacklistFolder in blackListfoldersList)
        {
            if (blacklistFolder.StartsWith("*"))
                files = files.Where(x => !x.Contains(blacklistFolder.Replace("*", ""))).ToList();
            else
                files = files.Where(x => !x.StartsWith(blacklistFolder.Replace("*", ""))).ToList();

            if (!blacklistFolder.EndsWith("*"))
                files = files.Where(x => !x.EndsWith(blacklistFolder.Replace("*", ""))).ToList();
        }
        foreach (var file in files)
        {
            
            Console.WriteLine($"Checking file {file}");
            try
            {
                var lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];


                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        //line = line.Substring(line.IndexOf(lineToCheck)+lineToCheck.Length);

                        string key = match.Groups["key"].Value;
                        string defaultValue = match.Groups["defaultValue"].Success ? match.Groups["defaultValue"].Value : null;
                        int argCount = match.Groups["args"].Success ? match.Groups["args"].Value.Split(',').Length : 0;

                        var existing = results.Where(x => x.Key == key).FirstOrDefault();
                        if (existing == null)
                        {
                            existing = new()
                            {
                                Key = key,
                                ArgumentsCount = argCount
                            };
                            if(defaultValue != null)
                                existing.TranslationsList.Add(new() { Code = "EN", Translation = defaultValue });

                            results.Add(existing);
                        }

                        if (defaultValue != null && existing.TranslationsDict.ContainsKey("EN"))
                        {
                            if(defaultValue != existing.TranslationsDict["EN"])
                            {
                                var firstFileAndLine = existing.FilesAndLinesUsed.First();

                                throw new Exception($"Translatable with key {key} has conflicting default values. \r\n{firstFileAndLine.File}, Line {firstFileAndLine.LineNumber} \r\n{file}, Line {i}");
                            }
                            
                        }
                        if (existing.ArgumentsCount != argCount)
                        {
                            var firstFileAndLine = existing.FilesAndLinesUsed.First();

                            throw new Exception($"Translatable with key {key} has conflicting argument count. \r\n{firstFileAndLine.File}, Line {firstFileAndLine.LineNumber} \r\n{file}, Line {i}");
                        }
                        existing.LastScanned = DateTime.Now;
                        existing.FilesAndLinesUsed.Add(new()
                        {
                            File = file.Replace(dir, ""),
                            LineNumber = i+1
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        return results;
        void AddToBlacklist(string relativeDir)
        {
            if (!relativeDir.StartsWith("*"))
                relativeDir = $"{dir}{relativeDir}";

            blackListfoldersList.Add(relativeDir);
        }
    }


    public static async Task SaveToFileAsync(List<TranslatableXmlField> savedModels, string filepath, bool keepOldKeys, bool overwriteOldTranslations)
    {
        List<TranslatableXmlField> existingResults = new();
        if (File.Exists(filepath))
        {
            existingResults = await LoadFromFileAsync(filepath);
        }

        savedModels.Merge(existingResults, !overwriteOldTranslations, keepOldKeys);
        savedModels = savedModels.OrderBy(x => x.Key).ToList();
        var model = new TranslatableXmlModel()
        {
            Translatables = savedModels
        };
        var fileInfo = new FileInfo(filepath);
        switch (fileInfo.Extension.ToLower())
        {
            case ".xml":
                using (var streamWriter = new StreamWriter(filepath, new FileStreamOptions()
                {
                    Access = FileAccess.Write,
                    Mode = FileMode.Create,
                }))
                {
                    var serialised = Serialize(model);
                    await streamWriter.WriteAsync(serialised);
                }
                break;
            case ".json":
                using (var streamWriter = new StreamWriter(filepath, new FileStreamOptions()
                {
                    Access = FileAccess.Write,
                    Mode = FileMode.Create,
                }))
                {
                    var serialised = JsonSerializer.Serialize(model);
                    await streamWriter.WriteAsync(serialised);
                }
                break;
            default:
                throw new Exception("Unknow file extension. Currently only xml and json are supported");
        }
        

        string Serialize(TranslatableXmlModel model)
        {
            
            var serialiser = new XmlSerializer(model.GetType());
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            using var stringwriter = new StringWriter();
            serialiser.Serialize(stringwriter, model, namespaces);
            return stringwriter.ToString();
        }
    }
    public static async Task<List<TranslatableXmlField>> LoadFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        var fileContent = await File.ReadAllTextAsync(filePath);

        var fileInfo = new FileInfo(filePath);
        if(fileInfo.Extension.ToLower() == ".json")
        {
            var content= JsonSerializer.Deserialize<TranslatableXmlModel>(fileContent);
            return content.Translatables;
        }
        return LoadFromXmlString(fileContent);
    }

    private static List<TranslatableXmlField> LoadFromXmlString(string content)
    {
        XmlSerializer serializer = new(typeof(TranslatableXmlModel));
        using TextReader reader = new StringReader(content);
        var result = serializer.Deserialize(reader) as TranslatableXmlModel;
        return result.Translatables;
    }

    public static async Task<List<TranslatableXmlField>> LoadFromUICAsync() 
    {
        var dir = UICTranslationFilePath;
        if (!File.Exists(dir))
            throw new FileNotFoundException($"{dir} not found. Set UICConfigOptions.AddTranslationFile true to generate file");

        return await LoadFromFileAsync(dir);
    }

    

    public class TranslatableXmlField
    {
        public string Key { get; set; }

        /// <summary>
        /// Dictionary is not supported for Xml serialiser.
        /// </summary>
        [XmlArray("Translations", IsNullable = true), XmlArrayItem("Translation", IsNullable = true)]
        public List<TranslationModel> TranslationsList { get; set; } = new();

        public int ArgumentsCount { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime LastScanned { get; set; }

        [XmlArray("Files", IsNullable = false), XmlArrayItem("FileAndNumber", IsNullable = false)]
        public List<FileAndLine> FilesAndLinesUsed { get; set; } = new();

        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<string, string> TranslationsDict => TranslationsList.ToDictionary(x => x.Code, x => x.Translation);


        
        public override string ToString()
        {
            string result = Key;
            if (TranslationsDict.ContainsKey("EN"))
                result += $" ({TranslationsDict["EN"]})";

            if (FilesAndLinesUsed.Count > 1)
                result += $" x{FilesAndLinesUsed.Count}";

            return result;
        }


        public class FileAndLine
        {
            public string File { get; set;}
            public int LineNumber { get; set; }

            public override string ToString() => $"{File}, Line {LineNumber}";
        }

        public class TranslationModel
        {
            public string Code { get; set; }
            public string Translation { get; set; }

            public override string ToString() => $"{Code} => {Translation}";
        }
    }

    [XmlRoot("TRANSLATION_MODEL", Namespace = null)]
    public class TranslatableXmlModel
    {
        public int UniqueKeys  => Translatables?.Count??0;

        [XmlArray("Translatables", IsNullable = true), XmlArrayItem("Translatable", IsNullable =true)]
        public List<TranslatableXmlField> Translatables { get; set; }
        
    }
}
