using System;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;

namespace ImpostorHqR.Extension.Graphs.Events.WebPages
{
    public class EventWebPage 
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

        private Timer Tmr { get; }

        public static void Create()
        {
            new EventWebPage();
        }

        public EventWebPage()
        {
            Start.OnClosed += Shutdown;
            CreateGraphs();
            
            this.Page = IGraphPage.Create(new IGraph[]
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
            }, "Impostor All Events / second", Start.GetConfig().EventsPageHandle, Start.GetConfig().EventsPageWidth);
            Tmr = new System.Timers.Timer(Start.GetConfig().EventsPageUpdateIntervalSeconds * 1000)
            {
                AutoReset = true
            };
            Tmr.Elapsed += Tick;
            Tmr.Start();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            lock (BenchmarkEventListener.SyncRoot)
            {
                try
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
                catch (Exception exception)
                {
                    ILogManager.Log("FATAL ERROR IN EVENT UPDATE LOOP!", this.ToString(), LogType.Error, ex:exception);
                }
                
            }
        }

        private readonly Color FillColor = Color.FromArgb(255, 0, 255, 0);

        private readonly Color LineColor = Color.FromArgb(100, 0, 200, 0);

        private const int Span = 30000;

        private const int Delay = 5000;

        private void CreateGraphs()
        {
            #region Game Events

            this.IGameAlterEventGraph = IGraph
                .Create("Game Alter Events", FillColor, LineColor, Span, Delay);

            this.IGameCreatedEventGraph = IGraph
                .Create("Game Created Events", FillColor, LineColor, Span, Delay);

            this.IGameDestroyedEventGraph = IGraph
                .Create("Game Destroyed Events", FillColor, LineColor, Span, Delay);

            this.IGameEndedEventGraph = IGraph
                .Create("Game Ended Events", FillColor, LineColor, Span, Delay);

            this.IGamePlayerJoinedEventGraph = IGraph
                .Create("Game Player Joined Events", FillColor, LineColor, Span, Delay);

            this.IGamePlayerLeftEventGraph = IGraph
                .Create("Game Player left Events", FillColor, LineColor, Span, Delay);

            this.IGameStartedEventGraph = IGraph
                .Create("Game Started Events", FillColor, LineColor, Span, Delay);

            this.IGameStartingEventGraph = IGraph
                .Create("Game Starting Events", FillColor, LineColor, Span, Delay);

            #endregion

            #region Player Events

            this.IPlayerChatEventGraph = IGraph
                .Create("Player Chat Events", FillColor, LineColor, Span, Delay);

            this.IPlayerCompletedTaskEventGraph = IGraph
                .Create("Player Completed Task Events", FillColor, LineColor, Span, Delay);

            this.IPlayerDestroyedEventGraph = IGraph
                .Create("Player Destroyed Events", FillColor, LineColor, Span, Delay);

            this.IPlayerExileEventGraph = IGraph
                .Create("Player Exile Events", FillColor, LineColor, Span, Delay);

            this.IPlayerMovementEventGraph = IGraph
                .Create("Player Movement Events", FillColor, LineColor, Span, Delay);

            this.IPlayerMurderEventGraph = IGraph
                .Create("Player Murder Events", FillColor, LineColor, Span, Delay);

            this.IPlayerSetStartCounterEventGraph = IGraph
                .Create("Player Set Start Counter Events", FillColor, LineColor, Span, Delay);

            this.IPlayerSpawnedEventGraph = IGraph
                .Create("Player Spawned Events", FillColor, LineColor, Span, Delay);

            this.IPlayerStartMeetingEventGraph = IGraph
                .Create("Player Start Meeting Events", FillColor, LineColor, Span, Delay);

            this.IPlayerVentEventGraph = IGraph
                .Create("Player Vent Events", FillColor, LineColor, Span, Delay);

            #endregion

            #region Meeting Events

            this.IMeetingEndedEventGraph = IGraph
                .Create("Meeting Ended Events", FillColor, LineColor, Span, Delay);

            this.IMeetingStartedEventGraph = IGraph
                .Create("Meeting Started Events", FillColor, LineColor, Span, Delay);

            #endregion
        }

        public void Shutdown()
        {
            Tmr.Stop();
            Tmr.Dispose();
        }
    }
}
