using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using ImpostorHqR.Extension.Api.Interface;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    internal class WebPageLoad : IExtensionService
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

        #endregion
        private List<IGraph> Graphs { get; set; }

        private IGraphPage Page { get; set; }

        private long LastAmountOfJobs { get; set; }

        private volatile int _exceptionsLastTick = 0;

        public void Init()
        {
            if (!WebPageConfig.Instance.EnableLoadPage) return;

            AppDomain.CurrentDomain.FirstChanceException += OnException;

            GenerateGraphs();

            this.Page = Proxy.Instance.PreInitialization.PageProvider.GraphPageProvider.Create(Graphs.ToArray(), "System Load Information", WebPageConfig.Instance.LoadPageHandle, WebPageConfig.Instance.LoadPageWidth);

            var tmr = new System.Timers.Timer(WebPageConfig.Instance.LoadPageIntervalSeconds * 1000) { AutoReset = true };
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

            if (LoadWebPageConfig.Instance.EnableCpuGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("CPU Usage %", Color.DarkBlue, Color.Crimson, 30000, 2500);
                this.Graphs.Add(graph);
                this.CpuGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableMemoryGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Memory Usage (MB)", Color.Crimson, Color.DarkBlue, 30000, 2500);
                this.Graphs.Add(graph);
                this.MemoryGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableTotalThreadPoolThreadsGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Thread Pool Threads", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.TotalThreadPoolThreadsGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableTotalApplicationThreadsGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Total Application Threads", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.TotalApplicationThreadsGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableThreadPoolCompletionPortsGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Thread Pool Completion Ports", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.ThreadPoolCompletionPortsGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableThreadPoolJobsCompletedPerSecondGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Thread Pool Completed Jobs / second", Color.DarkMagenta, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.ThreadPoolJobsCompletedPerSecondGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableThreadPoolJobsQueuedGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Thread Pool Pending Work Item Count (diagnose starvation)", Color.DarkMagenta, Color.Red,
                        15000, 2500);
                this.Graphs.Add(graph);
                this.ThreadPoolJobsQueuedGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableTotalProcessHandlesGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Total Process Handles", Color.Aqua, Color.Red, 15000, 2500);
                this.Graphs.Add(graph);
                this.TotalProcessHandlesGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableHttpBusyThreadsGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Active HTTP Threads", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpBusyThreadsGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableHttpFileTransfersGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Concurrent HTTP File Transfers", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpFileTransfersGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableHttpRequestsPerSecondGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("HTTP Requests / second", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpRequestsPerSecondGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableHttpFileTransferRateKbPsGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("HTTP File Transfer KB / second", Color.Yellow, Color.DarkCyan, 25000, 2500);
                this.Graphs.Add(graph);
                this.HttpFileTransferRateKbPsGraph = graph;
            }

            if (LoadWebPageConfig.Instance.EnableExceptionGraph)
            {
                var graph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                    .Create("Exceptions Per Second", Color.Red, Color.Green, 10000, 2500);
                this.Graphs.Add(graph);
                this.ExceptionGraph = graph;
            }

            if (Graphs.Count != 0) return;
            Proxy.Instance.PreInitialization.ConsoleLogger.LogError("Load page fatal: all graphs are disabled. There must be at least one active graph. Please see the config file!", this, true);
            Environment.Exit(-1);
        }

        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CpuGraph?.Update(Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent);
            MemoryGraph?.Update(Proxy.Instance.PreInitialization.LoadMonitor.MemoryUsageMb);
            TotalThreadPoolThreadsGraph?.Update(ThreadPool.ThreadCount);
            TotalApplicationThreadsGraph?.Update(CurrentProcess.Threads.Count);
            ThreadPool.GetAvailableThreads(out _, out var ports);
            ThreadPoolCompletionPortsGraph?.Update(ports);
            ThreadPoolJobsCompletedPerSecondGraph?.Update(ThreadPool.CompletedWorkItemCount - LastAmountOfJobs);
            ThreadPoolJobsQueuedGraph?.Update(ThreadPool.PendingWorkItemCount);
            LastAmountOfJobs = ThreadPool.CompletedWorkItemCount;
            TotalProcessHandlesGraph?.Update(CurrentProcess.HandleCount);
            HttpBusyThreadsGraph?.Update(Proxy.Instance.PreInitialization.LoadMonitor.HttpServerMonitor.GetActiveThreads());
            HttpFileTransfersGraph?.Update(Proxy.Instance.PreInitialization.LoadMonitor.HttpServerMonitor.GetConcurrentDownloads());
            HttpRequestsPerSecondGraph?.Update(Proxy.Instance.PreInitialization.LoadMonitor.HttpServerMonitor.RequestsPerSecond);
            HttpFileTransferRateKbPsGraph?.Update(Proxy.Instance.PreInitialization.LoadMonitor.HttpServerMonitor.FileDataRateKbPerSecond);
            ExceptionGraph?.Update(_exceptionsLastTick);
            _exceptionsLastTick = 0;
            CurrentProcess.Refresh();
            Page.Update();
        }

        public void PostInit() { }

        public void Shutdown() { }
    }
}
