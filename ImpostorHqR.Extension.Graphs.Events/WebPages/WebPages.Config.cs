using ImpostorHqR.Extension.Api.Interface;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    public class WebPageConfig : IConfigurationHolder
    {
        public static WebPageConfig Instance;

        public bool EnableChatPage { get; set; }
        public string ChatPageHandle { get; set; }

        public bool EnableEventsPage { get; set; }
        public string EventsPageHandle { get; set; }
        public byte EventsPageWidth { get; set; }
        public ushort EventsPageUpdateIntervalSeconds { get; set; }

        public void SetDefaults()
        {
            this.EnableChatPage = true;
            this.ChatPageHandle = "playerchat";

            this.EnableEventsPage = true;
            this.EventsPageHandle = "impostorevents";
            this.EventsPageWidth = 100;
            this.EventsPageUpdateIntervalSeconds = 1;
        }
    }
}
