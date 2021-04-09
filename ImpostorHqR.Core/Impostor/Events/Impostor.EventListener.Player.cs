using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using System;

namespace ImpostorHqR.Core.Impostor.Events
{
    class ImpostorPlayerEventListener : IEventListener
    {
        public static ImpostorPlayerEventListener Instance;

        public ImpostorPlayerEventListener() => Instance = this;

        #region Player Events

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent evt) => PlayerSpawned?.Invoke(evt);

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent evt) => PlayerChat?.Invoke(evt);

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent evt) => PlayerDestroyed?.Invoke(evt);

        [EventListener]
        public void OnPlayerCompletedTask(IPlayerCompletedTaskEvent evt) => PlayerCompletedTask?.Invoke(evt);

        [EventListener]
        public void OnPlayerExiled(IPlayerExileEvent evt) => PlayerExile?.Invoke(evt);

        [EventListener]
        public void OnPlayerMurdered(IPlayerMurderEvent evt) => PlayerMurder?.Invoke(evt);

        [EventListener]
        public void OnPlayerSetStartCounter(IPlayerSetStartCounterEvent evt) => PlayerSetStartCounter?.Invoke(evt);

        [EventListener]
        public void OnPlayerStartMeeting(IPlayerStartMeetingEvent evt) => PlayerStartMeeting?.Invoke(evt);


        public event Action<IPlayerSpawnedEvent> PlayerSpawned;
        public event Action<IPlayerChatEvent> PlayerChat;
        public event Action<IPlayerDestroyedEvent> PlayerDestroyed;
        public event Action<IPlayerCompletedTaskEvent> PlayerCompletedTask;
        public event Action<IPlayerExileEvent> PlayerExile;
        public event Action<IPlayerMurderEvent> PlayerMurder;
        public event Action<IPlayerSetStartCounterEvent> PlayerSetStartCounter;
        public event Action<IPlayerStartMeetingEvent> PlayerStartMeeting;

        #endregion
    }
}
