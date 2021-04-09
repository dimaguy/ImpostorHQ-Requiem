namespace ImpostorHqR.Extension.Api.Loader
{
    public interface IExtensionEntryPoint
    {
        string PackageName { get; }

        string Author { get; }

        string Version { get; }

        string Name { get; }

        int ApiVersion { get; }
    }
}
