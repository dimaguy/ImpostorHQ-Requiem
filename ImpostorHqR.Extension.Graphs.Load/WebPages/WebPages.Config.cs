using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    [Configuration(false, true)]
    public class WebPageConfig
    {
        public bool EnableThreadPage { get; set; } = false;
        public string ThreadPageHandle { get; set; } = "threads";

        public bool EnableExceptionsPage { get; set; } = true;
        public string ExceptionsPageHandle { get; set; } = "exceptions";

        public bool EnableLoadPage { get; set; } = true;
        public string LoadPageHandle { get; set; } = "system";
        public byte LoadPageWidth { get; set; } = 100;
        public byte LoadPageIntervalSeconds { get; set; } = 1;
    }
}
