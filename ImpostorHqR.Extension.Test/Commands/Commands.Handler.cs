using Impostor.Api.Net;
using System.Drawing;
using System.Linq;

namespace ImpostorHq.Extension.Test.Commands
{
    public static class CommandHandler
    {
        public static async void HandleLoadCommand(IClientPlayer sender, string[] data)
        {
            if (sender == null) return;

            var message = Proxy.Instance.PreInitialization.ChatUtils.MessageProvider.Create("(server status)", new string[]
            {
                "| Server load status |",
                $"CPU Usage: {Proxy.Instance.PreInitialization.LoadMonitor.CpuUsagePercent}%",
                $"Memory usage: {Proxy.Instance.PreInitialization.LoadMonitor.MemoryUsageMb} MB",
                $"Players: {Proxy.Instance.Impostor.ImpostorClientManager.Clients.Count()}",
                $"Lobbies: {Proxy.Instance.Impostor.ImpostorGameManager.Games.Count()}"
            }, Color.Cyan);
            await Proxy.Instance.PreInitialization.ChatUtils.MessageWriter.WriteChatTo(sender, message);
        }

        public static void HandleTestCommand(IClientPlayer sender, string[] data)
        {
            sender?.Character?.SendChatAsync("test.");
        }
    }
}
