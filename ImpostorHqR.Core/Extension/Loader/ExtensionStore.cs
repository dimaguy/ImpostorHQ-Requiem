using System.Collections.Generic;
using System.Linq;
using ImpostorHqR.Core.Helper;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Service;

namespace ImpostorHqR.Core.Extension.Loader
{
    public class ExtensionStore
    {
        private List<ImpostorHqR.Core.Extension.Loader.Extension> Extensions { get; }

        public IEnumerable<ImpostorHqR.Core.Extension.Loader.Extension> GetExtensions => Extensions.AsEnumerable();

        public ExtensionStore()
        {
            this.Extensions = new List<ImpostorHqR.Core.Extension.Loader.Extension>();
            IServiceManager.RegisterSingleton(this);
        }

        public void Add(LoaderExtension extension)
        {
            var start = ReflectionHelper.CreateInstance<IExtensionRequiem>(extension.Start);
            Extensions.Add(new ImpostorHqR.Core.Extension.Loader.Extension(start, extension.Information, extension.Dependencies));
        }

        public void ExtensionInit()
        {
            foreach (var extension in Extensions) extension.Start.Init();
        }

        public void ExtensionPostInit()
        {
            foreach (var extension in Extensions) extension.Start.PostInit();
        }

        public void ExtensionShutDown()
        {
            foreach (var extension in Extensions) extension.Start.Shutdown();
        }
    }
}
