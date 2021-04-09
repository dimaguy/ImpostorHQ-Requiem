using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ImpostorHqR.Extension.Api.Interface;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    public class ConsoleWebPage : IExtensionService
    {
        public static readonly Channel<string> Updates = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
            {SingleReader = true, SingleWriter = false});

        private IReadonlyConsolePage Page { get; set; }

        public void Init()
        {
            if(!WebPageConfig.Instance.EnableEventsPage)return;
            this.Page =  Proxy.Instance.PreInitialization.PageProvider.ReadonlyConsolePageProvider.ProduceApiPage(
                "Server-Wide Chat", Color.Lime, "Game chat will be displayed here.",
                WebPageConfig.Instance.ChatPageHandle);

            Task.Run(async () =>
            {
                await foreach (var message in Updates.Reader.ReadAllAsync())
                {
                    Page.Push(string.Concat(message,"\n"));
                }
            });
        }

        public void PostInit() { }

        public void Shutdown() { }
    }
}
