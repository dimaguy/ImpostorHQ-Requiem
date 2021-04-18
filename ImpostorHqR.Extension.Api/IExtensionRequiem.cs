namespace ImpostorHqR.Extension.Api
{
    /// <summary>
    /// If implemented, your class will be constructed, and the respective functions will be called.
    /// This creates an ImpostorMin plugin.
    /// An assembly may have multiple plugin implementations.
    /// </summary>
    public interface IExtensionRequiem
    {
        #region Loading Events

        /// <summary>
        /// This function is called after the configs have been loaded.
        /// </summary>
        void Init();

        /// <summary>
        /// This function may be used for accessing data from other plugins.
        /// </summary>
        void PostInit();

        /// <summary>
        /// This function is called when the server is closing.
        /// </summary>
        void Shutdown();

        #endregion
    }
}
