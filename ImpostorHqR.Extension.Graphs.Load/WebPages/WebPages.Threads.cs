using System;
using System.Drawing;
using System.Linq;
using ImpostorHqR.Extension.Api.Interface;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    class WebPageThreads : IExtensionService
    {
        private ISimpleApiPage Page { get; set; }

        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Page.Set(GetRecords(), Color.FromArgb(0, 21, 21, 21));
        }

        private string GetRecords()
        {
            var rsb = Proxy.Instance.PreInitialization.StringBuilderPool.Get();
            var sb = rsb.StringBuilder;
            sb.AppendLine($"It is {DateTime.Now}<br>");
            lock (ThreadMonitor.Instance.Results)
            {
                foreach (var instanceResult in ThreadMonitor.Instance.Results.OrderByDescending(i => i.Value))
                {
                    sb.Append($"Thread ID: {instanceResult.Key} : {GetSpaces(instanceResult.Key)}{instanceResult.Value}%<br>");
                }
            }

            var result = sb.ToString();
            Proxy.Instance.PreInitialization.StringBuilderPool.Return(rsb);

            return result;
        }

        private string GetSpaces(int id)
        {
            return new string(' ', 10 - id.ToString().Length);
        }

        public void Init()
        {
            if (!WebPageConfig.Instance.EnableThreadPage) return;

            this.Page = Proxy.Instance.PreInitialization.PageProvider.SimpleApiPageProvider.ProduceApiPage("Thread CPU Usage", Color.Aqua, WebPageConfig.Instance.ThreadPageHandle);
            var tmr = new System.Timers.Timer(1000) { AutoReset = true };
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        public void PostInit()
        {
        }

        public void Shutdown()
        {
        }
    }
}
