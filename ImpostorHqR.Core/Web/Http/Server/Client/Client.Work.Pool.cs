using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Server.IO;
using ImpostorHqR.Core.Web.Http.Server.Request;
using ImpostorHqR.Extension.Api.Interface.Helpers;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Web.Http.Server.Client
{
    public class HttpClientWorkPool : IHttpServerMonitor
    {
        public static readonly HttpClientWorkPool Instance = new HttpClientWorkPool();

        private int _busyThreads = 0;

        private int _usersCount = 0;

        private int _rate = 0;

        private readonly TaskFactory _taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.LongRunning, TaskContinuationOptions.None, TaskScheduler.Current);

        public int RequestsPerSecond { get; private set; }

        public int FileDataRateKbPerSecond
        {
            get { lock (_incrementSyncLock) return _rate; }
        }

        public int GetConcurrentDownloads()
        {
            return HttpClientProcessor.ConcurrentFileTransfers;
        }

        private readonly object _incrementSyncLock = new object();

        public HttpClientWorkPool()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ThreadPool.SetMaxThreads(ConfigHolder.Instance.ResourcePoolSize, 1000);
            var tmr = new System.Timers.Timer(1000) {AutoReset = true};
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        public int GetActiveThreads()
        {
            lock (_incrementSyncLock) return _busyThreads;
        }

        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_incrementSyncLock)
            {
                this.RequestsPerSecond = _usersCount;
                _usersCount = 0;
                this._rate = (int)(HttpClientProcessor.TransferRateKBytes);
                HttpClientProcessor.TransferRateKBytes = 0;
            }
        }

        public void PostWorkItem(Socket client)
        {
            Interlocked.Increment(ref _usersCount);
            Task.Factory.StartNew(async () =>
            {
                Interlocked.Increment(ref _busyThreads);
                await Process(client);
                Interlocked.Decrement(ref _busyThreads);
            });
        }

        private async Task Process(Socket tcpClient)
        {
            using (tcpClient)
            {
                try
                {
                    var stream = await SelectProtocol(tcpClient);
                    var requestData = await HttpLineReader.ReadLineSizedBuffered(stream, 2048);
                    if (string.IsNullOrEmpty(requestData)) return;
                    var request = HttpRequestParser.ParseRequest(requestData);
                    if (request == null) return;
                    var holder = new HttpClientHolder(stream, tcpClient, (HttpInitialRequest)request);
                    await HttpClientProcessor.ProcessClient(holder, (HttpInitialRequest)request);
                }
                catch (Exception e)
                {
                    LogManager.Instance.Log(new LogEntry()
                    {
                        Message = $"Http error: {e.ToString()}",
                        Source = this,
                        Type = LogType.Error
                    });
                }
            }
        }

        private static async Task<Stream> SelectProtocol(Socket client)
        {
            var ns = new NetworkStream(client, true);
            if (!ConfigHolder.Instance.EnableSsl) return ns;
            try
            {
                var stream = new SslStream(ns);
                await stream.AuthenticateAsServerAsync(ServerMain.Instance.Ssl, false, SslProtocols.Tls12, true);
                return stream;
            }
            catch
            {
                //fall to insecure protocol if SSL fails to authenticate.
                return ns;
            }
        }
    }
}
