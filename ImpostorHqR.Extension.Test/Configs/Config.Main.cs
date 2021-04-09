using ImpostorHq.Extension.Api.Interface;

namespace ImpostorHq.Extension.Test
{
    public class ConfigHolder : IConfigurationHolder
    {
        public static ConfigHolder Instance;

        public string Message { get; set; }

        public ConfigHolder()
        {
            Instance = this;
        }

        public void SetDefaults()
        {
            this.Message = "default message.";
        }
    }
}