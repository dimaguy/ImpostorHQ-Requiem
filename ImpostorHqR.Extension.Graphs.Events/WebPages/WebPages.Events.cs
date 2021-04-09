using System.Drawing;
using System.Timers;
using ImpostorHqR.Extension.Api.Interface;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    public class EventWebService : IExtensionService
    {
        #region Graphs

        public IGraph IGameAlterEventGraph { get; private set; }
        public IGraph IGameCreatedEventGraph { get; private set; }
        public IGraph IGameDestroyedEventGraph { get; private set; }
        public IGraph IGameEndedEventGraph { get; private set; }
        public IGraph IGamePlayerJoinedEventGraph { get; private set; }
        public IGraph IGamePlayerLeftEventGraph { get; private set; }
        public IGraph IGameStartedEventGraph { get; private set; }
        public IGraph IGameStartingEventGraph { get; private set; }

        public IGraph IPlayerChatEventGraph { get; private set; }
        public IGraph IPlayerCompletedTaskEventGraph { get; private set; }
        public IGraph IPlayerDestroyedEventGraph { get; private set; }
        public IGraph IPlayerExileEventGraph { get; private set; }
        public IGraph IPlayerMovementEventGraph { get; private set; }
        public IGraph IPlayerMurderEventGraph { get; private set; }
        public IGraph IPlayerSetStartCounterEventGraph { get; private set; }
        public IGraph IPlayerSpawnedEventGraph { get; private set; }
        public IGraph IPlayerStartMeetingEventGraph { get; private set; }
        public IGraph IPlayerVentEventGraph { get; private set; }

        public IGraph IMeetingEndedEventGraph { get; private set; }
        public IGraph IMeetingStartedEventGraph { get; private set; }

        #endregion

        private IGraphPage Page { get; set; }

        public void Init()
        {
            if (!WebPageConfig.Instance.EnableEventsPage) return;
            CreateGraphs();
            var tmr = new System.Timers.Timer(WebPageConfig.Instance.EventsPageUpdateIntervalSeconds * 1000);
            tmr.AutoReset = true;
            tmr.Elapsed += Tick;
            tmr.Start();
            this.Page = Proxy.Instance.PreInitialization.PageProvider.GraphPageProvider.Create(new IGraph[]
            {
                IGameAlterEventGraph,
                IGameCreatedEventGraph,
                IGameDestroyedEventGraph,
                IGameEndedEventGraph,
                IGamePlayerJoinedEventGraph,
                IGamePlayerLeftEventGraph,
                IGameStartedEventGraph,
                IGameStartingEventGraph,
                IPlayerChatEventGraph,
                IPlayerCompletedTaskEventGraph,
                IPlayerDestroyedEventGraph,
                IPlayerExileEventGraph,
                IPlayerMovementEventGraph,
                IPlayerMurderEventGraph,
                IPlayerSetStartCounterEventGraph,
                IPlayerSpawnedEventGraph,
                IPlayerStartMeetingEventGraph,
                IPlayerVentEventGraph,
                IMeetingEndedEventGraph,
                IMeetingStartedEventGraph,
            }, "Impostor All Events / second", WebPageConfig.Instance.EventsPageHandle, WebPageConfig.Instance.EventsPageWidth);
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            lock (BenchmarkEventListener.syncRoot)
            {
                this.IGameAlterEventGraph.Update(BenchmarkEventListener.Instance.OnGameAlterRate);
                this.IGameCreatedEventGraph.Update(BenchmarkEventListener.Instance.OnGameCreatedRate);
                this.IGameDestroyedEventGraph.Update(BenchmarkEventListener.Instance.OnGameDestroyedRate);
                this.IGameEndedEventGraph.Update(BenchmarkEventListener.Instance.OnGameEndedRate);
                this.IGamePlayerJoinedEventGraph.Update(BenchmarkEventListener.Instance.OnGamePlayerJoinedRate);
                this.IGamePlayerLeftEventGraph.Update(BenchmarkEventListener.Instance.OnGamePlayerLeftRate);
                this.IGameStartedEventGraph.Update(BenchmarkEventListener.Instance.OnGameStartedRate);
                this.IGameStartingEventGraph.Update(BenchmarkEventListener.Instance.OnGameStartingRate);
                this.IPlayerChatEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerChatRate);
                this.IPlayerCompletedTaskEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerCompletedTaskRate);
                this.IPlayerDestroyedEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerDestroyedRate);
                this.IPlayerExileEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerExileRate);
                this.IPlayerMovementEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerMovementRate);
                this.IPlayerMurderEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerMurderRate);
                this.IPlayerSetStartCounterEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerSetStartCounterRate);
                this.IPlayerSpawnedEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerSpawnedRate);
                this.IPlayerStartMeetingEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerStartMeetingRate);
                this.IPlayerVentEventGraph.Update(BenchmarkEventListener.Instance.OnPlayerVentRate);
                this.IMeetingEndedEventGraph.Update(BenchmarkEventListener.Instance.OnMeetingEndedRate);
                this.IMeetingStartedEventGraph.Update(BenchmarkEventListener.Instance.OnMeetingStartedRate);
                this.Page.Update();
            }
        }

        public void PostInit()
        {

        }

        public void Shutdown()
        {

        }

        private readonly Color FillColor = Color.FromArgb(255, 0, 255, 0);

        private readonly Color LineColor = Color.FromArgb(100, 0, 200, 0);

        private const int Span = 30000;

        private const int Delay = 5000;

        private void CreateGraphs()
        {
            #region Game Events

            this.IGameAlterEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Alter Events", FillColor, LineColor, Span, Delay);

            this.IGameCreatedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Created Events", FillColor, LineColor, Span, Delay);

            this.IGameDestroyedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Destroyed Events", FillColor, LineColor, Span, Delay);

            this.IGameEndedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Ended Events", FillColor, LineColor, Span, Delay);

            this.IGamePlayerJoinedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Player Joined Events", FillColor, LineColor, Span, Delay);

            this.IGamePlayerLeftEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Player left Events", FillColor, LineColor, Span, Delay);

            this.IGameStartedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Started Events", FillColor, LineColor, Span, Delay);

            this.IGameStartingEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Game Starting Events", FillColor, LineColor, Span, Delay);

            #endregion

            #region Player Events

            this.IPlayerChatEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Chat Events", FillColor, LineColor, Span, Delay);

            this.IPlayerCompletedTaskEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Completed Task Events", FillColor, LineColor, Span, Delay);

            this.IPlayerDestroyedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Destroyed Events", FillColor, LineColor, Span, Delay);

            this.IPlayerExileEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Exile Events", FillColor, LineColor, Span, Delay);

            this.IPlayerMovementEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Movement Events", FillColor, LineColor, Span, Delay);

            this.IPlayerMurderEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Murder Events", FillColor, LineColor, Span, Delay);

            this.IPlayerSetStartCounterEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Set Start Counter Events", FillColor, LineColor, Span, Delay);

            this.IPlayerSpawnedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Spawned Events", FillColor, LineColor, Span, Delay);

            this.IPlayerStartMeetingEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Start Meeting Events", FillColor, LineColor, Span, Delay);

            this.IPlayerVentEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Player Vent Events", FillColor, LineColor, Span, Delay);

            #endregion

            #region Meeting Events

            this.IMeetingEndedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Meeting Ended Events", FillColor, LineColor, Span, Delay);

            this.IMeetingStartedEventGraph = Proxy.Instance.PreInitialization.PageProvider.GraphProvider
                .Create("Meeting Started Events", FillColor, LineColor, Span, Delay);

            #endregion
        }
    }
}
