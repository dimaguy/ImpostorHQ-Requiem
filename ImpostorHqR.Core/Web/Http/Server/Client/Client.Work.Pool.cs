using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Server.IO;
using ImpostorHqR.Core.Web.Http.Server.Request;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace ImpostorHqR.Core.Web.Http.Server.Client
{
    public static class HttpClientWorkPool
    {
        private static int _busyThreads = 0;

        private static int _usersCount = 0;

        private static int _rate = 0;

        public static int RequestsPerSecond { get; private set; }

        public static int FileDataRateKbPerSecond => _rate;

        public static int GetConcurrentDownloads()
        {
            return HttpClientProcessor.ConcurrentFileTransfers;
        }

        public static int GetCacheSize() => HttpServerFileCache.CurrentSize;

        private static bool Ssl { get; }

        static HttpClientWorkPool()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Ssl = IConfigurationStore.GetByType<RequiemConfig>().EnableSsl;
            var tmr = new System.Timers.Timer(1000) { AutoReset = true };
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        public static int GetActiveThreads() => _busyThreads;

        private static void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            RequestsPerSecond = _usersCount;
            _usersCount = 0;
            _rate = (int) (HttpClientProcessor.TransferRateKBytes);
            HttpClientProcessor.TransferRateKBytes = 0;
        }

        public static async ValueTask ProcessWorkItemAsync(Socket tcpClient)
        {
            Interlocked.Increment(ref _usersCount);
            Interlocked.Increment(ref _busyThreads);
            var finalized = false;
            try
            {
                var stream = Ssl
                    ? await AuthenticateSsl(tcpClient)
                    : new NetworkStream(tcpClient);

                var requestData = await HttpLineReader.ReadLineSizedBuffered(stream, 2048);
                if (requestData == null || requestData.Length < 10) return;

                var request = HttpRequestParser.ParseRequest(requestData);
                if (request == null) return;

                using var holder = new HttpClientHolder(stream, tcpClient, (HttpInitialRequest) request);
                finalized = true;
                await HttpClientProcessor.ProcessClient(holder, (HttpInitialRequest) request);
            }
            catch (Exception e)
            {
                ILogManager.Log("Http work pool error.", "Client.Work.Pool", LogType.Error, true, true, e);
            }
            finally
            {
                if(!finalized) HttpClientHolder.Close(tcpClient);
                Interlocked.Decrement(ref _busyThreads);
            }
        }

        private static async ValueTask<Stream> AuthenticateSsl(Socket client)
        {
            var ns = new NetworkStream(client, true);
            try
            {
                var stream = new SslStream(ns);
                await stream.AuthenticateAsServerAsync(HttpServer.Ssl, false, SslProtocols.Tls12, true);
                return stream;
            }
            catch
            {
                //fall to insecure protocol if SSL fails to authenticate.
                //not tested, not documented, ??
                return ns;
            }
        }
    }
}
