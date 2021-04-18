using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Extension.Loader
{
    public class Extension
    {
        public IExtensionRequiem Start { get; }

        public IExtensionInformation Information { get; }

        public string[] Dependencies { get; }

        public Extension(IExtensionRequiem ext, IExtensionInformation info, string[] dependencies)
        {
            this.Start = ext;
            this.Information = info;
            this.Dependencies = dependencies;
        }
    }
}
