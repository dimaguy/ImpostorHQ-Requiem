using System.IO;

namespace ImpostorHqR.Core.Extension.Loader
{
    public static class ExtensionLoaderConstant
    {
        public const int ApiVersion = 0;
        public const string ExtensionDir = "hq";
        public static readonly string ExtensionDataDir = Path.Combine(ExtensionDir, "data");
    }
}
