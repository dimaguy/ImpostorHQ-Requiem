using ImpostorHqR.Extension.Api.Loader.Timeline;

// ReSharper disable UnusedMember.Global

namespace ImpostorHqR.Extension.Api.Loader
{
    public abstract class ExtensionProxy
    {
        public IPreInitializationEvent PreInitialization { get; set; }

        public IInitializationEvent Initialization { get; set; }

        public IPostInitializationEvent PostInitialization { get; set; }

        public abstract void PreInit(IPreInitializationEvent instance);

        public abstract void Init(IInitializationEvent instance);

        public abstract void PostInit(IPostInitializationEvent instance);

        public abstract void Shutdown();
    }
}
