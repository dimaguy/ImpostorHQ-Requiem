using Impostor.Api.Events.Managers;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Extension.Api.Registry
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once IdentifierTypo
    public interface Impostor
    {
        public static ILogger<IDimaCode> Logger;

        public static IEventManager EventManager;

        public static IGameManager GameManager;

        public static IClientManager ClientManager;

        public static IMessageWriterProvider MessageWriterProvider;
    }
}
