using ImpostorHqR.Extension.Api.Interface;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    class LoadWebPageConfig : IConfigurationHolder
    {
        public static LoadWebPageConfig Instance { get; set; }

        #region Properties

        public bool EnableCpuGraph { get; set; }
        public bool EnableMemoryGraph { get; set; }
        public bool EnableTotalThreadPoolThreadsGraph { get; set; }
        public bool EnableTotalApplicationThreadsGraph { get; set; }
        public bool EnableThreadPoolCompletionPortsGraph { get; set; }
        public bool EnableThreadPoolJobsCompletedPerSecondGraph { get; set; }
        public bool EnableThreadPoolJobsQueuedGraph { get; set; }
        public bool EnableTotalProcessHandlesGraph { get; set; }
        public bool EnableHttpBusyThreadsGraph { get; set; }
        public bool EnableHttpFileTransfersGraph { get; set; }
        public bool EnableHttpRequestsPerSecondGraph { get; set; }
        public bool EnableHttpFileTransferRateKbPsGraph { get; set; }
        public bool EnableExceptionGraph { get; set; }

        #endregion

        public LoadWebPageConfig()
        {
            Instance = this;
        }

        public void SetDefaults()
        {
            this.EnableCpuGraph = true;
            this.EnableMemoryGraph = true;
            this.EnableTotalThreadPoolThreadsGraph = true;
            this.EnableTotalApplicationThreadsGraph = true;
            this.EnableThreadPoolCompletionPortsGraph = true;
            this.EnableThreadPoolJobsCompletedPerSecondGraph = true;
            this.EnableThreadPoolJobsQueuedGraph = true;
            this.EnableTotalProcessHandlesGraph = true;
            this.EnableHttpBusyThreadsGraph = true;
            this.EnableHttpFileTransfersGraph = true;
            this.EnableHttpRequestsPerSecondGraph = true;
            this.EnableHttpFileTransferRateKbPsGraph = true;
            this.EnableExceptionGraph = true;
        }
    }
}
