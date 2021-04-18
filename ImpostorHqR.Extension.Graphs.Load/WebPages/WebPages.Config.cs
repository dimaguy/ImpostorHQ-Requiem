using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    [Configuration(false, true)]
    public class WebPageConfig
    {
        #region Thread Page

        public bool EnableThreadPage { get; set; } = false;
        public string ThreadPageHandle { get; set; } = "threads";
        public bool ThreadPageRequiresAuthentication { get; set; } = false;
        public string ThreadPageUser { get; set; } = "useless";
        public string ThreadPagePassword { get; set; } = "debugger";

        #endregion

        #region Exception Page
        
        public bool EnableExceptionsPage { get; set; } = true;
        public string ExceptionsPageHandle { get; set; } = "exceptions";
        public bool ExceptionsPageRequiresAuthentication { get; set; } = false;
        public string ExceptionPageUser { get; set; } = "actually useful";
        public string ExceptionPagePassword { get; set; } = "feature";

        #endregion

        #region Load Page

        public bool EnableLoadPage { get; set; } = true;
        public string LoadPageHandle { get; set; } = "system";
        public bool LoadPageRequiresAuthentication { get; set; } = false;
        public string LoadPageUser { get; set; } = "the most";
        public string LoadPagePassword { get; set; } = "useful feature";
        public byte LoadPageWidth { get; set; } = 100;
        public byte LoadPageIntervalSeconds { get; set; } = 1;

        #endregion
    }
}
