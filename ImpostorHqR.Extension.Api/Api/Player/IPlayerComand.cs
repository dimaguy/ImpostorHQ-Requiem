#nullable enable
using System;
using Impostor.Api.Net;

namespace ImpostorHqR.Extension.Api.Api.Player
{
    public interface IPlayerCommand
    {
        #region Privider

        private static Converter<(CommandData data, Action<IClientPlayer, string[]> commandInvoked, Action<IClientPlayer> invalidSyntax), IPlayerCommand> _Provider;

        public static IPlayerCommand Create(CommandData data, Action<IClientPlayer, string[]> commandInvoked,
            Action<IClientPlayer>? invalidSyntax)
        {
            return _Provider((data, commandInvoked, invalidSyntax));
        }

        #endregion

        CommandData Data { get; }

        void Process(IClientPlayer client, string? data);

        string Help { get; }
    }
}