namespace ImpostorHqR.Extension.Api.Interface
{
    /// <summary>
    /// Implement this for easy configs. The config will be created and loaded before PreInit.
    /// </summary>
    public interface IConfigurationHolder
    {
        /// <summary>
        /// Called when your extension generates the config for the first time.
        /// </summary>
        void SetDefaults();
    }
}
