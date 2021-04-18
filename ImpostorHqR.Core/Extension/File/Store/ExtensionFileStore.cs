using System.IO;

namespace ImpostorHqR.Core.Extension.File.Store
{
    public class ExtensionFileStore
    {
        public static readonly ExtensionFileStore Instance = new ExtensionFileStore();

        public string GetConfigPath(Loader.Extension extension, string id)
        {
            return Path.Combine(GetFolder(extension), $"{id}.{extension.Information.Version}.cfg");
        }

        public string GetFolder(Loader.Extension extension)
        {
            var path = Path.Combine("ImpostorHq.Extension.Data", extension.Information.Package);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
