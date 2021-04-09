using ImpostorHq.Extension.Api.Loader;

namespace ImpostorHq.Extension.Test
{
    public class Main : IExtensionEntryPoint
    {
        public string PackageName => "uwu.imin.test";

        public string Author => "uwu";

        public string Version => "[earthInHeaven]";

        public string Name => "test";

        public int ApiVersion => 0;
    }
}
