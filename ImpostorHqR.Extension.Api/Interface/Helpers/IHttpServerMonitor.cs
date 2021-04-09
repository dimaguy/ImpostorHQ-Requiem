namespace ImpostorHqR.Extension.Api.Interface.Helpers
{
    public interface IHttpServerMonitor
    {
        int GetActiveThreads();
        int GetConcurrentDownloads();
        int RequestsPerSecond { get; }
        int FileDataRateKbPerSecond { get; }
    }
}
