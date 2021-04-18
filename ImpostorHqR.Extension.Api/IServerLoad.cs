using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImpostorHqR.Extension.Api
{
    public interface IServerLoad
    {
        private static readonly Process ServerProcess = Process.GetCurrentProcess();

        public static int CpuUsagePercent { get; private set; }

        public static int MemoryUsageMb { get; private set; }

        #region Server

        public static Func<int> GetHttpActiveThreads { get; private set; }

        public static Func<int> GetHttpRequestRate { get; private set; }

        public static Func<int> GetHttpRateKbPerSecond { get; private set; }

        public static Func<int> GetCacheSizeKb { get; private set; }

        public static Func<int> GetApiUsersCount { get; private set; }

        #endregion

        static IServerLoad()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var startTime = DateTime.Now;
                    var startUsage = ServerProcess.TotalProcessorTime;

                    await Task.Delay(500);

                    var endTime = DateTime.Now;
                    var endUsage = ServerProcess.TotalProcessorTime;
                    ServerProcess.Refresh();

                    CpuUsagePercent = (int) ((endUsage - startUsage).TotalMilliseconds /
                                             (Environment.ProcessorCount * ((endTime - startTime).TotalMilliseconds)) *
                                             100);
                    MemoryUsageMb = (int) ((ServerProcess.PrivateMemorySize64 / 1024f) / 1024f);
                }

            }, TaskCreationOptions.LongRunning);
        }
    }
}
