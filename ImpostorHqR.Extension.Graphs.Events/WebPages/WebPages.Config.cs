using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    [Configuration(false, true)]
    public class WebPageConfig
    {
        #region Chat Page

        public bool EnableChatPage { get; set; } = true;
        public string ChatPageHandle { get; set; } = "playerchat";
        public bool ChatPageRequiresAuthentication { get; set; } = false;
        public string ChatPageUser { get; set; } = "the doctor";
        public string ChatPagePassword { get; set; } = "_nsakey";

        #endregion

        #region Events Page

        public bool EnableEventsPage { get; set; } = true;
        public string EventsPageHandle { get; set; } = "impostorevents";
        public bool EventsPageRequiresAuthentication { get; set; } = false;
        public string EventsPageUser { get; set; } = "the doctor";
        public string EventsPagePassword { get; set; } = "theOncomingStorm";
        public byte EventsPageWidth { get; set; } = 100;
        public ushort EventsPageUpdateIntervalSeconds { get; set; } = 1;

        #endregion

    }
}
