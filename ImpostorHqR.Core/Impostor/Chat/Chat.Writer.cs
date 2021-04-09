using Impostor.Api.Games;
using Impostor.Api.Net;
using System.Linq;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Impostor.Chat
{
    public class ChatWriter : IChatWriter
    {
        public static readonly IChatWriter Instance = new ChatWriter();

        /// <summary>
        /// Used to send a chat message to a specific player.
        /// </summary>
        public async Task WriteChatTo(IClientPlayer player, IChatMessage data)
        {
            if (player == null)
            {
                ConsoleLogging.Instance.LogError("ChatWriter.WriteChatTo : null player.", null);
                return;
            }

            if (player.Client.Connection == null)
            {
                ConsoleLogging.Instance.LogError("ChatWriter.WriteChatTo : null player connection.", null);
                return;
            }

            var packet = PacketGenerator.WriteChat(player, data.Messages, $"[{((ChatMessage)data).GenerateColor()}]{data.SourceName}");
            await player.Client.Connection.SendAsync(packet);
        }

        /// <summary>
        /// Used to send a chat message to a lobby.
        /// </summary>
        public async Task WriteBroadcast(IGame game, IChatMessage data)
        {
            if (game == null) ConsoleLogging.Instance.LogError("ChatWriter.WriteBroadcast : null game.", null);
            var packet = PacketGenerator.WriteChat(game.Host, data.Messages, $"[{((ChatMessage)data).GenerateColor()}]{data.SourceName}");

            foreach (var clientPlayer in game.Players.ToList().Where(c => c.Client.Connection != null))
            {
                await clientPlayer.Client.Connection.SendAsync(packet);
            }
        }
    }
}
