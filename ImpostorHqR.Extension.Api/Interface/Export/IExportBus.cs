namespace ImpostorHqR.Extensions.Api.Interface.Export
{
    /// <summary>
    /// If implemented, your class will be instantiated after PreInit. The fields will be scanned, and other extensions will be able to read from them.
    /// You may use the constructor of your class to assign an instance, that you can then assign values to.
    /// You may also use a primitive box if you intend to export numeric values that may be accessed often.
    /// </summary>
    public interface IExportBus
    {

    }

    /// <summary>
    /// The component hub can be used to read the values from any ExportBus implementations (from extensions/your own).
    /// </summary>
    public interface IComponentHub
    {
        /// <summary>
        /// This function will search for a property of an export bus.
        /// </summary>
        /// <param name="package">The target extension's package name.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The result of the search, which will contain an error state and an object. If the error state is None, the object has been found and returned.</returns>
        ComponentSearchResult Acquire(string package, string name);
    }

    public struct ComponentSearchResult
    {
        public ComponentSearchError Error { get; set; }

        public object Result { get; set; }
    }

    /// <summary>
    /// ExtensionNotFound - The extension with the specified package name is not installed.
    /// ValueNotFound - The specified package does not have an export with the specified property.
    /// None - the target has been acquired.
    /// </summary>
    public enum ComponentSearchError
    {
        None, ExtensionNotFound, ValueNotFound
    }
}
