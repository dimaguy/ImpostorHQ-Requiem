using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    internal class WebPageLoad
    {
        private static readonly Process CurrentProcess = System.Diagnostics.Process.GetCurrentProcess();

        #region Graphs

        private IGraph CpuGraph { get; set; }
        private IGraph MemoryGraph { get; set; }
        private IGraph TotalThreadPoolThreadsGraph { get; set; }
        private IGraph TotalApplicationThreadsGraph { get; set; }
        private IGraph ThreadPoolCompletionPortsGraph { get; set; }
        private IGraph ThreadPoolJobsCompletedPerSecondGraph { get; set; }
        private IGraph ThreadPoolJobsQueuedGraph { get; set; }
        private IGraph TotalProcessHandlesGraph { get; set; }
        private IGraph HttpBusyThreadsGraph { get; set; }
        private IGraph HttpFileTransfersGraph { get; set; }
        private IGraph HttpRequestsPerSecondGraph { get; set; }
        private IGraph HttpFileTransferRateKbPsGraph { get; set; }
        private IGraph ExceptionGraph { get; set; }
        private IGraph CacheGraph { get; set; }

        #endregion

        private List<IGraph> Graphs { get; set; }

        private IGraphPage Page { get; set; }

        private long LastAmountOfJobs { get; set; }

        private volatile int _exceptionsLastTick = 0;

        public void Start()
        {
            var cfg = IConfigurationStore.GetByType<WebPageConfig>();
            if (!cfg.EnableLoadPage) return;
            Trace.Assert(!string.IsNullOrEmpty(cfg.LoadPageHandle), "Load page handle cannot be empty!");

            AppDomain.CurrentDomain.FirstChanceException += OnException;

            GenerateGraphs();
            if (!cfg.LoadPageRequiresAuthentication)
            {
                this.Page = IGraphPage.Create(Graphs.ToArray(), "System Load Information",
                    cfg.LoadPageHandle,
                    cfg.LoadPageWidth);
            }
            else
            {
                Trace.Assert(!string.IsNullOrEmpty(cfg.LoadPagePassword), "Load page password cannot be empty!");
                Trace.Assert(!string.IsNullOrEmpty(cfg.LoadPageUser), "Load page user cannot be empty!");
                this.Page = IGraphPage.Create(Graphs.ToArray(), "System Load Information",
                    cfg.LoadPageHandle,
                    cfg.LoadPageWidth, new WebPageAuthenticationOption(cfg.LoadPageUser, cfg.LoadPagePassword));
            }

            
            var tmr = new System.Timers.Timer(IConfigurationStore.GetByType<WebPageConfig>().LoadPageIntervalSeconds * 1000) { AutoReset = true };
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        private void OnException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Interlocked.Increment(ref _exceptionsLastTick);
        }

        private void GenerateGraphs()
        {
            this.Graphs = new List<IGraph>();
            var cfg = IConfigurationStore.GetByType<LoadWebPageConfig>();
            if (cfg.EnableCpuGraph)
            {
                var graph = IGraph
                    .Create("CPU Usage %", Color.DarkBlue, Color.Crimson, 30000, 2500);
                this.Graphs.Add(graph);
                this.CpuGraph = graph;
            }

            if (cfg.EnableMemoryGraph)
            {
                var graph = IGraph
                    .Create("Memory Usage (MB)", Color.Crimson, Color.DarkBlue, 30000, 2500);
                this.Graphs.Add(graph);
                this.MemoryGraph = graph;
            }

            if (cfg.EnableCacheGraph)
            {
                var graph = IGraph
                    .Create("Http Cache Size (KB)", Color.Cyan, Color.Aquamarine, 50000, 2500);
                this.Graphs.Add(graph);
                this.CacheGraph = graph;
            }

            if (cfg.EnableTotalThreadPoolThreadsGraph)
            {
                var graph = IGraph
                    .Create("Thread Pool Threads", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.TotalThreadPoolThreadsGraph = graph;
            }

            if (cfg.EnableTotalApplicationThreadsGraph)
            {
                var graph = IGraph
                    .Create("Total Application Threads", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.TotalApplicationThreadsGraph = graph;
            }

            if (cfg.EnableThreadPoolCompletionPortsGraph)
            {
                var graph = IGraph
                    .Create("Thread Pool Completion Ports", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.ThreadPoolCompletionPortsGraph = graph;
            }

            if (cfg.EnableThreadPoolJobsCompletedPerSecondGraph)
            {
                var graph = IGraph
                    .Create("Thread Pool Completed Jobs / second", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.ThreadPoolJobsCompletedPerSecondGraph = graph;
            }

            if (cfg.EnableThreadPoolJobsQueuedGraph)
            {
                var graph = IGraph
                    .Create("Thread Pool Pending Work Item Count (diagnose starvation)", Color.DarkMagenta, Color.Red,
                        15000, 2500);
                this.Graphs.Add(graph);
                this.ThreadPoolJobsQueuedGraph = graph;
            }

            if (cfg.EnableTotalProcessHandlesGraph)
            {
                var graph = IGraph
                    .Create("Total Process Handles", Color.Aqua, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.TotalProcessHandlesGraph = graph;
            }

            if (cfg.EnableHttpBusyThreadsGraph)
            {
                var graph = IGraph
                    .Create("Active HTTP Threads", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpBusyThreadsGraph = graph;
            }

            if (cfg.EnableHttpFileTransfersGraph)
            {
                var graph = IGraph
                    .Create("Concurrent HTTP File Transfers", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpFileTransfersGraph = graph;
            }

            if (cfg.EnableHttpRequestsPerSecondGraph)
            {
                var graph = IGraph
                    .Create("HTTP Requests / second", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpRequestsPerSecondGraph = graph;
            }

            if (cfg.EnableHttpFileTransferRateKbPsGraph)
            {
                var graph = IGraph
                    .Create("HTTP File Transfer KB / second", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpFileTransferRateKbPsGraph = graph;
            }

            if (cfg.EnableExceptionGraph)
            {
                var graph = IGraph
                    .Create("Exceptions Per Second", Color.Red, Color.Green, 10000, 2500);
                this.Graphs.Add(graph);
                this.ExceptionGraph = graph;
            }

            if (Graphs.Count != 0) return;
            ILogManager.Log("Load page fatal: all graphs are disabled. There must be at least one active graph. Please see the config file!", this.ToString(), LogType.Error);
            Environment.Exit(-1);
        }

        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CpuGraph?.Update(IServerLoad.CpuUsagePercent);
            MemoryGraph?.Update(IServerLoad.MemoryUsageMb);
            TotalThreadPoolThreadsGraph?.Update(ThreadPool.ThreadCount);
            TotalApplicationThreadsGraph?.Update(CurrentProcess.Threads.Count);
            ThreadPool.GetAvailableThreads(out _, out var ports);
            ThreadPoolCompletionPortsGraph?.Update(ports);
            ThreadPoolJobsCompletedPerSecondGraph?.Update(ThreadPool.CompletedWorkItemCount - LastAmountOfJobs);
            ThreadPoolJobsQueuedGraph?.Update(ThreadPool.PendingWorkItemCount);
            LastAmountOfJobs = ThreadPool.CompletedWorkItemCount;
            TotalProcessHandlesGraph?.Update(CurrentProcess.HandleCount);
            HttpBusyThreadsGraph?.Update(IServerLoad.GetHttpActiveThreads());
            HttpFileTransfersGraph?.Update(IServerLoad.GetHttpActiveThreads());
            HttpRequestsPerSecondGraph?.Update(IServerLoad.GetHttpRequestRate());
            HttpFileTransferRateKbPsGraph?.Update(IServerLoad.GetHttpRateKbPerSecond());
            ExceptionGraph?.Update(_exceptionsLastTick);
            CacheGraph?.Update(IServerLoad.GetCacheSizeKb()/1024);
            _exceptionsLastTick = 0;
            CurrentProcess.Refresh();
            Page.Update();
        }
    }
}
