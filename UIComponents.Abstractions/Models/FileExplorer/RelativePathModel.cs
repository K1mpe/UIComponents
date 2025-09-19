using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Abstractions.Models.FileExplorer
{
    public class RelativePathModel : IRelativePath
    {
        public string AbsolutePathReference { get; set; }

        public string RelativePath { get; set; }


        public static RelativePathModel FromBase64String(string base64String)
        {
            var jsonString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64String));
            return System.Text.Json.JsonSerializer.Deserialize<RelativePathModel>(jsonString);
        }
        public string ToBase64String()
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(this);
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonString));
        }
    }
}
