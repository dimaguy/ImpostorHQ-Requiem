using System;
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
            if (!IConfigurationStore.GetByType<WebPageConfig>().EnableThreadPage) return;

            this.Page = IApiPage.Create("Thread CPU Usage", Color.Aqua, IConfigurationStore.GetByType<WebPageConfig>().ThreadPageHandle);
            var tmr = new System.Timers.Timer(1000) { AutoReset = true };
            tmr.Elapsed += Tick;
            tmr.Start();
        }
    }
}
