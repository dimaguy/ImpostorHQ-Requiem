using System;
using System.Drawing;
using ImpostorHq.Extension.Api.Interface.Web;
using ImpostorHq.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHq.Extension.Test.Services.Web.Table
{
    internal class ServerLoadWebService : ISimpleWebService
    {
        private const string InitialTitle = "Server Load";

        public ushort UpdateInterval => 1000;

        public ISimplePage Page { get; private set; }

        public (string, ISimplePage) Register()
        {
            return ("load", Page = Proxy.Instance.PreInitialization.PageProvider.SimplePageProvider.ProduceTablePage(Color.Green, InitialTitle));
        }

        public void Update()
        {
            lock (Page)
            {
                Page.Clear();
                Page.AddEntry($"It is {DateTime.Now}.");

                if (Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent < 15)
                {
                    var color = Color.Lime;
                    Page.AddEntry(Proxy.Instance.PreInitialization.PageProvider.SimplePageAttributeHelper.SetColorAttribute($"CPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%", color));
                }
                else if (Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent < 25)
                {
                    var color = Color.Green;
                    Page.AddEntry(Proxy.Instance.PreInitialization.PageProvider.SimplePageAttributeHelper.SetColorAttribute($"CPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%", color));
                }
                else if (Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent < 50)
                {
                    var color = Color.Yellow;
                    Page.AddEntry(Proxy.Instance.PreInitialization.PageProvider.SimplePageAttributeHelper.SetColorAttribute($"CPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%", color));
                }
                else if (Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent < 75)
                {
                    var color = Color.Orange;
                    Page.AddEntry(Proxy.Instance.PreInitialization.PageProvider.SimplePageAttributeHelper.SetColorAttribute($"CPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%", color));
                }
                else if (Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent < 90)
                {
                    var color = Color.OrangeRed;
                    Page.AddEntry(Proxy.Instance.PreInitialization.PageProvider.SimplePageAttributeHelper.SetColorAttribute($"CPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%", color));
                }
                else
                {
                    var color = Color.Red;
                    Page.AddEntry(Proxy.Instance.PreInitialization.PageProvider.SimplePageAttributeHelper.SetColorAttribute($"CPU Usage - critical: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%", color));
                    Page.SetParameters(color, InitialTitle.ToUpper() + " - CPU OVERLOAD");
                }

                Page.AddEntry($"Memory Usage: {Proxy.Instance.PreInitialization.LoadMonitor.MemoryUsageMb} MB");
            }
        }

    }
}
