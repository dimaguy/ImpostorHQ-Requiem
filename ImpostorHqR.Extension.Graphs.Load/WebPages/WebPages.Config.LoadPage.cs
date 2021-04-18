using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    [Configuration(false, true)]
    class LoadWebPageConfig
    {
        #region Properties
                                                        // sorry, i didn't feel like adjusting indentation
        public bool EnableCpuGraph { get; set; }                                            = true;
        public bool EnableMemoryGraph { get; set; }                                         = true;
        public bool EnableTotalThreadPoolThreadsGraph { get; set; }                         = true;
        public bool EnableTotalApplicationThreadsGraph { get; set; }                        = true;
        public bool EnableThreadPoolCompletionPortsGraph { get; set; }                      = true;
        public bool EnableThreadPoolJobsCompletedPerSecondGraph { get; set; }               = true;
        public bool EnableThreadPoolJobsQueuedGraph { get; set; }                           = true;
        public bool EnableTotalProcessHandlesGraph { get; set; }                            = true;
        public bool EnableHttpBusyThreadsGraph { get; set; }                                = true;
        public bool EnableHttpFileTransfersGraph { get; set; }                              = true;
        public bool EnableHttpRequestsPerSecondGraph { get; set; }                          = true;
        public bool EnableHttpFileTransferRateKbPsGraph { get; set; }                       = true;
        public bool EnableExceptionGraph { get; set; }                                      = true;
        public bool EnableCacheGraph { get; set; } = true;

        #endregion
    }
}
