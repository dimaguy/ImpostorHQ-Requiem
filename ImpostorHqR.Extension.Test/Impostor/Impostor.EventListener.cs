using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using System.Threading;

namespace ImpostorHq.Extension.Test.Impostor
{
    public class ImpostorEventListener : IEventListener
    {
        public static ImpostorEventListener Instance;

        private int _messagesPerSecond = 0, _movementsPerSecond = 0, _deathsPerSecond = 0, _ventsPerSecond = 0;

        public int ChatMessagesPerSecond { get; private set; }

        public int MovementsPerSecond { get; private set; }

        public int DeathsPerSecond { get; private set; }

        public int VentsPerSecond { get; private set; }

        public ImpostorEventListener()
        {
            Instance = this;
            var tmr = new System.Timers.Timer(1000);
            tmr.AutoReset = true;
            tmr.Elapsed += (a, b) =>
            {
                ChatMessagesPerSecond = _messagesPerSecond;
                MovementsPerSecond = _movementsPerSecond;
                DeathsPerSecond = _deathsPerSecond;
                VentsPerSecond = _ventsPerSecond;
                _messagesPerSecond = 0;
                _movementsPerSecond = 0;
                _deathsPerSecond = 0;
                _ventsPerSecond = 0;
            };
            tmr.Start();
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent evt) => Interlocked.Increment(ref _messagesPerSecond);

        [EventListener]
        public void OnPlayerMovement(IPlayerMovementEvent evt) => Interlocked.Increment(ref _movementsPerSecond);

        [EventListener]
        public void OnPlayerMurder(IPlayerMurderEvent evt) => Interlocked.Increment(ref _deathsPerSecond);

        [EventListener]
        public void OnPlayerVent(IPlayerVentEvent evt) => Interlocked.Increment(ref _ventsPerSecond);
    }
}