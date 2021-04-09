using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace ImpostorHqR.Core.Impostor.Chat
{
    public static partial class PacketGenerator
    {
        /// <summary>
        /// Used to write a chat packet. Send directly to the player.
        /// </summary>
        public static IMessageWriter WriteChat(IClientPlayer player, string[] chat, string source)
        {
            var game = player.Game.Code;
            var netId = player.Character.NetId;
            var originalName = player.Character.PlayerInfo.PlayerName;

            var messageWriter = ImpostorObjectHolder.Instance.ImpostorMessageWriterProvider.Get(MessageType.Reliable);
            messageWriter.StartMessage(MessageFlags.GameData);
            messageWriter.Write(game);
            messageWriter.StartMessage(ChatPacketGeneratorConstant.GameDataRpcFlag);
            messageWriter.WritePacked(netId);
            messageWriter.Write(ChatPacketGeneratorConstant.RpcSetName);
            messageWriter.Write(source);
            messageWriter.EndMessage();
            foreach (var message in chat)
            {
                messageWriter.StartMessage(ChatPacketGeneratorConstant.GameDataRpcFlag);
                messageWriter.WritePacked(netId);
                messageWriter.Write(ChatPacketGeneratorConstant.RpcSendChat);
                messageWriter.Write(message);
                messageWriter.EndMessage();
            }
            messageWriter.StartMessage(ChatPacketGeneratorConstant.GameDataRpcFlag);
            messageWriter.WritePacked(netId);
            messageWriter.Write(ChatPacketGeneratorConstant.RpcSetName);
            messageWriter.Write(originalName);
            messageWriter.EndMessage();
            messageWriter.EndMessage();
            return messageWriter;
        }
    }
}
