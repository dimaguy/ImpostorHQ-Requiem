using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Extension.Graphs.Events
{
    public class ExtensionInfo : IExtensionInformation
    {
        public string Author => "dima";

        public string Version => "zero";

        public short ApiVersion => 0;

        public string Package => "dima.std.graphs.events";

        public string DisplayName => "Impostor Event Graphs";
    }
}
