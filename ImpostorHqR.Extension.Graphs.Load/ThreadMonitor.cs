using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;
using ImpostorHqR.Extension.Graphs.Load.WebPages;
using ThreadState = System.Diagnostics.ThreadState;

namespace ImpostorHqR.Extension.Graphs.Load
{
    public static class ThreadMonitor
    {
        public static readonly ConcurrentDictionary<int, int> Results = new ConcurrentDictionary<int, int>();

        private static readonly Process CurrentProcess = Process.GetCurrentProcess();

        static ThreadMonitor()
        {
            Trace.Assert(IConfigurationStore.GetByType<WebPageConfig>().EnableThreadPage, "THREAD MONITOR STARTED BY ITSELF!");
            _ = Callback();
        }

        private static async Task Callback()
        {
            var records = new List<ThreadItem>();
            while (true)
            {
                CurrentProcess.Refresh();
                foreach (ProcessThread processThread in CurrentProcess.Threads)
                {
                    try
                    {
                        if (processThread.ThreadState == ThreadState.Terminated ||
                            processThread.ThreadState == ThreadState.Unknown) continue;
                        var time = processThread.TotalProcessorTime;
                        records.Add(new ThreadItem()
                        {
                            StartCpuTime = new TimeSpan(time.Ticks),
                            Thread = processThread,
                            RecordTime = DateTime.Now
                        });
                    }
                    catch
                    {
                        // ignored
                    }
                }

                await Task.Delay(500);

                foreach (var threadItem in records)
                {
                    try
                    {
                        if (threadItem.Thread.ThreadState == ThreadState.Terminated ||
                            threadItem.Thread.ThreadState == ThreadState.Unknown) continue;
                        var endUsage = threadItem.Thread.TotalProcessorTime;
                        var startUsage = threadItem.StartCpuTime;
                        var usage = (int)((endUsage - startUsage).TotalMilliseconds /
                            (Environment.ProcessorCount *
                             ((DateTime.Now - threadItem.RecordTime).TotalMilliseconds)) * 100);
                        lock (Results)
                        {
                            if (Results.ContainsKey(threadItem.Thread.Id))
                            {
                                Results[threadItem.Thread.Id] = usage;
                            }
                            else
                            {
                                Results.TryAdd(threadItem.Thread.Id, usage);
                            }
                        }
                        threadItem.Thread.Dispose();
                    }
                    catch
                    {
                        // invalid operation exception because thread has exited
                    }

                }
                records.Clear();
            }
        }

        private struct ThreadItem
        {
            public ProcessThread Thread { get; set; }

            public TimeSpan StartCpuTime { get; set; }

            public DateTime RecordTime { get; set; }
        }
    }
}
