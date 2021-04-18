using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    [Configuration(false, true)]
    public class WebPageConfig
    {
        public bool EnableChatPage { get; set; } = true;
        public string ChatPageHandle { get; set; } = "playerchat";
        public bool EnableEventsPage { get; set; } = true;
        public string EventsPageHandle { get; set; } = "impostorevents";
        public byte EventsPageWidth { get; set; } = 100;
        public ushort EventsPageUpdateIntervalSeconds { get; set; } = 1;
    }
}
