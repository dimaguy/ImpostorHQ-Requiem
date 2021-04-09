using System.Collections.Generic;
using System.Reflection;
using ImpostorHqR.Core.Extension.ComponentBus;
using ImpostorHqR.Extension.Api.Loader;

namespace ImpostorHqR.Core.Extension.Loader
{
    public class Extension
    {
        public string Author => Entry.Author;

        public string PackageName => Entry.PackageName;

        public string FriendlyName => Entry.Name;

        public string PluginVersion => Entry.Version;

        public Assembly Assembly { get; }

        public IExtensionEntryPoint Entry { get; }

        public ExtensionProxy Proxy { get; }

        public List<ComponentBusItem> Exports { get; set; }

        public Extension(IExtensionEntryPoint entryPoint, ExtensionProxy proxy, Assembly assembly)
        {
            this.Assembly = assembly;
            this.Proxy = proxy;
            this.Entry = entryPoint;
        }
    }
}
