using Impostor.Api.Events.Managers;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages;
using ImpostorHqR.Extensions.Api.Interface.Impostor;
using ImpostorHqR.Extensions.Api.Interface.Logging;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Core.Impostor
{
    public class ImpostorObjectHolder : IImpostorObjectHolder
    {
        public static readonly ImpostorObjectHolder Instance = new ImpostorObjectHolder();

        public ILogger<ILogEmitter> ImpostorLogger { get; set; }

        public IEventManager ImpostorEventManager { get; set; }

        public IGameManager ImpostorGameManager { get; set; }

        public IClientManager ImpostorClientManager { get; set; }

        public IMessageWriterProvider ImpostorMessageWriterProvider { get; set; }
    }
}
