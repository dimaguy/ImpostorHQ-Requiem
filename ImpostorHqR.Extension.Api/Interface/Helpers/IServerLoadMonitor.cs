namespace ImpostorHqR.Extension.Api.Interface.Helpers
{
    public interface IServerLoadMonitor
    {
        public int CpuUsagePercent { get; }

        public int MemoryUsageMb { get; }

        public IHttpServerMonitor HttpServerMonitor { get; }
    }
}
