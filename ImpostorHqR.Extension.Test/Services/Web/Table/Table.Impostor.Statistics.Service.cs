using System;
using System.Drawing;
using System.Linq;
using ImpostorHq.Extension.Api.Interface.Web;
using ImpostorHq.Extension.Api.Interface.Web.Page.NoApi;
using ImpostorHq.Extension.Test.Impostor;

namespace ImpostorHq.Extension.Test.Services.Web.Table
{
    internal class ImpostorStatisticsWebService : ISimpleWebService
    {
        public ushort UpdateInterval => 1000;

        private static readonly Random Random = new Random();

        public ISimplePage Page { get; private set; }

        public (string, ISimplePage) Register()
        {
            return ("impostor", Page = Proxy.Instance.PreInitialization.PageProvider.SimplePageProvider.ProduceTablePage(Color.Azure, "Impostor Statistics"));
        }

        public void Update()
        {
            lock (Page)
            {
                // (byte) color < 27 || (byte) color > 167
                Page.Clear();
                Page.AddEntry($"It is {DateTime.Now}.");
                Page.AddEntry("Players: " + Proxy.Instance.Impostor.ImpostorClientManager.Clients.Count());
                Page.AddEntry("Games: " + Proxy.Instance.Impostor.ImpostorGameManager.Games.Count());
                Page.AddEntry($"Chat messages: {ImpostorEventListener.Instance.ChatMessagesPerSecond} / second.");
                Page.AddEntry($"Deaths: {ImpostorEventListener.Instance.DeathsPerSecond} / second.");
                Page.AddEntry($"Movement packets: {ImpostorEventListener.Instance.MovementsPerSecond} / second.");
                Page.AddEntry($"Impostors venting: {ImpostorEventListener.Instance.VentsPerSecond} / second.");
                if (Random.Next(0, 500) == 250) { Page.AddEntry("AeonLucid: Is Bad."); }
            }
        }
    }
}
