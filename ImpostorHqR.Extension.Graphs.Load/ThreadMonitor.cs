using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ThreadState = System.Diagnostics.ThreadState;

namespace ImpostorHqR.Extension.Graphs.Load
{
    public class ThreadMonitor
    {
        public static readonly ThreadMonitor Instance = new ThreadMonitor();

        public Dictionary<int, int> Results = new Dictionary<int, int>();

        public ThreadMonitor()
        {
            var t = new Thread(Callback);
            t.Start();
        }

        private void Callback()
        {
            List<ThreadItem> records = new List<ThreadItem>();
            while (true)
            {
                foreach (ProcessThread processThread in Process.GetCurrentProcess().Threads)
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
                    }
                }

                Thread.Sleep(500);

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
                                Results.Add(threadItem.Thread.Id, usage);
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
