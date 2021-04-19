using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    public class ConsoleWebPage
    {
        public static readonly Channel<string> Updates = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
            {SingleReader = true, SingleWriter = false});

        private IReadonlyConsolePage Page { get; set; }

        private bool Cancel = false;

        private readonly CancellationTokenSource Cts = new CancellationTokenSource();

        public static void Create()
        {
            new ConsoleWebPage();
        }

        public ConsoleWebPage()
        {
            var cfg = Start.GetConfig();
            if (!cfg.EnableEventsPage) return;
            Trace.Assert(!string.IsNullOrEmpty(cfg.ChatPageHandle), "Chat page handle cannot be empty!");
            Start.OnClosed += Shutdown;
            if(!cfg.ChatPageRequiresAuthentication) this.Page = IReadonlyConsolePage.Create("Server-Wide Chat", Color.Lime, "Game chat will be displayed here.", Start.GetConfig().ChatPageHandle);
            else
            {
                Trace.Assert(!string.IsNullOrEmpty(cfg.ChatPageUser), "Chat page user cannot be empty!");
                Trace.Assert(!string.IsNullOrEmpty(cfg.ChatPagePassword), "Chat page password cannot be empty!");

                this.Page = IReadonlyConsolePage.Create("Server-Wide Chat", Color.Lime, "Game chat will be displayed here.", cfg.ChatPageHandle, new WebPageAuthenticationOption(cfg.ChatPageUser, cfg.ChatPagePassword));
            }
            _ = QueueAndSend();
        }

        private async Task QueueAndSend()
        {
            try
            {
                var sb = new StringBuilder();
                var locks = new SemaphoreSlim(1,1);
                var enumerable = Updates.Reader.ReadAllAsync(Cts.Token);
                _ = Task.Run(async () =>
                {
                    //dequeue and buffer
                    await foreach (var message in enumerable)
                    {
                        await locks.WaitAsync(Cts.Token);
                        sb.AppendLine(message);
                        locks.Release();
                    }
                });

                while (!Cts.IsCancellationRequested)
                {
                    await locks.WaitAsync(Cts.Token);
                    if (sb.Length > 0)
                    {
                        Page.Push(sb.ToString());
                        sb.Clear();
                    }
                    locks.Release();
                    await Task.Delay(500);
                }
            }
            catch (Exception e)
            {
                ILogManager.Log("FATAL ERROR IN PLAYER CHAT UPDATER!", this.ToString(), LogType.Error, ex:e);
            }
            // instead of pushing an update for each message (which can result in massive PPS), 
            // aggregate more messages per update.

        }

        public void Shutdown()
        {
            Cancel = true;
            Cts.Cancel();
        }
    }
}
