using System;
using System.Net;
using System.Net.Sockets;
using HqResearch.Ssl;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Services;
using ImpostorHqR.Core.Web.Common.Protection.DoS;
using ImpostorHqR.Core.Web.Http.Server.Client;

namespace ImpostorHqR.Core.Web.Http.Server
{
    class ServerMain : IService
    {
        public static ServerMain Instance;

        public X509Certificate2 Ssl = SslCertificateProvider.GetCertificate("anti.the-dying-of-the-light.who");

        private bool Cancel { get; set; }

        private TcpListener Listener { get; set; }

        public ServerMain()
        {
            Instance = this;
        }

        public void Activate() { }

        public void PostInit()
        {
            ConsoleLogging.Instance.LogDebug($"Starting HTTP{(ConfigHolder.Instance.EnableSsl ? "S" : "")} server on {ConfigHolder.Instance.HttpPort}.", this, true);
            this.Listener = new TcpListener(IPAddress.Any, ConfigHolder.Instance.HttpPort);
            this.Listener.Start();
            _ = ListenAsync();
        }

        private async Task ListenAsync()
        {
            ConsoleLogging.Instance.LogDebug($"HTTP acceptor thread started.", this, true);
            while (!Cancel)
            {
                try
                {
                    var client = await this.Listener.AcceptSocketAsync();
                    if (Cancel) return;
                    if (HqServerProtector.Instance.IsAttacking(((IPEndPoint) client.RemoteEndPoint)?.Address))
                    {
                        client.Close();
                        continue;
                    }

                    HttpClientWorkPool.Instance.PostWorkItem(client);
                }
                catch (SocketException)
                {
                    //shutdown
                }
                catch (Exception e)
                {
                    ConsoleLogging.Instance.LogError($"Error in HTTP acceptor: {e}", this, true);
                }
            }
        }

        public void Shutdown()
        {
            this.Cancel = true;
            this.Listener.Stop();
        }
    }
}
