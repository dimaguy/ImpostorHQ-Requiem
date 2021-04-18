namespace ImpostorHqR.Extension.Api
{
    public interface IExtensionInformation
    {
        /// <summary>
        /// This string represents a notation for a certain context, similar to namespaces.
        /// E.G: author.project.module
        /// </summary>
        string Package { get; }

        string Author { get; }

        string Version { get; }

        /// <summary>
        /// The name that will be displayed in the console (along with the package name).
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The API version your plugin was compiled against. Go to the VERSION.cs file and copy it from there.
        /// This is used to identify version mis-matches.
        /// </summary>
        short ApiVersion { get; }
    }
}
