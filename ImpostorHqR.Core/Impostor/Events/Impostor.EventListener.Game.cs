using Impostor.Api.Events;
using System;

namespace ImpostorHqR.Core.Impostor.Events
{
    public class ImpostorGameEventListener : IEventListener
    {
        #region Game Events

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent evt) => GameCreated?.Invoke(evt);

        [EventListener]
        public void OnGameAltered(IGameAlterEvent evt) => GameAltered?.Invoke(evt);

        [EventListener]
        public void OnGamePlayerJoined(IGamePlayerJoinedEvent evt) => GamePlayerJoined?.Invoke(evt);

        [EventListener]
        public void OnGameStarting(IGameStartingEvent evt) => GameStartingEvent?.Invoke(evt);

        [EventListener]
        public void OnGameStarted(IGameStartedEvent evt) => GameStarted?.Invoke(evt);

        [EventListener]
        public void OnGameEnded(IGameEndedEvent evt) => GameEnded?.Invoke(evt);

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent evt) => GameDestroyed?.Invoke(evt);


        public event Action<IGameCreatedEvent> GameCreated;
        public event Action<IGameAlterEvent> GameAltered;
        public event Action<IGamePlayerJoinedEvent> GamePlayerJoined;
        public event Action<IGameStartingEvent> GameStartingEvent;
        public event Action<IGameStartedEvent> GameStarted;
        public event Action<IGameEndedEvent> GameEnded;
        public event Action<IGameDestroyedEvent> GameDestroyed;

        #endregion
    }
}
