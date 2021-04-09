using System.Collections.Generic;

namespace ImpostorHqR.Core.Extension.Loader.ExtensionStore
{
    public class ExtensionStore
    {
        public static readonly ExtensionStore Instance = new ExtensionStore();

        public List<Extension> Extensions { get; private set; }

        public ExtensionStore()
        {
            this.Extensions = new List<Extension>();
        }

        public void AddExtension(Extension ext)
        {
            lock (Extensions) Extensions.Add(ext);
        }
    }
}
