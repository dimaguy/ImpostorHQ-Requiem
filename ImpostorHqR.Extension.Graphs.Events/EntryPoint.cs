using ImpostorHqR.Extension.Api.Loader;

namespace ImpostorHqR.Extension.Graphs.Events
{
    public class EntryPoint : IExtensionEntryPoint
    {
        public string PackageName => "dima.std.graphs.events";

        public string Author => "dima";

        public string Version => "zero";

        public string Name => "Impostor Event Graphs";

        public int ApiVersion => 0;
    }
}
