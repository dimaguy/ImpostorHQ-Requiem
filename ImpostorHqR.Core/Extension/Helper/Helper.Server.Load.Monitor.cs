using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Web.Http.Server.Client;
using ImpostorHqR.Extension.Api.Interface.Helpers;

namespace ImpostorHqR.Core.Extension.Helper
{
    public class ServerLoadMonitor : IServerLoadMonitor
    {
        public static readonly ServerLoadMonitor Instance = new ServerLoadMonitor();

        private static readonly Process ServerProcess = Process.GetCurrentProcess();

        public int CpuUsagePercent { get; private set; }

        public int MemoryUsageMb { get; private set; }

        public IHttpServerMonitor HttpServerMonitor => HttpClientWorkPool.Instance;

        public ServerLoadMonitor()
        {
            new Thread(async () =>
            {
                while (true)
                {
                    var startTime = DateTime.Now;
                    var startUsage = ServerProcess.TotalProcessorTime;
                    await Task.Delay(500);
                    var endTime = DateTime.Now;
                    var endUsage = ServerProcess.TotalProcessorTime;
                    ServerProcess.Refresh();
                    this.CpuUsagePercent = (int)((endUsage - startUsage).TotalMilliseconds / (Environment.ProcessorCount * ((endTime - startTime).TotalMilliseconds)) * 100);
                    this.MemoryUsageMb = (int)((ServerProcess.PrivateMemorySize64 / 1024f) / 1024f);
                }
            }).Start();
        }
    }
}