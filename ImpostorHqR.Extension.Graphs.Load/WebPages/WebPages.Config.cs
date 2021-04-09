using ImpostorHqR.Extension.Api.Interface;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    public class WebPageConfig : IConfigurationHolder
    {
        public static WebPageConfig Instance { get; set; }

        public bool EnableThreadPage { get; set; }
        public string ThreadPageHandle { get; set; }

        public bool EnableExceptionsPage { get; set; }
        public string ExceptionsPageHandle { get; set; }

        public bool EnableLoadPage { get; set; }
        public string LoadPageHandle { get; set; }
        public byte LoadPageWidth { get; set; }
        public byte LoadPageIntervalSeconds { get; set; }

        public void SetDefaults()
        {
            this.EnableThreadPage = true;
            this.ThreadPageHandle = "threads";
            this.EnableExceptionsPage = true;
            this.ExceptionsPageHandle = "exceptions";
            this.EnableLoadPage = true;
            this.LoadPageHandle = "system";
            this.LoadPageWidth = 100;
            this.LoadPageIntervalSeconds = 1;
        }
    }
}
