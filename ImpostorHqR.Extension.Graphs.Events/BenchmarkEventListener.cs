using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using System.Threading;
using System.Timers;
using ImpostorHqR.Extension.Graphs.Events.WebPages;

namespace ImpostorHqR.Extension.Graphs.Events
{
    public class BenchmarkEventListener : IEventListener
    {
        public static readonly BenchmarkEventListener Instance = new BenchmarkEventListener();

        public static readonly object syncRoot = new object();

        public BenchmarkEventListener()
        {
            var tmr = new System.Timers.Timer(1000);
            tmr.AutoReset = true;
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            lock (syncRoot)
            {
                #region Assign

                this.OnGameAlterRate = this.OnGameAlterCount;
                this.OnGameCreatedRate = this.OnGameCreatedCount;
                this.OnGameDestroyedRate = this.OnGameDestroyedCount;
                this.OnGameEndedRate = this.OnGameEndedCount;
                this.OnGamePlayerJoinedRate = this.OnGamePlayerJoinedCount;
                this.OnGamePlayerLeftRate = this.OnGamePlayerLeftCount;
                this.OnGameStartedRate = this.OnGameStartedCount;
                this.OnGameStartingRate = this.OnGameStartingCount;
                this.OnPlayerChatRate = this.OnPlayerChatCount;
                this.OnPlayerCompletedTaskRate = this.OnPlayerCompletedTaskCount;
                this.OnPlayerDestroyedRate = this.OnPlayerDestroyedCount;
                this.OnPlayerExileRate = this.OnPlayerExileCount;
                this.OnPlayerMovementRate = this.OnPlayerMovementCount;
                this.OnPlayerMurderRate = this.OnPlayerMurderCount;
                this.OnPlayerSetStartCounterRate = this.OnPlayerSetStartCounterCount;
                this.OnPlayerSpawnedRate = this.OnPlayerSpawnedCount;
                this.OnPlayerStartMeetingRate = this.OnPlayerStartMeetingCount;
                this.OnPlayerVentRate = this.OnPlayerVentCount;
                this.OnMeetingEndedRate = this.OnMeetingEndedCount;
                this.OnMeetingStartedRate = this.OnMeetingStartedCount;

                #endregion

                #region Reset

                this.OnGameAlterCount = 0;
                this.OnGameCreatedCount = 0;
                this.OnGameDestroyedCount = 0;
                this.OnGameEndedCount = 0;
                this.OnGamePlayerJoinedCount = 0;
                this.OnGamePlayerLeftCount = 0;
                this.OnGameStartedCount = 0;
                this.OnGameStartingCount = 0;
                this.OnPlayerChatCount = 0;
                this.OnPlayerCompletedTaskCount = 0;
                this.OnPlayerDestroyedCount = 0;
                this.OnPlayerExileCount = 0;
                this.OnPlayerMovementCount = 0;
                this.OnPlayerMurderCount = 0;
                this.OnPlayerSetStartCounterCount = 0;
                this.OnPlayerSpawnedCount = 0;
                this.OnPlayerStartMeetingCount = 0;
                this.OnPlayerVentCount = 0;
                this.OnMeetingEndedCount = 0;
                this.OnMeetingStartedCount = 0;

                #endregion
            }
        }

        #region Accumulators

        private int OnGameAlterCount = 0;
        private int OnGameCreatedCount = 0;
        private int OnGameDestroyedCount = 0;
        private int OnGameEndedCount = 0;
        private int OnGamePlayerJoinedCount = 0;
        private int OnGamePlayerLeftCount = 0;
        private int OnGameStartedCount = 0;
        private int OnGameStartingCount = 0;
        private int OnPlayerChatCount = 0;
        private int OnPlayerCompletedTaskCount = 0;
        private int OnPlayerDestroyedCount = 0;
        private int OnPlayerExileCount = 0;
        private int OnPlayerMovementCount = 0;
        private int OnPlayerMurderCount = 0;
        private int OnPlayerSetStartCounterCount = 0;
        private int OnPlayerSpawnedCount = 0;
        private int OnPlayerStartMeetingCount = 0;
        private int OnPlayerVentCount = 0;
        private int OnMeetingEndedCount = 0;
        private int OnMeetingStartedCount = 0;

        #endregion

        #region Exposers

        public int OnGameAlterRate { get; private set; }
        public int OnGameCreatedRate { get; private set; }
        public int OnGameDestroyedRate { get; private set; }
        public int OnGameEndedRate { get; private set; }
        public int OnGamePlayerJoinedRate { get; private set; }
        public int OnGamePlayerLeftRate { get; private set; }
        public int OnGameStartedRate { get; private set; }
        public int OnGameStartingRate { get; private set; }
        public int OnPlayerChatRate { get; private set; }
        public int OnPlayerCompletedTaskRate { get; private set; }
        public int OnPlayerDestroyedRate { get; private set; }
        public int OnPlayerExileRate { get; private set; }
        public int OnPlayerMovementRate { get; private set; }
        public int OnPlayerMurderRate { get; private set; }
        public int OnPlayerSetStartCounterRate { get; private set; }
        public int OnPlayerSpawnedRate { get; private set; }
        public int OnPlayerStartMeetingRate { get; private set; }
        public int OnPlayerVentRate { get; private set; }
        public int OnMeetingEndedRate { get; private set; }
        public int OnMeetingStartedRate { get; private set; }

        #endregion

        #region Increments

        [EventListener]
        public void OnGameAlter(IGameAlterEvent evt)
        {
            Interlocked.Increment(ref OnGameAlterCount);
        }

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent evt)
        {
            Interlocked.Increment(ref OnGameCreatedCount);
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent evt)
        {
            Interlocked.Increment(ref OnGameDestroyedCount);
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent evt)
        {
            Interlocked.Increment(ref OnGameEndedCount);
        }

        [EventListener]
        public void OnGamePlayerJoined(IGamePlayerJoinedEvent evt)
        {
            Interlocked.Increment(ref OnGamePlayerJoinedCount);
        }

        [EventListener]
        public void OnGamePlayerLeft(IGamePlayerLeftEvent evt)
        {
            Interlocked.Increment(ref OnGamePlayerLeftCount);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent evt)
        {
            Interlocked.Increment(ref OnGameStartedCount);
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent evt)
        {
            Interlocked.Increment(ref OnGameStartingCount);
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent evt)
        {
            ConsoleWebPage.Updates.Writer.TryWrite($"[{evt.Game.Code.Code}] {evt.ClientPlayer.Character?.PlayerInfo.PlayerName}: \"{evt.Message}\"");
            Interlocked.Increment(ref OnPlayerChatCount);
        }

        [EventListener]
        public void OnPlayerCompletedTask(IPlayerCompletedTaskEvent evt)
        {
            Interlocked.Increment(ref OnPlayerCompletedTaskCount);
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent evt)
        {
            Interlocked.Increment(ref OnPlayerDestroyedCount);
        }

        [EventListener]
        public void OnPlayerExile(IPlayerExileEvent evt)
        {
            Interlocked.Increment(ref OnPlayerExileCount);
        }

        [EventListener]
        public void OnPlayerMovement(IPlayerMovementEvent evt)
        {
            Interlocked.Increment(ref OnPlayerMovementCount);
        }

        [EventListener]
        public void OnPlayerMurder(IPlayerMurderEvent evt)
        {
            Interlocked.Increment(ref OnPlayerMurderCount);
        }

        [EventListener]
        public void OnPlayerSetStartCounter(IPlayerSetStartCounterEvent evt)
        {
            Interlocked.Increment(ref OnPlayerSetStartCounterCount);
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent evt)
        {
            Interlocked.Increment(ref OnPlayerSpawnedCount);
        }

        [EventListener]
        public void OnPlayerStartMeeting(IPlayerStartMeetingEvent evt)
        {
            Interlocked.Increment(ref OnPlayerStartMeetingCount);
        }

        [EventListener]
        public void OnPlayerVent(IPlayerVentEvent evt)
        {
            Interlocked.Increment(ref OnPlayerVentCount);
        }

        [EventListener]
        public void OnMeetingEnded(IMeetingEndedEvent evt)
        {
            Interlocked.Increment(ref OnMeetingEndedCount);
        }

        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent evt)
        {
            Interlocked.Increment(ref OnMeetingStartedCount);
        }

        #endregion
    }
}
