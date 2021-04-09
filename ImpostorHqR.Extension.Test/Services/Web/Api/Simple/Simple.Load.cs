using System;
using System.Drawing;
using ImpostorHq.Extension.Api.Interface.Web;
using ImpostorHq.Extension.Api.Interface.Web.Page.Api.Simple;

namespace ImpostorHq.Extension.Test.Services.Web.Api.Simple
{
    public class LoadApiWebService : ISimpleApiWebService
    {
        public ISimpleApiPage Register()
        {
            var page = Proxy.Instance.PreInitialization.PageProvider.SimpleApiPageProvider.ProduceApiPage(
                "Load in real time", Color.White, "loadRealTime");

            var updateTimer = new System.Timers.Timer(500);
            updateTimer.AutoReset = true;
            updateTimer.Elapsed += (sender, args) =>
            {
                var stats = GetLatest();
                page.Set(stats.Message, stats.BackgroundColor);
            };
            updateTimer.Start();

            return page;
        }

        private ISimpleApiPageElement GetLatest()
        {
            var message = $"It is {DateTime.Now}\nCPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%\nMemory Usage: {Proxy.Instance.PreInitialization.LoadMonitor.MemoryUsageMb} MB";
            return Proxy.Instance.PreInitialization.PageProvider.SimpleApiPageElementProvider.Create(message, Color.Black);
        }
    }
}
