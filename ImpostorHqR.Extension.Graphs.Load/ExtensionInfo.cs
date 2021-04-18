using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Extension.Graphs.Load
{
    public class ExtensionInfo : IExtensionInformation
    {
        public string Package => "dima.std.graphs.load";

        public string Author => "dima";

        public string Version => "zero";

        public string DisplayName => "Requiem Load Graph";

        public short ApiVersion => 0;
    }
}
