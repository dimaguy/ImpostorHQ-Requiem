using Impostor.Api.Games;
using Impostor.Api.Net;
using System.Drawing;
using System.Threading.Tasks;

namespace ImpostorHqR.Extensions.Api.Interface.Game
{
    public interface IChatUtils
    {
        IChatWriter MessageWriter { get; }

        IChatMessageProvider MessageProvider { get; }
    }

    /// <summary>
    /// This system will help you write chat messages. It is remade here to allow for more control.
    /// </summary>
    public interface IChatWriter
    {
        /// <summary>
        /// Writes chat to a specified ClientPlayer. Only that player can see the message.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        Task WriteChatTo(IClientPlayer player, IChatMessage data);
        /// <summary>
        /// Will write a chat message to a lobby.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task WriteBroadcast(IGame player, IChatMessage data);
    }

    public interface IChatMessage
    {
        /// <summary>
        /// The Color that the sender's name will be set to.
        /// </summary>
        Color Color { get; }
        /// <summary>
        /// The messages to send. If you intend to send more, this is more efficient than splitting them into different operations.
        /// </summary>
        string[] Messages { get; }
        /// <summary>
        /// The name that will appear to have sent the message. Can be anything.
        /// </summary>
        string SourceName { get; }
    }

    public interface IChatMessageProvider
    {
        IChatMessage Create(string sourceName, string[] messages, Color color);
    }
}
