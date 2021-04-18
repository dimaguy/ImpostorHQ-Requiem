namespace ImpostorHqR.Extension.Api.Configuration
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class ConfigurationAttribute : System.Attribute
    {
        /// <summary>
        /// Indicates if the config should be written when the program is exiting.
        /// This is used to update configs from inside the applications.
        /// </summary>
        public bool SaveOnExit { get; }

        /// <summary>
        /// Indicates if the configuration should be written to the disk immediately, in case it is not present on the disk.
        /// </summary>
        public bool CreateOnLoad { get; }

        public ConfigurationAttribute(bool saveOnExit, bool createDefault)
        {
            this.SaveOnExit = saveOnExit;
            this.CreateOnLoad = createDefault;
        }
    }
}
