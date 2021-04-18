using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace ImpostorHqR.Extension.Api.Utils.Chat
{
    public static class ChatWriter
    {
        /// <summary>
        /// Used to send a chat message to a specific player.
        /// </summary>
        public static async ValueTask WriteChatTo(IClientPlayer player, string sourceName, string[] messages)
        {
            if (player == null)
            {
                ILogManager.Log("null player.", "ChatWriter.WriteChatTo", LogType.Error);
                return;
            }

            if (player.Client.Connection == null)
            {
                ILogManager.Log("null player connection.", "ChatWriter.WriteChatTo", LogType.Error);

                return;
            }

            var packet = PacketGenerator.WriteChat(player, messages, sourceName);
            await player.Client.Connection.SendAsync(packet);
        }

        /// <summary>
        /// Used to send a chat message to a lobby.
        /// </summary>
        public static async ValueTask WriteBroadcast(IGame game, string sourceName, string[] messages, Color color)
        {
            Trace.Assert(game != null, "WriteBroadcast was called with a null IGame. Extension developer error!");
            var packet = PacketGenerator.WriteChat(game.Host, messages, $"[{GenerateColor(color)}]{sourceName}");

            foreach (var clientPlayer in game.Players)
            {
                var task = clientPlayer?.Client?.Connection?.SendAsync(packet);
                if (task != null) await task.Value;
            }
        }

        private static string GenerateColor(Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }
    }
}
