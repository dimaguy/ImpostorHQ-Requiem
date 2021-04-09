using Impostor.Api.Events.Managers;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages;
using ImpostorHqR.Extensions.Api.Interface.Logging;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Extensions.Api.Interface.Impostor
{
    public interface IImpostorObjectHolder
    {
        ILogger<ILogEmitter> ImpostorLogger { get; }

        IEventManager ImpostorEventManager { get; }

        IGameManager ImpostorGameManager { get; }

        IClientManager ImpostorClientManager { get; }

        IMessageWriterProvider ImpostorMessageWriterProvider { get; }
    }
}
