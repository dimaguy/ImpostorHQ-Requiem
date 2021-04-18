using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    class WebPageThreads
    {
        private IApiPage Page { get; set; }

        private string Indent = "\t\t\t\t";

        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Page.Set(GetRecords(), Color.FromArgb(0, 21, 21, 21));
        }

        private string GetRecords()
        {
            using var sb = IReusableStringBuilder.Get();
            sb.AppendLine($"It is {DateTime.Now}<br>");
            
            foreach (var instanceResult in ThreadMonitor.Results.OrderByDescending(i => i.Value))
            {
                sb.Append($"Thread ID: {instanceResult.Key} : {Indent}{instanceResult.Value}%<br>");
            }
            
            var result = sb.ToString();
            return result;
        }

        public void Start()
        {
            var cfg = IConfigurationStore.GetByType<WebPageConfig>();
            if (!cfg.EnableThreadPage) return;
            Trace.Assert(!string.IsNullOrEmpty(cfg.ThreadPageHandle), "Thread page handle cannot be empty!");

            if (!cfg.ThreadPageRequiresAuthentication)
            {
                this.Page = IApiPage.Create("Thread CPU Usage", Color.Aqua,
                    cfg.ThreadPageHandle);
            }
            else
            {
                Trace.Assert(!string.IsNullOrEmpty(cfg.ThreadPagePassword), "Thread page password cannot be empty!");
                Trace.Assert(!string.IsNullOrEmpty(cfg.ThreadPageUser), "Thread page user cannot be empty!");
                this.Page = IApiPage.Create("Thread CPU Usage", Color.Aqua,
                    cfg.ThreadPageHandle, new WebPageAuthenticationOption(cfg.ThreadPageUser, cfg.ThreadPagePassword));
            }

            var tmr = new System.Timers.Timer(1000) { AutoReset = true };
            tmr.Elapsed += Tick;
            tmr.Start();
        }
    }
}
